using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
        private int totalMilliSeconds;
        private int listenCount;
        private bool isSkip;
        private bool playing;
        private string nameWithoutExtension;
        private int index;
        private string directoryName = string.Empty;

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
            DirectoryName = Path.GetDirectoryName(filePath);
            RelativePath = GetRelativePath(filePath, RootDirectoryPath);
        }

        // ReSharper disable once UnusedMember.Global
        // EntityFramework で利用するため、デフォルトコンストラクタが必要。
        public SoundFile()
        {
        }

        [NotMapped]
        public static string RootDirectoryPath { get; set; } = string.Empty;

        public int Id { get; set; }

        // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
        public string FullName { get => fullName; set => SetProperty(ref fullName, value); }

        public string RelativePath { get; set; }

        public string FileHash { get; set; }

        [NotMapped]
        public string Name { get => name; set => SetProperty(ref name, value); }

        [NotMapped]
        public string NameWithoutExtension
        {
            get => nameWithoutExtension;
            set => SetProperty(ref nameWithoutExtension, value);
        }

        [NotMapped]
        public string DirectoryName { get => directoryName; set => SetProperty(ref directoryName, value); }

        [NotMapped]
        public string Extension { get; set; }

        public int TotalMilliSeconds { get => totalMilliSeconds; set => SetProperty(ref totalMilliSeconds, value); }

        public int ListenCount { get => listenCount; set => SetProperty(ref listenCount, value); }

        public bool IsSkip { get => isSkip; set => SetProperty(ref isSkip, value); }

        [NotMapped]
        public bool Playing { get => playing; set => SetProperty(ref playing, value); }

        [NotMapped]
        public int Index { get => index; set => SetProperty(ref index, value); }

        public static string ComputeHash(string path)
        {
            using var stream = File.OpenRead(path);
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
        }

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
                TotalMilliSeconds = (int)vr.TotalTime.TotalMilliseconds;
                return;
            }

            using var afr = new AudioFileReader(FullName);
            var time = (int)afr.TotalTime.TotalMilliseconds;
            TotalMilliSeconds = time;
        }

        private static string GetRelativePath(string fullPath, string basePath)
        {
            if (!fullPath.StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("ファイルは指定されたルートパスの外にあります");
            }

            return Path.GetRelativePath(basePath, fullPath);
        }
    }
}