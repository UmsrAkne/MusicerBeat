using System;
using Prism.Mvvm;

namespace MusicerBeat.Models
{
    public class CrossFadeSetting : BindableBase
    {
        private TimeSpan duration = TimeSpan.Zero;
        private TimeSpan frontCut = TimeSpan.Zero;
        private TimeSpan backCut = TimeSpan.Zero;
        private bool enabled;

        public bool Enabled { get => enabled; set => SetProperty(ref enabled, value); }

        public TimeSpan Duration
        {
            get => duration;
            set
            {
                if (SetProperty(ref duration, value))
                {
                    RaisePropertyChanged(nameof(RequiredCrossFadeDuration));
                }
            }
        }

        public TimeSpan FrontCut
        {
            get => frontCut;
            set
            {
                if (SetProperty(ref frontCut, value))
                {
                    RaisePropertyChanged(nameof(RequiredCrossFadeDuration));
                }
            }
        }

        public TimeSpan BackCut
        {
            get => backCut;
            set
            {
                if (SetProperty(ref backCut, value))
                {
                    RaisePropertyChanged(nameof(RequiredCrossFadeDuration));
                }
            }
        }

        public TimeSpan RequiredCrossFadeDuration => (Duration * 2) + FrontCut + BackCut;

        /// <summary>
        /// クロスフェードの音量変更処理のインターバルです。
        /// </summary>
        public TimeSpan FadeProcessInterval => TimeSpan.FromMilliseconds(200);

        /// <summary>
        /// クロスフェード上昇側の音量の最大値です。
        /// </summary>
        public float MaxVolume { get; set; } = 1.0f;

        public float VolumeFadeStep
        {
            get
            {
                var numStepPerSec = Duration.TotalMilliseconds / FadeProcessInterval.TotalMilliseconds;
                return (float)(MaxVolume / numStepPerSec);
            }
        }
    }
}