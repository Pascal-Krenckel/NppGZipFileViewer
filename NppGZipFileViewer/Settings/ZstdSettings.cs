using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NppGZipFileViewer.Settings
{
    [Serializable]
    public class ZstdSettings : CompressionSettings
    {
        public int CompressionLevel { get; set; } = 11;
        public int BufferSize { get; set; } = 1024*1024;

        public override string CompressionAlgorithm => "zstd";

        public override Stream GetCompressionStream(Stream outStream)
        {
            ZstdSharp.CompressionStream stream = new ZstdSharp.CompressionStream(outStream,CompressionLevel,BufferSize,true);
            return stream;
        }
        public override Stream GetDecompressionStream(Stream inStream)
        {
            ZstdSharp.DecompressionStream decompressionStream = new ZstdSharp.DecompressionStream(inStream,BufferSize,true);
            return decompressionStream;
        }
    }
}
