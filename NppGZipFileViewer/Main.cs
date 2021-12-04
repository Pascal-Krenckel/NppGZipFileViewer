using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.IO.Compression;
using Kbg.NppPluginNET.PluginInfrastructure;
using NppGZipFileViewer;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NppGZipFileViewer.Forms;

namespace Kbg.NppPluginNET
{
    class Main
    {
        internal const string PluginName = "NppGZipFileViewer";
        
        static FileTracker fileTracker = new FileTracker();
        static string iniFilePath = null;
        
        static Bitmap tbBmp = NppGZipFileViewer.Properties.Resources.gzip_filled16;
        
        
        static Dictionary<IntPtr, Position> cursorPosition = new Dictionary<IntPtr, Position>();
        static Preferences Preferences { get; set; }
        static NotepadPPGateway nppGateway;
        static ScintillaGateway scintillaGateway;
        
        public static void OnNotification(ScNotification notification)
        {            
            switch (notification.Header.Code)
            {
                case (uint)NppMsg.NPPN_FILEOPENED:
                    if (Preferences.DecompressAll || Preferences.HasGZipSuffix(NppGZipFileViewerHelper.GetFilePath(notification)))
                        TryDecompress(notification);                  
                    break;
                case (uint)NppMsg.NPPN_FILEBEFORESAVE:
                    try
                    {
                        var path = NppGZipFileViewerHelper.GetFilePath(notification);

                        if (!Preferences.HasGZipSuffix(path) && !fileTracker.Contains(notification.Header.IdFrom))
                            return;

                        using var contentStream = NppGZipFileViewerHelper.GetContentStream(notification, path);
                        using var encodedContentStream = NppGZipFileViewerHelper.Encode(contentStream);

                        cursorPosition.Add(notification.Header.IdFrom, scintillaGateway.GetCurrentPos());
                        scintillaGateway.BeginUndoAction();
                        NppGZipFileViewerHelper.SetText(encodedContentStream);
                        scintillaGateway.EndUndoAction();
                    }
                    catch(Exception ex) { }
                    break;
                case (uint)NppMsg.NPPN_FILESAVED:
                    {
                        var path = NppGZipFileViewerHelper.GetFilePath(notification);
                        bool toCompress = ShouldBeCompressed(notification);
                        bool wasCompressed = cursorPosition.ContainsKey(notification.Header.IdFrom);
                        
                        if(wasCompressed != toCompress)
                        {
                            // save again, but update file tracker based on toCompressed
                            if (toCompress)
                                fileTracker.Include(notification.Header.IdFrom, path);
                            else fileTracker.Remove(notification.Header.IdFrom);
                            nppGateway.SwitchToFile(path);
                            nppGateway.MakeCurrentBufferDirty();
                            nppGateway.SaveCurrentFile();
                            return;
                        }

                        else if(wasCompressed) // if compressed we need to undo the changes
                        { 
                            scintillaGateway.Undo();
                            scintillaGateway.GotoPos(cursorPosition[notification.Header.IdFrom]);
                            scintillaGateway.EmptyUndoBuffer();
                            scintillaGateway.SetSavePoint();
                        }                        
                        cursorPosition.Remove(notification.Header.IdFrom);
                    }
                    break;
                case (uint)NppMsg.NPPN_FILECLOSED:
                    {
                        fileTracker.Remove(notification.Header.IdFrom);
                        break;
                    }
                case (uint)NppMsg.NPPN_BUFFERACTIVATED:
                    // TODO: update Status Bar and Command Check
                    UpdateStatusbar(notification.Header.IdFrom);
                    UpdateCommandChecked(notification.Header.IdFrom);
                    break;

            }

        }
        
        private static void UpdateStatusbar(IntPtr from)
        {
            string str;
            var enc = Encoding.GetEncoding( scintillaGateway.GetCodePage());
            if (fileTracker.Contains(from))
                str = $"gzip/{enc.WebName}";
            else str = $"{enc.WebName}";
            nppGateway.SetStatusBar(NppMsg.STATUSBAR_UNICODE_TYPE, str);
        }
        private static void UpdateCommandChecked(IntPtr from)
        {
            nppGateway.SetMenuItemCheck("Compress", fileTracker.Contains(from));
        }

        private static bool ShouldBeCompressed(ScNotification notification)
        {           
            var newPath = NppGZipFileViewerHelper.GetFilePath(notification).ToString();

            // if file with GZip suffix
            if (Preferences.HasGZipSuffix(newPath)) return true;

            // no path change -> file tracked
            var oldPath = fileTracker.GetStoredPath(notification.Header.IdFrom);
            if (newPath == oldPath)
                return fileTracker.Contains(notification.Header.IdFrom);

            // path changed (not GZip suffix,already handled), but not tracked -> false
            if (!fileTracker.Contains(notification.Header.IdFrom))
                return false;

            // path changed, from gz (tracked), to not -> do not compress
            if (Preferences.HasGZipSuffix(oldPath))
                return false;

            // path changed && tracked -> true, create a copy if compression is not wanted
            return true;
        }

        private static void TryDecompress(ScNotification notification)
        {
            var path = NppGZipFileViewerHelper.GetFilePath(notification);
                       

            using var gzContentStream = NppGZipFileViewerHelper.GetContentStream(notification, path);
            
            try
            {
                using var decodedContentStream = NppGZipFileViewerHelper.Decode(gzContentStream);
                NppGZipFileViewerHelper.SetText(decodedContentStream);
                Win32.SendMessage(PluginBase.GetCurrentScintilla(), SciMsg.SCI_GOTOPOS, 0, 0);
                Win32.SendMessage(PluginBase.GetCurrentScintilla(), SciMsg.SCI_EMPTYUNDOBUFFER, 0, 0);
                Win32.SendMessage(PluginBase.GetCurrentScintilla(), SciMsg.SCI_SETSAVEPOINT, 0, 0);
                fileTracker.Include(notification.Header.IdFrom,path);
            }
            catch(InvalidDataException ex)
            {
                if (Preferences.HasGZipSuffix(path))
                    fileTracker.Include(notification.Header.IdFrom,path);
            }
            
        }
        internal static void CommandMenuInit()
        {
            nppGateway = new NotepadPPGateway();
            scintillaGateway = new ScintillaGateway(PluginBase.GetCurrentScintilla());

            StringBuilder sbIniFilePath = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETPLUGINSCONFIGDIR, Win32.MAX_PATH, sbIniFilePath);
            iniFilePath = sbIniFilePath.ToString();
            if (!Directory.Exists(iniFilePath)) Directory.CreateDirectory(iniFilePath);
            iniFilePath = Path.Combine(iniFilePath, PluginName + ".config");
            
            try
            {
                Preferences = Preferences.Deserialize(iniFilePath);
            }
            catch(Exception ex)
            {
                Preferences = new Preferences(false, ".gz");
            }

            PluginBase.SetCommand(0, "Compress", ToogleCompress,false);
            PluginBase.SetCommand(1, "---",null);
            PluginBase.SetCommand(2, "Settings", OpenSettings);
            PluginBase.SetCommand(3, "About", OpenAbout);
            PluginBase.SetCommand(3, "Credits", OpenCredits);
            SetToolBarIcon();
        }

        private static void OpenCredits()
        {
            Credits credits = new Credits();
            credits.ShowDialog();
        }

        private static void OpenAbout()
        {
            AboutNppGZip about = new AboutNppGZip();
            about.Show();
        }

        private static void OpenSettings()
        {
            var settingDialog = new SettingsDialog();
            settingDialog.Preferences = Preferences;
            if (settingDialog.ShowDialog() == DialogResult.OK)
            {
                Preferences = settingDialog.Preferences;
                Preferences.Serialize(iniFilePath);

            }
        }

        internal static void SetToolBarIcon()
        {
            toolbarIcons tbIcons = new toolbarIcons();
            tbIcons.hToolbarBmp = tbBmp.GetHbitmap();
            tbIcons.hToolbarIcon = tbBmp.GetHicon();
            tbIcons.hToolbarIconDarkMode = tbBmp.GetHicon();
            IntPtr pTbIcons = Marshal.AllocHGlobal(Marshal.SizeOf(tbIcons));
            Marshal.StructureToPtr(tbIcons, pTbIcons, false);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_ADDTOOLBARICON_FORDARKMODE, PluginBase._funcItems.Items[0]._cmdID, pTbIcons);
            Marshal.FreeHGlobal(pTbIcons);
        }

        internal static void PluginCleanUp()
        {
            Preferences.Serialize(iniFilePath);    
        }
        
        internal static void ToogleCompress()
        {
            IntPtr bufferId = nppGateway.GetCurrentBufferId();
            if (fileTracker.Contains(bufferId))
                if (Preferences.HasGZipSuffix(nppGateway.GetFullPathFromBufferId(bufferId)))
                    MessageBox.Show("This files suffix is a gzip suffix. This file will always be compressed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    fileTracker.Remove(bufferId);
                    nppGateway.MakeCurrentBufferDirty();
                }
            else
            {
                fileTracker.Include(bufferId, nppGateway.GetFullPathFromBufferId(bufferId));
                nppGateway.MakeCurrentBufferDirty();
            }

            UpdateCommandChecked(bufferId);
            UpdateStatusbar(bufferId);           
        }
    }
}