using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using MusicerBeat.Models.Databases;
using NAudio.Wave;
using NVorbis;
using Prism.Mvvm;

namespace MusicerBeat.Models
{
    public class SoundFile : BindableBase, IEntity
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

        // ReSharper disable once UnusedMember.Global
        // EntityFramework で利用するため、デフォルトコンストラクタが必要。
        public SoundFile()
        {
        }

        public int Id { get; set; }

        public string FullName { get => fullName; set => SetProperty(ref fullName, value); }

        [NotMapped]
        public string Name { get => name; set => SetProperty(ref name, value); }

        [NotMapped]
        public string NameWithoutExtension
        {
            get => nameWithoutExtension;
            set => SetProperty(ref nameWithoutExtension, value);
        }

        [NotMapped]
        public string Extension { get; set; }

        public int TotalSeconds { get => totalSeconds; set => SetProperty(ref totalSeconds, value); }

        public int ListenCount { get => listenCount; set => SetProperty(ref listenCount, value); }

        public bool IsSkip { get => isSkip; set => SetProperty(ref isSkip, value); }

        [NotMapped]
        public bool Playing { get => playing; set => SetProperty(ref playing, value); }

        [NotMapped]
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