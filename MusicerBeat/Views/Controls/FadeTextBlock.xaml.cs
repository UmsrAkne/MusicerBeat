using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace MusicerBeat.Views.Controls
{
    public partial class FadeTextBlock
    {
        // ReSharper disable once ArrangeModifiersOrder
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                nameof(Text),
                typeof(string),
                typeof(FadeTextBlock),
                new PropertyMetadata(string.Empty, OnTextChanged));

        public FadeTextBlock()
        {
            InitializeComponent();
        }

        public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (FadeTextBlock)d;
            control.StartFadeAnimation((string)e.NewValue);
        }

        private void StartFadeAnimation(string newText)
        {
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200));
            fadeOut.Completed += (_, _) =>
            {
                InnerTextBlock.Text = newText;
                var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));
                InnerTextBlock.BeginAnimation(OpacityProperty, fadeIn);
            };

            InnerTextBlock.BeginAnimation(OpacityProperty, fadeOut);
        }
    }
}