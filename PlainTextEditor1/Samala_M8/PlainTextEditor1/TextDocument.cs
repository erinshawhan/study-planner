using System;
using System.IO;

namespace PlainTextEditor1
{
    public class TextDocument
    {
        public string FilePath { get; private set; }
        public string Content { get; set; }
        public bool IsDirty { get; set; }

        public TextDocument()
        {
            Content = string.Empty;
            FilePath = null;
            IsDirty = false;
        }

        public void Open(string path)
        {
            Content = File.ReadAllText(path);
            FilePath = path;
            IsDirty = false;
        }

        public void Save()
        {
            if (!string.IsNullOrEmpty(FilePath))
            {
                File.WriteAllText(FilePath, Content);
                IsDirty = false;
            }
        }

        public void SaveAs(string path)
        {
            File.WriteAllText(path, Content);
            FilePath = path;
            IsDirty = false;
        }
    }
}
