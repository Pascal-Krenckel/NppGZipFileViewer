using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Joveler.Compression.XZ;

namespace NppGZipFileViewer.Settings
{
    public class XZSettings : CompressionSettings
    {
        static XZSettings()
        {
            string currentDir = System.IO.Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location);
            string libDir = "";
            switch (RuntimeInformation.ProcessArchitecture)
            {
                case Architecture.X86:
                    libDir = System.IO.Path.Combine(currentDir, "x86","liblzma.dll");
                    break;
                case Architecture.X64:
                    libDir = System.IO.Path.Combine(currentDir, "x64", "liblzma.dll");                  
                    break;
                case Architecture.Arm64:
                    libDir = System.IO.Path.Combine(currentDir, "arm64", "liblzma.dll");
                    break;                
            }

            XZInit.GlobalInit(libDir);
        }

        public override string CompressionAlgorithm => "xz";

        public int BufferSize { get; set; } = 1024*1024;

        public LzmaCheck ChecksumType { get; set; }

        public bool ExtremeFlag { get; set; } = false;

        public LzmaCompLevel CompressionLevel { get; set; } = LzmaCompLevel.Default;

        private XZCompressOptions CompressionOptions => new XZCompressOptions()
        {
            BufferSize = BufferSize,
            Check = ChecksumType,
            ExtremeFlag = ExtremeFlag,
            LeaveOpen = true,
            Level = CompressionLevel,

        };
        private XZDecompressOptions DecompressOptions => new XZDecompressOptions()
        {
            BufferSize = BufferSize,
            LeaveOpen = true,
        };

        public bool MultiThread { get; set; } = false;
        public ulong BlockSize { get; set; }
        public int Threads { get; set; }

        private XZThreadedCompressOptions ThreadOptions => new XZThreadedCompressOptions() { BlockSize = BlockSize, Threads = Threads };
        private XZThreadedDecompressOptions ThreadDecompressOptions => new XZThreadedDecompressOptions() { Threads = Threads };

        public override Stream GetCompressionStream(Stream outStream)
        {
            if(!MultiThread)
            return new XZStream(outStream, CompressionOptions);
            else 
                return new XZStream(outStream, CompressionOptions,ThreadOptions );
        }
        public override Stream GetDecompressionStream(Stream inStream)
        {
            if (!MultiThread)
                return new XZStream(inStream, DecompressOptions);
            return new XZStream(inStream, DecompressOptions,ThreadDecompressOptions);
        }
    }
}
