using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MusicerBeat.Views.Controls
{
    public partial class CustomToggleButton : UserControl
    {
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            nameof(Content),
            typeof(object),
            typeof(CustomToggleButton),
            new PropertyMetadata(default(object)));

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            nameof(Command),
            typeof(ICommand),
            typeof(CustomToggleButton),
            new PropertyMetadata(default(ICommand)));

        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(
            nameof(IsChecked),
            typeof(bool),
            typeof(CustomToggleButton),
            new FrameworkPropertyMetadata(
                false,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public CustomToggleButton()
        {
            InitializeComponent();
        }

        public object Content { get => (object)GetValue(ContentProperty); set => SetValue(ContentProperty, value); }

        public ICommand Command { get => (ICommand)GetValue(CommandProperty); set => SetValue(CommandProperty, value); }

        public bool IsChecked { get => (bool)GetValue(IsCheckedProperty); set => SetValue(IsCheckedProperty, value); }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            IsChecked = !IsChecked;
        }
    }
}