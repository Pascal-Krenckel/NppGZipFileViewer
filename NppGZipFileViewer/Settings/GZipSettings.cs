using ICSharpCode.SharpZipLib.GZip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NppGZipFileViewer.Settings;

[Serializable]
public class GZipSettings : CompressionSettings
{
    public GZipSettings()
    {
        
    }

    public int CompressionLevel { get; set; } = 6;
    public int BufferSize { get; set; } = 512;

    public override string CompressionAlgorithm => "gzip";


    public override Stream GetCompressionStream(Stream outStream)
    {
        GZipOutputStream outputStream = new GZipOutputStream(outStream, BufferSize);
        outputStream.IsStreamOwner = false;
        outputStream.SetLevel(CompressionLevel);
        return outputStream;
    }

    public override Stream GetDecompressionStream(Stream inStream) =>
        new GZipInputStream(inStream)
        {
            IsStreamOwner = false
        };
}
