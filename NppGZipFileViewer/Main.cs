using Kbg.NppPluginNET.PluginInfrastructure;
using NppGZipFileViewer;
using NppGZipFileViewer.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

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
                    if (Preferences.DecompressAll || Preferences.HasGZipSuffix(nppGateway.GetFullPathFromBufferId(notification.Header.IdFrom)))
                        TryDecompress(notification);
                    break;
                case (uint)NppMsg.NPPN_FILEBEFORESAVE:
                    try
                    {
                        var path = nppGateway.GetFullPathFromBufferId(notification.Header.IdFrom);

                        if (fileTracker.IsExcluded(notification.Header.IdFrom))
                            return;

                        if (!Preferences.HasGZipSuffix(path) && !fileTracker.IsIncluded(notification.Header.IdFrom))
                            return;

                        var nppEnc = (NppEncoding)nppGateway.GetBufferEncoding(notification.Header.IdFrom);
                        var internalEnc = nppEnc == NppEncoding.ANSI || NppEncoding.UTF8 == nppEnc || nppEnc == NppEncoding.UTF8_BOM;

                        scintillaGateway.BeginUndoAction();
                        cursorPosition.Add(notification.Header.IdFrom, scintillaGateway.GetCurrentPos());
                        using var contentStream = NppGZipFileViewerHelper.GetContentStream(notification, path);
                        using var encodedContentStream = NppGZipFileViewerHelper.Encode(contentStream, fileTracker.GetEncoding(notification.Header.IdFrom));
                        NppGZipFileViewerHelper.SetEncodedText(encodedContentStream);
                        scintillaGateway.EndUndoAction();
                    }
                    catch (Exception ex) { }
                    break;
                case (uint)NppMsg.NPPN_FILESAVED:
                    {
                        var nppEnc = (NppEncoding)nppGateway.GetBufferEncoding(notification.Header.IdFrom);
                        var internalEnc = nppEnc == NppEncoding.ANSI || NppEncoding.UTF8 == nppEnc || nppEnc == NppEncoding.UTF8_BOM;
                        

                        var path = nppGateway.GetFullPathFromBufferId(notification.Header.IdFrom);
                        bool toCompress = ShouldBeCompressed(notification);
                        bool wasCompressed = cursorPosition.ContainsKey(notification.Header.IdFrom);                       

                        if (wasCompressed != toCompress)
                        {
                            // save again, but update file tracker based on toCompressed
                            if (toCompress)
                            {
                                Encoding encoding = NppGZipFileViewerHelper.ResetEncoding();
                                fileTracker.Include(notification.Header.IdFrom, path,encoding);
                            }
                            else
                            {
                                fileTracker.Exclude(notification.Header.IdFrom, path);
                                scintillaGateway.Undo(); //undo compression
                                scintillaGateway.GotoPos(cursorPosition[notification.Header.IdFrom]);
                                scintillaGateway.EmptyUndoBuffer();
                                cursorPosition.Remove(notification.Header.IdFrom);
                            }
                            nppGateway.SwitchToFile(path);
                            nppGateway.MakeCurrentBufferDirty();
                            nppGateway.SaveCurrentFile();
                            return;
                        }

                        else if (wasCompressed) // if compressed we need to undo the changes
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

        private static void UpdateStatusbar(IntPtr from, bool resetStatusbar = false)
        {            
            if (fileTracker.IsIncluded(from))
            {                
                var enc = fileTracker.GetEncoding(from);                
                string str = $"gzip/{enc.WebName.ToUpper()}";
                if (enc.GetPreamble().Length > 0)
                    str += " BOM";
                nppGateway.SetStatusBar(NppMsg.STATUSBAR_UNICODE_TYPE, str);
            }          
            else if(resetStatusbar)
            {
                var enc = NppGZipFileViewerHelper.ToEncoding((NppEncoding)nppGateway.GetBufferEncoding(from));
                string str = $"{enc.WebName.ToUpper()}";
                if (enc.GetPreamble().Length > 0)
                    str += " BOM";
                nppGateway.SetStatusBar(NppMsg.STATUSBAR_UNICODE_TYPE, str);
            }
        }
        private static void UpdateCommandChecked(IntPtr from)
        {
            nppGateway.SetMenuItemCheck(0, fileTracker.IsIncluded(from));
        }

        private static bool ShouldBeCompressed(ScNotification notification)
        {
            var newPath = nppGateway.GetFullPathFromBufferId(notification.Header.IdFrom).ToString();

            // no path change -> file tracked
            var oldPath = fileTracker.GetStoredPath(notification.Header.IdFrom);
            if (newPath == oldPath)
            {
                if (fileTracker.IsIncluded(notification.Header.IdFrom)) // is tracked, so compress
                    return true;
                if (fileTracker.IsExcluded(notification.Header.IdFrom)) // is excluded, so don't compress
                    return false;
                return Preferences.HasGZipSuffix(newPath); // no manually set information, store iff gz-suffix (should be tracked then, but who knows) 
            }

            // path changed

            // from gz to non gz -> don't compress (even if tracked, because it might be from the old gz file)
            if (Preferences.HasGZipSuffix(oldPath) && !Preferences.HasGZipSuffix(newPath))
                return false;

            // from non gz to gz -> compress, even if excluded since it might be from the old non gz file
            if (!Preferences.HasGZipSuffix(oldPath) && Preferences.HasGZipSuffix(newPath))
                return true;


            // same suffix type:

            // from gz to gz or non gz to non gz, use tracker

            if (fileTracker.IsIncluded(notification.Header.IdFrom))
                return true;

            if (fileTracker.IsExcluded(notification.Header.IdFrom))
                return false;

            // not tracked -> go by suffix, should always return false, since gz-files should always be tracked
            return Preferences.HasGZipSuffix(newPath);
        }

        private static void TryDecompress(ScNotification notification)
        {
            var path = nppGateway.GetFullPathFromBufferId(notification.Header.IdFrom);

            using var gzContentStream = NppGZipFileViewerHelper.GetContentStream(notification, path);

            if (gzContentStream.Length == 0)
                if (Preferences.HasGZipSuffix(path))
                    fileTracker.Include(notification.Header.IdFrom, path, new UTF8Encoding(false));
                else return;
            try
            {
                using var decodedContentStream = NppGZipFileViewerHelper.Decode(gzContentStream);                
                Encoding encoding = NppGZipFileViewerHelper.SetDecodedText(decodedContentStream);

                nppGateway.SendMenuEncoding(NppEncoding.UTF8);
                Win32.SendMessage(PluginBase.GetCurrentScintilla(), SciMsg.SCI_GOTOPOS, 0, 0);
                Win32.SendMessage(PluginBase.GetCurrentScintilla(), SciMsg.SCI_EMPTYUNDOBUFFER, 0, 0);
                Win32.SendMessage(PluginBase.GetCurrentScintilla(), SciMsg.SCI_SETSAVEPOINT, 0, 0);

                fileTracker.Include(notification.Header.IdFrom, path, encoding);
            }
            catch (InvalidDataException ex)
            {
                if (Preferences.HasGZipSuffix(path))
                    fileTracker.Exclude(notification.Header.IdFrom, path);
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
            catch (Exception ex)
            {
                Preferences = new Preferences(false, ".gz");
            }

            PluginBase.SetCommand(0, "Toogle Compression", ToogleCompress, false);
            PluginBase.SetCommand(1, "---", null);
            PluginBase.SetCommand(2, "Make Compressed", Compress, false);
            PluginBase.SetCommand(3, "Make Decompressed", Decompress, false);
            PluginBase.SetCommand(4, "---", null);
            PluginBase.SetCommand(5, "Settings", OpenSettings);
            PluginBase.SetCommand(6, "About", OpenAbout);
            PluginBase.SetCommand(7, "Credits", OpenCredits);
            SetToolBarIcon();
        }

        private static void Decompress()
        {
            try
            {
                IntPtr bufferId = nppGateway.GetCurrentBufferId();
                var path = nppGateway.GetCurrentFilePath();
                using var contentStream = NppGZipFileViewerHelper.GetCurrentContentStream();
                using var decodedStream = NppGZipFileViewerHelper.Decode(contentStream);
                NppGZipFileViewerHelper.SetDecodedText(decodedStream);
                if (fileTracker.IsIncluded(bufferId))
                    ToogleCompress();
                fileTracker.Exclude(bufferId, path); // make sure it's excluded
            }
            catch (InvalidDataException ex)
            {
                MessageBox.Show($"Couldn't decompress file.\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void Compress()
        {
            IntPtr bufferId = nppGateway.GetCurrentBufferId();
            var path = nppGateway.GetCurrentFilePath();
            using var contentStream = NppGZipFileViewerHelper.GetCurrentContentStream();
            var encoding = NppGZipFileViewerHelper.ToEncoding( (NppEncoding)nppGateway.GetBufferEncoding(bufferId));
            using var encodedStream = NppGZipFileViewerHelper.Encode(contentStream,encoding );
            NppGZipFileViewerHelper.SetEncodedText(encodedStream);
            if (fileTracker.IsIncluded(bufferId))
                ToogleCompress();
            fileTracker.Exclude(bufferId, path); // make sure it's excluded
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
            if (fileTracker.IsIncluded(bufferId))
            {
                var enc = NppGZipFileViewerHelper.ToNppEncoding(fileTracker.GetEncoding(bufferId));
                fileTracker.Exclude(bufferId, nppGateway.GetFullPathFromBufferId(bufferId));
                nppGateway.SendMenuEncoding(enc);
                nppGateway.MakeCurrentBufferDirty();
            }
            else
            {
                var encoding = NppGZipFileViewerHelper.ResetEncoding();
                fileTracker.Include(bufferId, nppGateway.GetFullPathFromBufferId(bufferId),encoding);
                nppGateway.MakeCurrentBufferDirty();
            }

            UpdateCommandChecked(bufferId);
            UpdateStatusbar(bufferId, true);
        }
    }
}