using System.Windows;
using MusicerBeat.Models;
using MusicerBeat.Models.Databases;
using MusicerBeat.ViewModels;
using MusicerBeat.Views;
using Prism.Ioc;

namespace MusicerBeat
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private MainWindowViewModel mainWindowVm;

        protected override Window CreateShell()
        {
            var window = Container.Resolve<MainWindow>();
            mainWindowVm = window.DataContext as MainWindowViewModel;
            return window;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IRepository<SoundFile>, Repository<SoundFile>>();
            containerRegistry.Register<IRepository<ListenHistory>, Repository<ListenHistory>>();

            containerRegistry.RegisterSingleton<DatabaseContext>();
            containerRegistry.RegisterSingleton<SoundFileService>();

            containerRegistry.RegisterDialog<SettingPage, SettingPageViewModel>();
            containerRegistry.RegisterDialog<HistoryPage, HistoryPageViewModel>();

            var d = Container.Resolve<DatabaseContext>();
            d.Database.EnsureCreated();
        }

        /// <summary>
        /// アプリケーションが終了する時のイベント
        /// </summary>
        /// <param name="e">入力される EventArgs</param>
        protected override void OnExit(ExitEventArgs e)
        {
            mainWindowVm?.SaveVolume();
        }
    }
}