using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NppGZipFileViewer;
internal class BOMDetector
{
    // Source: https://stackoverflow.com/questions/3825390/effective-way-to-find-any-files-encoding
    /// <summary>
    /// Determines a arrays encoding by analyzing its byte order mark (BOM).    
    /// </summary>
    /// <param name="bom">The array to analyze.</param>
    /// <returns>The detected encoding.</returns>
    public static BOM GetEncoding(byte[] bom)
    {        
        // Analyze the BOM
        if (bom.Length >= 3 && bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return BOM.UTF7;
        if (bom.Length >= 3 && bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return BOM.UTF8;
        if (bom.Length >= 2 && bom[0] == 0xff && bom[1] == 0xfe) return BOM.UTF16LE;
        if (bom.Length >= 2 && bom[0] == 0xfe && bom[1] == 0xff) return BOM.UTF16BE;
        if (bom.Length >= 4 && bom[0] == 0xff && bom[1] == 0xfe && bom[2] == 0 && bom[3] == 0) return BOM.UTF32LE;
        if (bom.Length >= 4 && bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return BOM.UTF32BE;

        // no BOM found
        return BOM.None;
    }



}

public enum BOM
{
    UTF7, UTF8, UTF16LE, UTF16BE, UTF32LE, UTF32BE, None
}

public enum NppEncoding
{
    ANSI = 0,
    UTF8_BOM = 1,
    UTF16_BE = 2,
    UTF16_LE = 3,
    UTF8 = 4,
}
