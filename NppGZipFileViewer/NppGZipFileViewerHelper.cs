using Kbg.NppPluginNET.PluginInfrastructure;
using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;

namespace NppGZipFileViewer
{
    static class NppGZipFileViewerHelper
    {
        public static StringBuilder GetFilePath(ScNotification notification)
        {
            StringBuilder path = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETFULLPATHFROMBUFFERID, notification.Header.IdFrom, path);
            return path;
        }

        internal static MemoryStream GetContentStream(ScNotification notification, StringBuilder path)
        {
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_SWITCHTOFILE, notification.Header.IdFrom, path);

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

        internal static MemoryStream Decode(Stream gzStream)
        {
            using GZipStream decoder = new GZipStream(gzStream, CompressionMode.Decompress, true);
            MemoryStream decodedStream = new MemoryStream();
            decoder.CopyTo(decodedStream);
            return decodedStream;
        }

        internal static void SetText(MemoryStream decodedContentStream)
        {
            var pinnedArray = GCHandle.Alloc(decodedContentStream.GetBuffer(), GCHandleType.Pinned);

            Win32.SendMessage(PluginBase.GetCurrentScintilla(), SciMsg.SCI_CLEARALL, 0, 0);
            Win32.SendMessage(PluginBase.GetCurrentScintilla(), SciMsg.SCI_APPENDTEXT, (int)decodedContentStream.Length, pinnedArray.AddrOfPinnedObject());
            pinnedArray.Free();
        }

        internal static MemoryStream Encode(Stream stream)
        {
            MemoryStream encodedStream = new MemoryStream();
            using GZipStream encoder = new GZipStream(encodedStream, CompressionMode.Compress, true);
            stream.CopyTo(encoder);
            return encodedStream;
        }
    }
}
