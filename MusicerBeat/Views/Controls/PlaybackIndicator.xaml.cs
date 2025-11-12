using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MusicerBeat.Views.Controls
{
    public partial class PlaybackIndicator
    {
        public PlaybackIndicator()
        {
            InitializeComponent();
            Loaded += (_, _) => UpdateVisualState(IsPlaying);
        }

        public static readonly DependencyProperty IsPlayingProperty =
            DependencyProperty.Register(nameof(IsPlaying), typeof(bool), typeof(PlaybackIndicator),
                new PropertyMetadata(false, OnIsPlayingChanged));

        public bool IsPlaying { get => (bool)GetValue(IsPlayingProperty); set => SetValue(IsPlayingProperty, value); }

        private static void OnIsPlayingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PlaybackIndicator indicator)
            {
                indicator.UpdateVisualState((bool)e.NewValue);
            }
        }

        private void UpdateVisualState(bool isPlaying)
        {
            var backgroundBrush = IndicatorBorder.Background as SolidColorBrush;
            var borderBrush = IndicatorBorder.BorderBrush as SolidColorBrush;

            if (isPlaying)
            {
                // SolidColorBrush の初期化（null や IsFrozen に備える）
                if (backgroundBrush == null || backgroundBrush.IsFrozen)
                {
                    backgroundBrush = new SolidColorBrush(Colors.Transparent);
                    IndicatorBorder.Background = backgroundBrush;
                }

                if (borderBrush == null || borderBrush.IsFrozen)
                {
                    borderBrush = new SolidColorBrush(Colors.DimGray);
                    IndicatorBorder.BorderBrush = borderBrush;
                }

                // フェードイン（背景）
                var fadeInBackground = new ColorAnimation
                {
                    To = Colors.LightGreen,
                    Duration = TimeSpan.FromSeconds(0.5),
                    FillBehavior = FillBehavior.HoldEnd,
                };

                // フェードイン（枠線）
                var fadeInBorder = new ColorAnimation
                {
                    To = Colors.MediumSpringGreen,
                    Duration = TimeSpan.FromSeconds(0.5),
                    FillBehavior = FillBehavior.HoldEnd,
                };

                // 背景フェードイン → 完了後にループアニメーション開始
                fadeInBackground.Completed += (_, _) =>
                {
                    var animation = (Storyboard)FindResource("PlayAnimation");
                    Storyboard.SetTarget(animation, IndicatorBorder);
                    animation.Begin();
                };

                backgroundBrush.BeginAnimation(SolidColorBrush.ColorProperty, fadeInBackground);
                borderBrush.BeginAnimation(SolidColorBrush.ColorProperty, fadeInBorder);
            }
            else
            {
                var animation = (Storyboard)FindResource("PlayAnimation");
                animation.Stop();

                if (backgroundBrush == null || backgroundBrush.IsFrozen)
                {
                    backgroundBrush = new SolidColorBrush(Colors.LightGreen);
                    IndicatorBorder.Background = backgroundBrush;
                }

                if (borderBrush == null || borderBrush.IsFrozen)
                {
                    borderBrush = new SolidColorBrush(Colors.MediumSpringGreen);
                    IndicatorBorder.BorderBrush = borderBrush;
                }

                // フェードアウト（背景）
                var fadeOutBackground = new ColorAnimation
                {
                    To = Colors.Transparent,
                    Duration = TimeSpan.FromSeconds(0.5),
                    FillBehavior = FillBehavior.HoldEnd,
                };

                // フェードアウト（枠線）
                var fadeOutBorder = new ColorAnimation
                {
                    To = Colors.DimGray,
                    Duration = TimeSpan.FromSeconds(0.5),
                    FillBehavior = FillBehavior.HoldEnd,
                };

                backgroundBrush.BeginAnimation(SolidColorBrush.ColorProperty, fadeOutBackground);
                borderBrush.BeginAnimation(SolidColorBrush.ColorProperty, fadeOutBorder);
            }
        }
    }
}