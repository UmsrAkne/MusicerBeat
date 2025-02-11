using System.Diagnostics;
using MusicerBeat.Models;
using Prism.Mvvm;

namespace MusicerBeat.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MainWindowViewModel : BindableBase
    {
        private DirectoryAreaViewModel directoryAreaViewModel = new ();

        public MainWindowViewModel()
        {
            SetDummies();
        }

        public TextWrapper Title { get; set; } = new ();

        public DirectoryAreaViewModel DirectoryAreaViewModel
        {
            get => directoryAreaViewModel;
            set => SetProperty(ref directoryAreaViewModel, value);
        }

        [Conditional("DEBUG")]
        private void SetDummies()
        {
            var firstStorage = new SoundStorage() { FullPath = $@"C:\test\testDirectory0", };
            DirectoryAreaViewModel.AddSoundStorage(firstStorage);

            for (var i = 0; i < 30; i++)
            {
                DirectoryAreaViewModel.AddSoundStorage(new SoundStorage() { FullPath = $@"C:\test\testDirectory{i + 1}", });
            }

            DirectoryAreaViewModel.SelectedItem = firstStorage;
        }
    }
}