using System;
using System.Collections.Generic;
using System.Text;

namespace NppGZipFileViewer
{
    public class FileTracker
    {
        HashSet<IntPtr> zippedFiles = new HashSet<IntPtr>();
        Dictionary<IntPtr, string> zippedFilePathes = new Dictionary<IntPtr, string>();

        HashSet<IntPtr> excludedFiles = new HashSet<IntPtr> ();
        public void Include(IntPtr id, StringBuilder path)
        {
            Include(id, path.ToString());
        }
        public void Include(IntPtr id, string path)
        {
            excludedFiles.Remove(id);
            zippedFiles.Add(id);
            if (zippedFilePathes.ContainsKey(id))
                zippedFilePathes.Add(id, path);
            else zippedFilePathes[id] = path;
        }

        public void Remove(IntPtr id) { zippedFiles.Remove(id); zippedFilePathes.Remove(id); }

        public void Exclude(IntPtr id)
        {
            if(zippedFiles.Remove(id)) zippedFilePathes.Remove(id);

            excludedFiles.Add(id);
        }

        public bool Contains(IntPtr id) { return zippedFiles.Contains(id); }

        public bool IsExcluded(IntPtr id) { return excludedFiles.Contains(id); }

        public string GetStoredPath(IntPtr id) { zippedFilePathes.TryGetValue(id, out string path); return path; }


    }
}
