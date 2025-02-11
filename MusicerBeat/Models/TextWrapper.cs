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

        private string Title
        {
            get => string.IsNullOrWhiteSpace(Version)
                ? title
                : title + " version : " + Version;
            set => SetProperty(ref title, value);
        }

        private string Version { get => version; set => SetProperty(ref version, value); }

        [Conditional("RELEASE")]
        private void SetVersion()
        {
            Version = "20250211" + "a";
        }

        [Conditional("DEBUG")]
        private void AddDebugMark()
        {
            Title += " (Debug)";
        }
    }
}