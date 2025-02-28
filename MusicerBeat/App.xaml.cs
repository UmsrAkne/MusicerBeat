using System.Windows;
using MusicerBeat.Models.Databases;
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
            containerRegistry.RegisterSingleton<DatabaseContext>();

            var d = Container.Resolve<DatabaseContext>();
            d.Database.EnsureCreated();
        }
    }
}