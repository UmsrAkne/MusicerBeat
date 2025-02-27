using System;
using System.IO;
using System.Linq;
using NAudio.Wave;
using NVorbis;
using Prism.Mvvm;

namespace MusicerBeat.Models
{
    public class SoundFile : BindableBase
    {
        private string fullName;
        private string name;
        private int totalSeconds;
        private int listenCount;
        private bool isSkip;
        private bool playing;
        private string nameWithoutExtension;
        private int index;

        public SoundFile(string filePath)
        {
            if (!IsSoundFile(filePath))
            {
                throw new ArgumentException("入力されたファイルがサウンドファイルではありません");
            }

            FullName = filePath;
            Name = Path.GetFileName(filePath);
            NameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            Extension = Path.GetExtension(filePath).ToLower();
        }

        public string FullName { get => fullName; set => SetProperty(ref fullName, value); }

        public string Name { get => name; set => SetProperty(ref name, value); }

        public string NameWithoutExtension
        {
            get => nameWithoutExtension;
            set => SetProperty(ref nameWithoutExtension, value);
        }

        public string Extension { get; set; }

        public int TotalSeconds { get => totalSeconds; set => SetProperty(ref totalSeconds, value); }

        public int ListenCount { get => listenCount; set => SetProperty(ref listenCount, value); }

        public bool IsSkip { get => isSkip; set => SetProperty(ref isSkip, value); }

        public bool Playing { get => playing; set => SetProperty(ref playing, value); }

        public int Index { get => index; set => SetProperty(ref index, value); }

        public static bool IsSoundFile(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();
            return new[] { ".wav", ".mp3", ".ogg", }.Contains(extension);
        }

        public void LoadDuration()
        {
            if (Extension == ".ogg")
            {
                using var vr = new VorbisReader(FullName);
                TotalSeconds = (int)vr.TotalTime.TotalSeconds;
                return;
            }

            using var afr = new AudioFileReader(FullName);
            var time = (int)afr.TotalTime.TotalSeconds;
            TotalSeconds = time;
        }
    }
}