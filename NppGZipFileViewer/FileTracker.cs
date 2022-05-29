using System;
using System.Collections.Generic;
using System.Text;

namespace NppGZipFileViewer
{
    public class FileTracker
    {
        HashSet<IntPtr> zippedFiles = new HashSet<IntPtr>();
        Dictionary<IntPtr, string> filePathes = new Dictionary<IntPtr, string>();
        Dictionary<IntPtr, Encoding> encodings = new Dictionary<IntPtr, Encoding>();

        HashSet<IntPtr> excludedFiles = new HashSet<IntPtr> ();
        public void Include(IntPtr id, StringBuilder path, Encoding encoding)
        {
            Include(id, path.ToString(),encoding);
        }

        public void Exclude(IntPtr id, StringBuilder path)
        {
            Exclude(id, path.ToString());
        }

        public void Include(IntPtr id, string path, Encoding encoding)
        {
            excludedFiles.Remove(id);
            zippedFiles.Add(id);
            if (encodings.ContainsKey(id))
                encodings[id] = encoding;
            else encodings.Add(id, encoding);
            if (!filePathes.ContainsKey(id))
                filePathes.Add(id, path);
            else filePathes[id] = path;
        }
        public void Exclude(IntPtr id, string path)
        {
            zippedFiles.Remove(id);
            encodings.Remove(id);
            excludedFiles.Add(id);
            if (!filePathes.ContainsKey(id))
                filePathes.Add(id, path);
            else filePathes[id] = path;
        }


        public void Remove(IntPtr id)
        {
            zippedFiles.Remove(id);
            excludedFiles.Remove(id);
            filePathes.Remove(id);
            encodings.Remove(id);
        }

        public bool IsIncluded(IntPtr id) { return zippedFiles.Contains(id); }

        public bool IsExcluded(IntPtr id) { return excludedFiles.Contains(id); }

        public string GetStoredPath(IntPtr id) { filePathes.TryGetValue(id, out string path); return path; }

        public Encoding GetEncoding(IntPtr id)
        {
            if (encodings.TryGetValue(id, out Encoding encoding)) return encoding;
            else return new UTF8Encoding(false);
        }

    }
}
