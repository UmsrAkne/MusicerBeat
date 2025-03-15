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
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IRepository<SoundFile>, Repository<SoundFile>>();
            containerRegistry.Register<IRepository<ListenHistory>, Repository<ListenHistory>>();

            containerRegistry.RegisterSingleton<DatabaseContext>();
            containerRegistry.RegisterSingleton<SoundFileService>();

            containerRegistry.RegisterDialog<SettingPage, SettingPageViewModel>();

            var d = Container.Resolve<DatabaseContext>();
            d.Database.EnsureCreated();
        }
    }
}