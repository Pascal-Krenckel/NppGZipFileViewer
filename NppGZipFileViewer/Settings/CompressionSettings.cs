using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NppGZipFileViewer.Settings;

[Serializable]
public abstract class CompressionSettings
{
    public List<string> Extensions { get; set; } = new List<string>();

    public abstract string CompressionAlgorithm { get; }

    public void Compress(Stream inStream, Stream outStream)
    {
        using var compressionStream = GetCompressionStream(outStream);
        inStream.CopyTo(compressionStream);
    }

    public void Decompress(Stream inStream, Stream outStream)
    {
        using var compressionStream = GetDecompressionStream(inStream);
        compressionStream.CopyTo(outStream);
    }

    public abstract Stream GetCompressionStream(Stream outStream);
    public abstract Stream GetDecompressionStream(Stream inStream);

    public override string ToString() => CompressionAlgorithm;

}
