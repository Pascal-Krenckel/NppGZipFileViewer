using NppGZipFileViewer.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Reflection;
using System.Collections;

namespace NppGZipFileViewer
{
    [Serializable]
    public class Preferences
    {        
        public const int VERSION = 4;
        public int Version { get; set; } = VERSION;

        public bool DecompressAll { get; set; }


        public Preferences() : this(false) { }
        public Preferences(bool decompressAll)
        {
            DecompressAll = decompressAll;
            //OpenAsUTF8 = openAsUTF8;
        }

        public Settings.GZipSettings GZipSettings { get; set; } = new Settings.GZipSettings();
        public Settings.BZip2Settings BZip2Settings { get; set; } = new Settings.BZip2Settings();

        public Settings.XZSettings XZSettings { get; set; } = new XZSettings();

        public Settings.ZstdSettings ZstdSettings { get; set; } = new ZstdSettings();

        public List<string> CompressionAlgorithms { get; set; } = new List<string>();

        public CompressionSettings GetCompressionBySuffix(string path)
        {
            return EnumerateCompressions().FirstOrDefault(comp => comp.Extensions.Any(ext => path?.EndsWith(ext, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        public CompressionSettings GetCompressionBySuffix(StringBuilder path)
        {
            return GetCompressionBySuffix(path.ToString());
        }

        public void Serialize(string path)
        {
            using Stream streams = new FileStream(path, FileMode.Create, FileAccess.Write);
            Serialize(streams);
        }
        public void Serialize(Stream to)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Preferences));
                serializer.Serialize(to, this);
            }
            catch (Exception ex) { System.Windows.Forms.MessageBox.Show(ex.Message, "Error while serialize settings", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error); }
        }

        public static Preferences Deserialize(string path)
        {
            using Stream streams = new FileStream(path, FileMode.Open, FileAccess.Read);
            return Deserialize(streams);
        }

        public static Preferences Deserialize(Stream from)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Preferences));
            var pref = serializer.Deserialize(from) as Preferences;
            pref.CompressionAlgorithms = pref.CompressionAlgorithms.Distinct().ToList();
            if (pref.Version < 2)
            {
                pref.GZipSettings = Preferences.Default.GZipSettings;
                pref.BZip2Settings = Default.BZip2Settings;
            }
            if (pref.Version < 3)
            {
                pref.XZSettings = Preferences.Default.XZSettings;
            }
            if (pref.Version < 4)
            {
                pref.ZstdSettings = Preferences.Default.ZstdSettings;
            }
            pref.Version = Preferences.VERSION;
            return pref;
        }

        public CompressionSettings GetNextCompressor(string compressionAlgorithm, CompressionSettings compressionBySuffix)
        {
            string cAlg;
            if (string.IsNullOrWhiteSpace(compressionAlgorithm))
                cAlg = compressionBySuffix?.CompressionAlgorithm ?? CompressionAlgorithms.FirstOrDefault();
            else
                cAlg = (compressionAlgorithm != compressionBySuffix?.CompressionAlgorithm ?

                    CompressionAlgorithms
                    .SkipWhile(alg => alg != compressionAlgorithm)
                    .Skip(1)
                    :
                    CompressionAlgorithms
                    )
                    .FirstOrDefault(algName => algName != compressionBySuffix?.CompressionAlgorithm);


            return EnumerateCompressions().FirstOrDefault(comp => comp.CompressionAlgorithm == cAlg);

        }

        public static Preferences Default
        {
            get
            {
                Preferences preferences = new Preferences(false);
                preferences.CompressionAlgorithms = preferences.EnumerateCompressions().Select(c => c.CompressionAlgorithm).ToList();
                preferences.GZipSettings.Extensions.AddRange(new[] { ".gz", ".gzip" });
                preferences.BZip2Settings.Extensions.AddRange(new[] { ".bz2", ".bzip2" });
                preferences.XZSettings.Extensions.AddRange(new[] { ".xz" });
                preferences.ZstdSettings.Extensions.AddRange(new[] { ".zst" });
                return preferences;
            }
        }

        public IEnumerable<CompressionSettings> EnumerateCompressions()
        {
            return GetType().GetProperties().Where(m => m.PropertyType.IsSubclassOf(typeof(CompressionSettings)))                
                    .Select(m => (m.GetValue(this) as CompressionSettings));         

        }
    }
}
