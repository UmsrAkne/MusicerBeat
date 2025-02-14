using System.Diagnostics;
using Prism.Mvvm;

namespace MusicerBeat.Models
{
    public class TextWrapper : BindableBase
    {
        private string title;
        private string version = string.Empty;

        public TextWrapper()
        {
            Title = "MusicerBeat";

            SetVersion();
            AddDebugMark();
        }

        public string Title
        {
            get => string.IsNullOrWhiteSpace(Version)
                ? title
                : title + " version : " + Version;
            private set => SetProperty(ref title, value);
        }

        private string Version { get => version; set => SetProperty(ref version, value); }

        [Conditional("RELEASE")]
        private void SetVersion()
        {
            const int major = 0;
            const int minor = 3;
            const int patch = 0;
            const string date = "20250214";
            const string suffixId = "a";

            Version = $"{major}.{minor}.{patch} ({date}{suffixId})";
        }

        [Conditional("DEBUG")]
        private void AddDebugMark()
        {
            Title += " (Debug)";
        }
    }
}