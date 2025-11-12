using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MusicerBeat.Views.Controls
{
    public partial class PlaybackIndicator
    {
        private bool isLoaded; // 初回描画判定用

        public PlaybackIndicator()
        {
            InitializeComponent();
            Loaded += (_, _) =>
            {
                isLoaded = true;

                // 初期描画はアニメーションなしで反映
                UpdateVisualState(IsPlaying, false);
            };
        }

        public static readonly DependencyProperty IsPlayingProperty =
            DependencyProperty.Register(nameof(IsPlaying), typeof(bool), typeof(PlaybackIndicator),
                new PropertyMetadata(false, OnIsPlayingChanged));

        public bool IsPlaying { get => (bool)GetValue(IsPlayingProperty); set => SetValue(IsPlayingProperty, value); }

        private static void OnIsPlayingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PlaybackIndicator indicator)
            {
                var newValue = (bool)e.NewValue;
                indicator.UpdateVisualState(newValue, indicator.isLoaded);
            }
        }

        private void UpdateVisualState(bool isPlaying, bool useAnimation)
        {
            var backgroundBrush = IndicatorBorder.Background as SolidColorBrush;
            var borderBrush = IndicatorBorder.BorderBrush as SolidColorBrush;

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

            if (!useAnimation)
            {
                var animation = (Storyboard)FindResource("PlayAnimation");

                // アニメーションなしで即時適用
                backgroundBrush.BeginAnimation(SolidColorBrush.ColorProperty, null);
                borderBrush.BeginAnimation(SolidColorBrush.ColorProperty, null);

                if (isPlaying)
                {
                    backgroundBrush.Color = Colors.LightGreen;
                    borderBrush.Color = Colors.MediumSpringGreen;
                    Storyboard.SetTarget(animation, IndicatorBorder);
                    animation.Begin();
                }
                else
                {
                    animation.Stop();
                    backgroundBrush.Color = Colors.Transparent;
                    borderBrush.Color = Colors.DimGray;
                }

                return;
            }

            if (isPlaying)
            {
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