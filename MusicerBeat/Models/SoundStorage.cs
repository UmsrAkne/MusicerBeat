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
                    RaisePropertyChanged(nameof(Name));
                }
            }
        }

        public string Name
        {
            get => Path.GetFileName(FullPath);
            private set => SetProperty(ref name, value);
        }

        public IEnumerable<SoundStorage> GetChildren()
        {
            return Directory.GetDirectories(FullPath).Select(d => new SoundStorage() { FullPath = d, });
        }

        public IEnumerable<SoundFile> GetFiles()
        {
            var targetExtensions = new[] { ".ogg", ".wav", ".mp3", };
            return Directory.GetFiles(FullPath)
                .Where(f => targetExtensions.Contains(Path.GetExtension(f).ToLower()))
                .Select(d => new SoundFile(d))
                .OrderBy(f => f.Name);
        }
    }
}