using System;
using System.Collections.Generic;
using System.Text;

namespace NppGZipFileViewer
{
    public class FileTracker
    {
        HashSet<IntPtr> zippedFiles = new HashSet<IntPtr>();
        Dictionary<IntPtr, string> filePathes = new Dictionary<IntPtr, string>();

        HashSet<IntPtr> excludedFiles = new HashSet<IntPtr> ();
        public void Include(IntPtr id, StringBuilder path)
        {
            Include(id, path.ToString());
        }

        public void Exclude(IntPtr id, StringBuilder path)
        {
            Exclude(id, path.ToString());
        }

        public void Include(IntPtr id, string path)
        {
            excludedFiles.Remove(id);
            zippedFiles.Add(id);
            if (filePathes.ContainsKey(id))
                filePathes.Add(id, path);
            else filePathes[id] = path;
        }
        public void Exclude(IntPtr id, string path)
        {
            zippedFiles.Remove(id);

            excludedFiles.Add(id);
            if (filePathes.ContainsKey(id))
                filePathes.Add(id, path);
            else filePathes[id] = path;
        }


        public void Remove(IntPtr id)
        {
            zippedFiles.Remove(id);
            excludedFiles.Remove(id);
            filePathes.Remove(id);
        }

        public bool IsIncluded(IntPtr id) { return zippedFiles.Contains(id); }

        public bool IsExcluded(IntPtr id) { return excludedFiles.Contains(id); }

        public string GetStoredPath(IntPtr id) { filePathes.TryGetValue(id, out string path); return path; }


    }
}
