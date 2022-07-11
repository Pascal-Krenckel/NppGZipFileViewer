using Kbg.NppPluginNET.PluginInfrastructure;
using NppGZipFileViewer.Settings;
using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;

namespace NppGZipFileViewer
{
    static class NppGZipFileViewerHelper
    {
        internal static MemoryStream GetContentStream(ScNotification notification, StringBuilder path)
        {
            return GetContentStream(notification.Header.IdFrom, path.ToString());
        }

        private static MemoryStream GetContentStream(IntPtr idFrom, string path)
        {
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_SWITCHTOFILE, idFrom, path);

            int data_length = (int)Win32.SendMessage(PluginBase.GetCurrentScintilla(), SciMsg.SCI_GETLENGTH, 0, 0);
            if (data_length <= 0)
                return new MemoryStream();

            var pData = Win32.SendMessage(PluginBase.GetCurrentScintilla(), SciMsg.SCI_GETCHARACTERPOINTER, 0, 0);
            if (pData == IntPtr.Zero)
                return new MemoryStream();
            MemoryStream memoryStream = new MemoryStream();
            memoryStream.SetLength(data_length);
            Marshal.Copy(pData, memoryStream.GetBuffer(), 0, data_length);
            return memoryStream;
        }

        internal static MemoryStream GetContentStream(ScNotification notification, string path)
        {
            return GetContentStream(notification.Header.IdFrom, path);
        }

        internal static MemoryStream GetCurrentContentStream()
        {

            int data_length = (int)Win32.SendMessage(PluginBase.GetCurrentScintilla(), SciMsg.SCI_GETLENGTH, 0, 0);
            if (data_length <= 0)
                return new MemoryStream();

            var pData = Win32.SendMessage(PluginBase.GetCurrentScintilla(), SciMsg.SCI_GETCHARACTERPOINTER, 0, 0);
            if (pData == IntPtr.Zero)
                return new MemoryStream();
            MemoryStream memoryStream = new MemoryStream();
            memoryStream.SetLength(data_length);
            Marshal.Copy(pData, memoryStream.GetBuffer(), 0, data_length);
            return memoryStream;
        }

        internal static MemoryStream Decode(Stream gzStream, CompressionSettings compression)
        {
            MemoryStream decodedStream = new MemoryStream();
            compression.Decompress(gzStream, decodedStream);
            return decodedStream;
        }

        internal static Encoding ToEncoding(NppEncoding nppEncoding)
        {
            switch(nppEncoding)
            {
                case NppEncoding.UTF16_LE: return new UnicodeEncoding(false, true);
                default:
                case NppEncoding.UTF8: return new UTF8Encoding(false);
                case NppEncoding.UTF8_BOM: return new UTF8Encoding(true);
                case NppEncoding.ANSI: return new ASCIIEncoding();
                case NppEncoding.UTF16_BE: return new UnicodeEncoding(true, true);                 
            }
            
        }

        internal static Encoding SetDecodedText(MemoryStream decodedContentStream)
        {
            NotepadPPGateway nppGateway = new NotepadPPGateway();
            ScintillaGateway scintillaGateway = new ScintillaGateway(PluginBase.GetCurrentScintilla());
           
            //var encoding = nppGateway.GetBufferEncoding(nppGateway.GetCurrentBufferId());

            decodedContentStream.Position = 0;
            byte[] bom = new byte[Math.Min(4, decodedContentStream.Length)];
            
            decodedContentStream.Read(bom, 0, bom.Length);
            decodedContentStream.Position = 0;
            Encoding srcEncoding;
            switch (BOMDetector.GetEncoding(bom))
            {
                case BOM.UTF8:
                    srcEncoding = new UTF8Encoding(true);
                    break;
                case BOM.UTF16LE:
                    srcEncoding = new UnicodeEncoding(false, true);
                    break;
                case BOM.UTF16BE:
                    srcEncoding = new UnicodeEncoding(true, true);
                    break;
                case BOM.UTF7:
                case BOM.UTF32LE:
                case BOM.UTF32BE:
                case BOM.None:
                default:
                    srcEncoding = new UTF8Encoding();
                    break;
            }

            byte[] buffer = Encoding.Convert(srcEncoding, new UTF8Encoding(false), decodedContentStream.GetBuffer(), 0, (int)decodedContentStream.Length);
            
            var pinnedArray = GCHandle.Alloc(buffer, GCHandleType.Pinned);

            Win32.SendMessage(PluginBase.GetCurrentScintilla(), SciMsg.SCI_CLEARALL, 0, 0);
            scintillaGateway.SetCodePage(65001);
            Win32.SendMessage(PluginBase.GetCurrentScintilla(), SciMsg.SCI_ADDTEXT, buffer.Length, pinnedArray.AddrOfPinnedObject());
            pinnedArray.Free();
            return srcEncoding;
        }

        internal static NppEncoding ToNppEncoding(Encoding encoding)
        {
            return encoding?.CodePage switch
            {
                // UTF-8
                65001 => encoding.GetPreamble().Length == 0 ? NppEncoding.UTF8 : NppEncoding.UTF8_BOM,
                // utf-16be
                1201 => NppEncoding.UTF16_BE,
                // utf-16le
                1200 => NppEncoding.UTF16_LE,
                // iso-8859-1
                1252 => NppEncoding.ANSI,
                // default
                _ => NppEncoding.UTF8,
            };
        }

        internal static Encoding ResetEncoding()
        {
            NotepadPPGateway gateway = new NotepadPPGateway();
            ScintillaGateway scintillaGateway = new ScintillaGateway(PluginBase.GetCurrentScintilla());
            var bufferID = gateway.GetCurrentBufferId();
            long nppEnc = gateway.GetBufferEncoding(bufferID);
            Encoding encoding = ToEncoding((NppEncoding)nppEnc);
            scintillaGateway.SetCodePage(65001);
            gateway.SendMenuEncoding(NppEncoding.UTF8);
            return encoding;
        }

        internal static void SetEncodedText(MemoryStream encodedContentStream)
        {
            NotepadPPGateway nppGateway = new NotepadPPGateway();
            ScintillaGateway scintillaGateway = new ScintillaGateway(PluginBase.GetCurrentScintilla());
            var pinnedArray = GCHandle.Alloc(encodedContentStream.GetBuffer(), GCHandleType.Pinned);

            //Win32.SendMessage(PluginBase.GetCurrentScintilla(), SciMsg.SCI_CLEARALL, 0, 0);
            
            scintillaGateway.ClearAll();
            Win32.SendMessage(PluginBase.GetCurrentScintilla(), SciMsg.SCI_ADDTEXT, (int)encodedContentStream.Length, pinnedArray.AddrOfPinnedObject());            
            pinnedArray.Free();
        }

        internal static MemoryStream Encode(Stream stream, Encoding dstEncoding, CompressionSettings compression)
        {
            ScintillaGateway scintillaGateway = new ScintillaGateway(PluginBase.GetCurrentScintilla());
            MemoryStream encodedStream = new MemoryStream();
            using Stream compressionStream = compression.GetCompressionStream(encodedStream);

            Encoding srcEncoding = Encoding.GetEncoding(scintillaGateway.GetCodePage());
           

            if (srcEncoding == dstEncoding)
                stream.CopyTo(compressionStream);
            else
            {
                using MemoryStream mem = new MemoryStream();
                stream.CopyTo(mem);
                byte[] buffer = Encoding.Convert(srcEncoding, dstEncoding, mem.GetBuffer(), 0, (int)mem.Length);
                if (dstEncoding != new UTF8Encoding(false))
                {
                    var preamble = dstEncoding.GetPreamble();
                    compressionStream.Write(preamble, 0, preamble.Length);
                }
                compressionStream.Write(buffer,0, buffer.Length);
            }
            return encodedStream;
        }
    }
}
