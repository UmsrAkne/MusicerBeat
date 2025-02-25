using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Prism.Mvvm;

namespace MusicerBeat.Models
{
    public class SoundStorage : BindableBase
    {
        private string fullPath;
        private string name;

        public string FullPath
        {
            get => fullPath;
            set
            {
                if (SetProperty(ref fullPath, value))
                {
                    IsM3U = Path.GetExtension(fullPath)?.ToLower() == ".m3u";
                    RaisePropertyChanged(nameof(Name));
                }
            }
        }

        public string Name
        {
            get => Path.GetFileName(FullPath);
            private set => SetProperty(ref name, value);
        }

        public bool IsM3U { get; set; }

        public IEnumerable<SoundStorage> GetChildren()
        {
            var list = new List<SoundStorage>();
            if (!Directory.Exists(FullPath))
            {
                return list;
            }

            list.AddRange(Directory.GetDirectories(FullPath).Select(d => new SoundStorage() { FullPath = d, }));
            list.AddRange(
                Directory.GetFiles(FullPath)
                    .Where(s => Path.GetExtension(s).ToLower() == ".m3u")
                    .Select(s => new SoundStorage() { FullPath = s, }));

            return list;
        }

        public IEnumerable<SoundFile> GetFiles()
        {
            if (IsM3U)
            {
                return ParseM3U(File.ReadAllText(FullPath));
            }

            return Directory.GetFiles(FullPath)
                .Where(SoundFile.IsSoundFile)
                .Select(d =>
                {
                    var sf = new SoundFile(d);
                    sf.LoadDuration();
                    return sf;
                })
                .OrderBy(f => f.Name);
        }

        public List<SoundFile> ParseM3U(string text)
        {
            string[] newLineStrings = { "\r\n", "\n", "\r", };

            return text.Split(newLineStrings, StringSplitOptions.RemoveEmptyEntries)
                .Where(l => !l.TrimStart().StartsWith("#"))
                .Select(l => new SoundFile(l))
                .ToList();
        }
    }
}