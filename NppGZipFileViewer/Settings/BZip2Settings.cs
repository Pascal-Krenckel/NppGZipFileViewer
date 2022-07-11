using ICSharpCode.SharpZipLib.BZip2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NppGZipFileViewer.Settings;

[Serializable]
public class BZip2Settings : CompressionSettings
{
    public BZip2Settings()
    {
    }

    public int CompressionLevel { get; set; } = 6;

    public override string CompressionAlgorithm => "bzip2";

    public override Stream GetCompressionStream(Stream outStream) =>
        new BZip2OutputStream(outStream, CompressionLevel)
        {
            IsStreamOwner = false,

        };
    public override Stream GetDecompressionStream(Stream inStream) =>
        new BZip2InputStream(inStream)
        {
            IsStreamOwner = false,
        };
}