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
        // ReSharper disable once ArrangeModifiersOrder
        public static readonly DependencyProperty TargetValueProperty = DependencyProperty.Register(
            nameof(TargetValue),
            typeof(double),
            typeof(SmoothProgressBehavior),
            new PropertyMetadata(0.0, OnTargetValueChanged));

        // ReSharper disable once ArrangeModifiersOrder
        public static readonly DependencyProperty AnimationDurationProperty = DependencyProperty.Register(
            nameof(AnimationDuration),
            typeof(Duration),
            typeof(SmoothProgressBehavior),
            new PropertyMetadata(new Duration(TimeSpan.FromMilliseconds(200))));

        private double lastValue;

        /// <summary>
        /// The value the bar should animate toward (typically bound to the current playback position).
        /// </summary>
        public double TargetValue
        {
            get => (double)GetValue(TargetValueProperty);
            set => SetValue(TargetValueProperty, value);
        }

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

        private static void OnTargetValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SmoothProgressBehavior { AssociatedObject: not null, } behavior)
            {
                behavior.AnimateTo((double)e.NewValue);
            }
        }

        private void AnimateTo(double newValue)
        {
            // 現在の見た目の値を保持してからアニメを解除
            var currentValue = AssociatedObject.Value;
            AssociatedObject.BeginAnimation(RangeBase.ValueProperty, null);

            var animation = new DoubleAnimation
            {
                From = currentValue,
                To = newValue,
                Duration = AnimationDuration,
                FillBehavior = FillBehavior.HoldEnd,
            };

            lastValue = newValue;
            AssociatedObject.BeginAnimation(RangeBase.ValueProperty, animation);
        }
    }
}