using System;
using Prism.Mvvm;

namespace MusicerBeat.Models
{
    public class ApplicationSetting : BindableBase
    {
        private TimeSpan frontCut = TimeSpan.Zero;
        private TimeSpan backCut = TimeSpan.Zero;
        private TimeSpan crossFadeDuration = TimeSpan.Zero;
        private string rootDirectoryPath = string.Empty;

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
    }
}