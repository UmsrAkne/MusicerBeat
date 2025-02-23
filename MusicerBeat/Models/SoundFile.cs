using System;
using System.IO;
using System.Linq;
using NAudio.Wave;
using Prism.Mvvm;

namespace MusicerBeat.Models
{
    public class SoundFile : BindableBase
    {
        private string fullName;
        private string name;
        private int duration;
        private int listenCount;
        private bool isSkip;
        private bool playing;

        public SoundFile(string filePath)
        {
            if (!IsSoundFile(filePath))
            {
                throw new ArgumentException("入力されたファイルがサウンドファイルではありません");
            }

            FullName = filePath;
            Name = Path.GetFileName(filePath);
            Extension = Path.GetExtension(filePath).ToLower();
        }

        public string FullName { get => fullName; set => SetProperty(ref fullName, value); }

        public string Name { get => name; set => SetProperty(ref name, value); }

        public string Extension { get; set; }

        public int Duration { get => duration; set => SetProperty(ref duration, value); }

        public int ListenCount { get => listenCount; set => SetProperty(ref listenCount, value); }

        public bool IsSkip { get => isSkip; set => SetProperty(ref isSkip, value); }

        public bool Playing { get => playing; set => SetProperty(ref playing, value); }

        public static bool IsSoundFile(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();
            return new[] { ".wav", ".mp3", ".ogg", }.Contains(extension);
        }

        public void LoadDuration()
        {
            var time = (int)new Mp3FileReader(FullName).TotalTime.TotalSeconds;
            Duration = time;
        }
    }
}