using System;
using System.IO;
using System.Xml.Serialization;
using Prism.Mvvm;

namespace MusicerBeat.Models
{
    public class ApplicationSetting : BindableBase
    {
        public static readonly string SettingFileName = "setting.xml";

        private TimeSpan frontCut = TimeSpan.Zero;
        private TimeSpan backCut = TimeSpan.Zero;
        private TimeSpan crossFadeDuration = TimeSpan.Zero;
        private string rootDirectoryPath = string.Empty;
        private float volume = 1.0f;

        public TimeSpan FrontCut { get => frontCut; set => SetProperty(ref frontCut, value); }

        public TimeSpan BackCut { get => backCut; set => SetProperty(ref backCut, value); }

        public TimeSpan CrossFadeDuration
        {
            get => crossFadeDuration;
            set => SetProperty(ref crossFadeDuration, value);
        }

        public string RootDirectoryPath
        {
            get => rootDirectoryPath;
            set => SetProperty(ref rootDirectoryPath, value);
        }

        public float Volume { get => volume; set => SetProperty(ref volume, value); }

        public static ApplicationSetting LoadFromXml(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new ApplicationSetting();
            }

            var serializer = new XmlSerializer(typeof(ApplicationSetting));
            using var reader = new StreamReader(filePath);
            return (ApplicationSetting)serializer.Deserialize(reader);
        }

        public void SaveToXml(string filePath)
        {
            var serializer = new XmlSerializer(typeof(ApplicationSetting));
            using var writer = new StreamWriter(filePath);
            serializer.Serialize(writer, this);
        }
    }
}