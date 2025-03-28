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
        private IEnumerable<SoundFile> soundFilesCache;
        private bool isSelected;
        private bool isExpanded;
        private List<SoundStorage> children;
        private bool isEmpty = true;

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

                if (IsM3U)
                {
                    IsEmpty = false;
                    return;
                }

                var dirInfo = new DirectoryInfo(value);
                if (dirInfo.Exists)
                {
                    IsEmpty = !dirInfo.GetDirectories().Any() && !dirInfo.GetFiles().Any();
                }
            }
        }

        public string Name
        {
            get => Path.GetFileName(FullPath);
            private set => SetProperty(ref name, value);
        }

        public bool IsSelected { get => isSelected; set => SetProperty(ref isSelected, value); }

        public bool IsExpanded { get => isExpanded; set => SetProperty(ref isExpanded, value); }

        public List<SoundStorage> Children { get => children; set => SetProperty(ref children, value); }

        public bool IsEmpty { get => isEmpty; private set => SetProperty(ref isEmpty, value); }

        private bool IsM3U { get; set; }

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
            if (soundFilesCache != null)
            {
                return soundFilesCache;
            }

            if (IsM3U)
            {
                soundFilesCache = ParseM3U(File.ReadAllText(FullPath)).ToList();
                return soundFilesCache;
            }

            soundFilesCache = Directory.GetFiles(FullPath)
                .Where(SoundFile.IsSoundFile)
                .Select(d =>
                {
                    var sf = new SoundFile(d);
                    return sf;
                })
                .OrderBy(f => f.Name)
                .ToList();

            return soundFilesCache;
        }

        public List<SoundFile> ParseM3U(string text)
        {
            string[] newLineStrings = { "\r\n", "\n", "\r", };

            return text.Split(newLineStrings, StringSplitOptions.RemoveEmptyEntries)
                .Where(l => !l.TrimStart().StartsWith("#"))
                .Select(l => new SoundFile(l.Split('#').First().Trim()))
                .ToList();
        }
    }
}