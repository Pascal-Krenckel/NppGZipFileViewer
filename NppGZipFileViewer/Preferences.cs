using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace NppGZipFileViewer
{
    [Serializable]
    public class Preferences
    {
        public bool DecompressAll { get; set; }

        //public bool OpenAsUTF8 { get; set; }
        public List<string> Extensions { get; set; }


        public Preferences() : this(false) { }
        public Preferences(bool decompressAll,IEnumerable<string> exts)
        {
            Extensions = exts.ToList();
            DecompressAll = decompressAll;
            //OpenAsUTF8 = openAsUTF8;
        }
        public Preferences(bool decompressAll,params string[] exts) : this(decompressAll, (IEnumerable<string>)exts)
        {
        }



        public bool HasGZipSuffix(string path)
        {
            return Extensions.Any(suffix => path?.EndsWith(suffix) ?? false);
        }
        public bool HasGZipSuffix(StringBuilder path)
        {
            return HasGZipSuffix(path.ToString());
        }

        public void Serialize(string path)
        {
            using Stream streams = new FileStream(path, FileMode.Create, FileAccess.Write);
            Serialize(streams);
        }
        public void Serialize(Stream to)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Preferences));
            serializer.Serialize(to, this);
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
            pref.Extensions = pref.Extensions.Distinct().ToList();
            return pref;
        }


    }
}
