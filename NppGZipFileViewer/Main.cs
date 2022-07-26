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
using System.Linq;
using NppGZipFileViewer.Settings;
using System.Reflection;

namespace Kbg.NppPluginNET
{
    class Main
    {
        static Main()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var executingAssemblyName = Assembly.GetExecutingAssembly().GetName().Name + ".dll";
            var targetAssemblyName = new AssemblyName(args.Name).Name + ".dll";
            using var stream =  Assembly.GetExecutingAssembly().GetManifestResourceStream(executingAssemblyName + "." + targetAssemblyName);
            if (stream != null)
            {
                using MemoryStream ms = new MemoryStream();
                stream.CopyTo(ms);
                return Assembly.Load(ms.GetBuffer());
            }

            var p = AppDomain.CurrentDomain.BaseDirectory;
            string path = System.Reflection.Assembly.GetCallingAssembly().Location;
            string libPath = Path.Combine(Path.GetDirectoryName(path), targetAssemblyName);
            if (File.Exists(libPath))
                return System.Reflection.Assembly.LoadFile(libPath);
            else return null;
        }

        internal const string PluginName = "NppGZipFileViewer";

        static FileTracker fileTracker = new FileTracker();
        static string iniFilePath = null;

        static Bitmap tbBmp = NppGZipFileViewer.Properties.Resources.gzip_filled16;


        static Dictionary<IntPtr, Position> cursorPosition = new Dictionary<IntPtr, Position>();
        static Dictionary<IntPtr, CompressionSettings> compressionBeforeSave = new Dictionary<IntPtr, CompressionSettings>();

        public static Preferences Preferences { get; set; }
        static NotepadPPGateway nppGateway;
        static ScintillaGateway scintillaGateway;

        public static void OnNotification(ScNotification notification)
        {
            switch (notification.Header.Code)
            {
                case (uint)NppMsg.NPPN_FILEOPENED:
                    OpenFile(notification);
                    break;
                case (uint)NppMsg.NPPN_FILEBEFORESAVE:
                    try
                    {
                        var path = nppGateway.GetFullPathFromBufferId(notification.Header.IdFrom);

                        var compr = GetFileCompression(notification.Header.IdFrom);

                        // store the current compression settings
                        if (compressionBeforeSave.ContainsKey(notification.Header.IdFrom))
                            compressionBeforeSave[notification.Header.IdFrom] = compr;
                        else
                            compressionBeforeSave.Add(notification.Header.IdFrom, compr);
                        
                        if (cursorPosition.ContainsKey(notification.Header.IdFrom))
                            cursorPosition[notification.Header.IdFrom] = scintillaGateway.GetCurrentPos();
                        else
                            cursorPosition.Add(notification.Header.IdFrom, scintillaGateway.GetCurrentPos());

                        if (compr == null) return;

                        scintillaGateway.BeginUndoAction();
                       
                        
                        using var contentStream = NppGZipFileViewerHelper.GetContentStream(notification, path);
                        var fileEncoding = fileTracker.GetEncoding(notification.Header.IdFrom) ?? new UTF8Encoding(false);
                        using var encodedContentStream = NppGZipFileViewerHelper.Encode(contentStream, fileEncoding,compr);
                        NppGZipFileViewerHelper.SetEncodedText(encodedContentStream);
                        var currentNppEncoding = (NppEncoding)nppGateway.GetBufferEncoding(notification.Header.IdFrom);
                        //if (NppGZipFileViewerHelper.ToNppEncoding(fileEncoding) != currentNppEncoding)
                        //    nppGateway.SendMenuEncoding(NppGZipFileViewerHelper.ToNppEncoding(fileEncoding));
                        scintillaGateway.EndUndoAction();
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "Error at FileBeforeSave", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                    break;
                case (uint)NppMsg.NPPN_FILESAVED:
                {
                    var path = nppGateway.GetFullPathFromBufferId(notification.Header.IdFrom);
                    var targetCompression = ShouldBeCompressed(notification);
                    var oldCompressed = compressionBeforeSave.ContainsKey(notification.Header.IdFrom) ? compressionBeforeSave[notification.Header.IdFrom] : null;

                    if (oldCompressed != targetCompression)
                    {
                        // save again, but update file tracker based on toCompressed
                        if (targetCompression != null)
                        {
                            Encoding encoding = fileTracker.GetEncoding(notification.Header.IdFrom) ?? NppGZipFileViewerHelper.ResetEncoding();
                            fileTracker.Include(notification.Header.IdFrom, path, encoding, targetCompression);
                        }
                        else
                        {
                            fileTracker.Exclude(notification.Header.IdFrom, path);
                            scintillaGateway.Undo(); //undo compression
                            scintillaGateway.GotoPos(cursorPosition[notification.Header.IdFrom]);
                            scintillaGateway.EmptyUndoBuffer();
                        }
                        nppGateway.SwitchToFile(path);
                        scintillaGateway.Undo();
                        scintillaGateway.GotoPos(cursorPosition[notification.Header.IdFrom]);
                        nppGateway.MakeCurrentBufferDirty();
                        nppGateway.SaveCurrentFile();
                        return;
                    }

                    if (oldCompressed != null) // if compressed we need to undo the changes
                    {
                        scintillaGateway.Undo();
                        scintillaGateway.GotoPos(cursorPosition[notification.Header.IdFrom]);

                        scintillaGateway.EmptyUndoBuffer();
                        scintillaGateway.SetSavePoint();
                        scintillaGateway.SetSavePoint();

                    }
                }
                break;
                case (uint)NppMsg.NPPN_FILECLOSED:
                {
                    fileTracker.Remove(notification.Header.IdFrom);
                    cursorPosition.Remove(notification.Header.IdFrom);
                    compressionBeforeSave.Remove(notification.Header.IdFrom);
                    break;
                }
                case (uint)NppMsg.NPPN_BUFFERACTIVATED:
                    // TODO: update Status Bar and Command Check
                    UpdateStatusbar(notification.Header.IdFrom);
                    UpdateCommandChecked(notification.Header.IdFrom);
                    break;

            }

        }

        private static CompressionSettings GetFileCompression(IntPtr idFrom)
        {
            var path = nppGateway.GetFullPathFromBufferId(idFrom);


            if (fileTracker.IsExcluded(idFrom))
                return null; // file excluded

            if (Preferences.GetCompressionBySuffix(path) == null && !fileTracker.IsIncluded(idFrom))
                return null; // neither suffix nor included -> no compression, nothing to do

            // either suffix or included:
            var compr = fileTracker.GetCompressor(idFrom);
            if (compr == null)  // not included -> path
                compr = Preferences.GetCompressionBySuffix(nppGateway.GetFullPathFromBufferId(idFrom));

            return compr;
        }

        private static void OpenFile(ScNotification notification)
        {
            var path = nppGateway.GetFullPathFromBufferId(notification.Header.IdFrom);
            var sourceCompression = Preferences.GetCompressionBySuffix(nppGateway.GetFullPathFromBufferId(notification.Header.IdFrom));
            using var gzContentStream = NppGZipFileViewerHelper.GetContentStream(notification, path);

            if (sourceCompression != null)
            {
                if (gzContentStream.Length == 0) // Empty file:
                {
                    var encoding = NppGZipFileViewerHelper.ResetEncoding();
                    fileTracker.Include(notification.Header.IdFrom, path, encoding, sourceCompression);
                    return;
                }
                var enc = TryDecompress(gzContentStream, sourceCompression);
                if (enc != null)
                { // was able to decompress
                    fileTracker.Include(notification.Header.IdFrom, path, enc, sourceCompression);
                    return;
                }
            }
            if (Preferences.DecompressAll)
                foreach (var compression in Preferences.EnumerateCompressions())
                {
                    gzContentStream.Seek(0, SeekOrigin.Begin);
                    var enc = TryDecompress(gzContentStream, compression);
                    if (enc != null)
                    { // was able to decompress
                        fileTracker.Include(notification.Header.IdFrom, path, enc, compression);
                        return;
                    }
                }

            // no compression found:
            if(sourceCompression != null) // could not compress file although it has a specifix suffix -> exclude this file
                fileTracker.Exclude(notification.Header.IdFrom, path);
        }

        private static void UpdateStatusbar(IntPtr from, bool resetStatusbar = false)
        {
            if (fileTracker.IsIncluded(from))
            {
                var enc = fileTracker.GetEncoding(from);
                string str = $"{fileTracker.GetCompressor(from).CompressionAlgorithm}/{enc.WebName.ToUpper()}";
                if (enc.GetPreamble().Length > 0)
                    str += " BOM";
                nppGateway.SetStatusBar(NppMsg.STATUSBAR_UNICODE_TYPE, str);
            }
            else if (resetStatusbar)
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
            var compr = fileTracker.GetCompressor(from);
            foreach (var posCompr in Preferences.EnumerateCompressions())
                nppGateway.SetMenuItemCheck(posCompr.CompressionAlgorithm, compr?.CompressionAlgorithm == posCompr.CompressionAlgorithm);
        }

        private static CompressionSettings ShouldBeCompressed(ScNotification notification)
        {
            var newPath = nppGateway.GetFullPathFromBufferId(notification.Header.IdFrom).ToString();

            // no path change -> file tracked
            var oldPath = fileTracker.GetStoredPath(notification.Header.IdFrom);
            if (newPath == oldPath)
            {
                if (fileTracker.IsIncluded(notification.Header.IdFrom)) // is tracked, so compress
                    return fileTracker.GetCompressor(notification.Header.IdFrom);
                if (fileTracker.IsExcluded(notification.Header.IdFrom)) // is excluded, so don't compress
                    return null;
                return Preferences.GetCompressionBySuffix(newPath); // no manually set information, store iff gz-suffix (should be tracked then, but who knows) 
            }

            // path changed

            // compression based on suffix changed: return compression for new path
            if (Preferences.GetCompressionBySuffix(oldPath)  != Preferences.GetCompressionBySuffix(newPath))
                return Preferences.GetCompressionBySuffix(newPath);

            // same suffix type:

            // from gz to gz or non gz to non gz, use tracker

            if (fileTracker.IsIncluded(notification.Header.IdFrom))
                return fileTracker.GetCompressor(notification.Header.IdFrom);

            if (fileTracker.IsExcluded(notification.Header.IdFrom))
                return null;

            // not tracked -> go by suffix, should always return false, since gz-files should always be tracked
            return Preferences.GetCompressionBySuffix(newPath);
        }

        private static Encoding TryDecompress(Stream contentStream, CompressionSettings compression)
        { 
            try
            {
                using var decodedContentStream = NppGZipFileViewerHelper.Decode(contentStream, compression);
                Encoding encoding = NppGZipFileViewerHelper.SetDecodedText(decodedContentStream);

                nppGateway.SendMenuEncoding(NppEncoding.UTF8);
                Win32.SendMessage(PluginBase.GetCurrentScintilla(), SciMsg.SCI_GOTOPOS, 0, 0);
                Win32.SendMessage(PluginBase.GetCurrentScintilla(), SciMsg.SCI_EMPTYUNDOBUFFER, 0, 0);
                Win32.SendMessage(PluginBase.GetCurrentScintilla(), SciMsg.SCI_SETSAVEPOINT, 0, 0);

                return encoding;
            }
            catch (Exception ex)
            {
                return null;
                    
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
                Preferences = Preferences.Default;
            }           


            PluginBase.SetCommand(0, "Toggle Compression", ToogleCompress, false);
            PluginBase.SetCommand(1, "---", null);
            PluginBase.SetCommand(2, "Compress", Compress, false);
            PluginBase.SetCommand(3, "Decompress", Decompress, false);
            PluginBase.SetCommand(4, "---", null);
            PluginBase.SetCommand(5, "Settings", OpenSettings);
            PluginBase.SetCommand(6, "About", OpenAbout);
            PluginBase.SetCommand(7, "Credits", OpenCredits);
            PluginBase.SetCommand(8, "---", null);
            SetCompressionCommands(9);
            SetToolBarIcon();
        }

        private static void SetCompressionCommands(int startIndex)
        {
            foreach(var compr in Preferences.EnumerateCompressions())
            {
                PluginBase.SetCommand(startIndex++, compr.CompressionAlgorithm, () => SetCompression(compr.CompressionAlgorithm));
            }
        }

        

        
        private static void Decompress()
        {
            DecompressForm decompressForm = new DecompressForm();
            if(decompressForm.ShowDialog() == DialogResult.OK)
            {
                IntPtr bufferId = nppGateway.GetCurrentBufferId();
                var compr = decompressForm.CompressionSettings;
                using var contentStream = NppGZipFileViewerHelper.GetCurrentContentStream();
                using var decodedContentStream = NppGZipFileViewerHelper.Decode(contentStream, compr);
                var enc = NppGZipFileViewerHelper.SetDecodedText(decodedContentStream);
                var nppEnc = NppGZipFileViewerHelper.ToNppEncoding(enc);
                nppGateway.SendMenuEncoding(nppEnc);
            }         
        }

        private static void Compress()
        {      
            CompressForm compressForm = new CompressForm();
            if(compressForm.ShowDialog() == DialogResult.OK)
            {
                IntPtr bufferId = nppGateway.GetCurrentBufferId();
                var compr = compressForm.CompressionSettings;
                using var contentStream = NppGZipFileViewerHelper.GetCurrentContentStream();
                using var encodedContentStream = NppGZipFileViewerHelper.Encode(contentStream, fileTracker.GetEncoding(bufferId) ?? new UTF8Encoding(false), compr);
                NppGZipFileViewerHelper.SetEncodedText(encodedContentStream);
                nppGateway.SendMenuEncoding(NppEncoding.UTF8); // Set MenuEncoding to match scintillas internal buffer encoding
                // if it's not UTF-8... who cares
            }          
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
            var compressor = Preferences.GetNextCompressor(fileTracker.GetCompressor(bufferId)?.CompressionAlgorithm, Preferences.GetCompressionBySuffix(nppGateway.GetFullPathFromBufferId(bufferId)));

            SetCompression(bufferId, compressor);

            UpdateCommandChecked(bufferId);
            UpdateStatusbar(bufferId, true);
        }

        private static void SetCompression(IntPtr bufferId, CompressionSettings compressor)
        {
            if (null == compressor)
            {
                var enc = NppGZipFileViewerHelper.ToNppEncoding(fileTracker.GetEncoding(bufferId) ?? new UTF8Encoding(false));
                fileTracker.Exclude(bufferId, nppGateway.GetFullPathFromBufferId(bufferId));
                nppGateway.SendMenuEncoding(enc);
                nppGateway.MakeCurrentBufferDirty();
            }
            else
            {
                var encoding = fileTracker.GetEncoding(bufferId) ?? NppGZipFileViewerHelper.ResetEncoding();
                fileTracker.Include(bufferId, nppGateway.GetFullPathFromBufferId(bufferId), encoding, compressor);
                nppGateway.MakeCurrentBufferDirty();
            }
        }

        private static void SetCompression(string compressionAlgorithm)
        {
            IntPtr bufferId = nppGateway.GetCurrentBufferId();
            var compressor = Preferences.EnumerateCompressions().FirstOrDefault(alg => alg.CompressionAlgorithm == compressionAlgorithm);

            compressor = fileTracker.GetCompressor(bufferId) == compressor ? null : compressor; // if already compressed (same compressor) disable compression

            SetCompression(bufferId, compressor);


            UpdateCommandChecked(bufferId);
            UpdateStatusbar(bufferId, true);
        }
    }
}