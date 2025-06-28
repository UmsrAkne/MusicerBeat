using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using Microsoft.Xaml.Behaviors; // Install-Package Microsoft.Xaml.Behaviors.Wpf

namespace MusicerBeat.Behaviors
{
    public class SmoothProgressBehavior : Behavior<ProgressBar>
    {
        public readonly static DependencyProperty TargetValueProperty = DependencyProperty.Register(
            nameof(TargetValue),
            typeof(double),
            typeof(SmoothProgressBehavior),
            new PropertyMetadata(0.0, OnTargetValueChanged));

        private double lastValue;

        /// <summary>
        /// The value the bar should animate toward (typically bound to the current playback position).
        /// </summary>
        public double TargetValue
        {
            get => (double)GetValue(TargetValueProperty);
            set => SetValue(TargetValueProperty, value);
        }

        private static void OnTargetValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SmoothProgressBehavior { AssociatedObject: not null, } behavior)
            {
                behavior.AnimateTo((double)e.NewValue);
            }
        }

        public readonly static DependencyProperty AnimationDurationProperty = DependencyProperty.Register(
            nameof(AnimationDuration),
            typeof(Duration),
            typeof(SmoothProgressBehavior),
            new PropertyMetadata(new Duration(TimeSpan.FromMilliseconds(200))));

        /// <summary>
        /// Duration of the easing animation. Default is 200ms
        /// </summary>
        public Duration AnimationDuration
        {
            get => (Duration)GetValue(AnimationDurationProperty);
            set => SetValue(AnimationDurationProperty, value);
        }

        protected override void OnDetaching()
        {
            // Ensure no dangling animations.
            AssociatedObject?.BeginAnimation(RangeBase.ValueProperty, null);
            base.OnDetaching();
        }

        private void AnimateTo(double newValue)
        {
            // Cancel any in‑flight animation so we always start from the current visual value.
            AssociatedObject.BeginAnimation(RangeBase.ValueProperty, null);

            var animation = new DoubleAnimation
            {
                From = lastValue,
                To = newValue,
                Duration = AnimationDuration,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut, },
                FillBehavior = FillBehavior.HoldEnd,
            };

            lastValue = TargetValue;
            AssociatedObject.BeginAnimation(RangeBase.ValueProperty, animation);
        }
    }
}