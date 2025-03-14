using System;
using MusicerBeat.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace MusicerBeat.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class SettingPageViewModel : BindableBase, IDialogAware
    {
        private ApplicationSetting applicationSetting = new ();

        public event Action<IDialogResult> RequestClose;

        public string Title => "Setting Page";

        public ApplicationSetting ApplicationSetting
        {
            get => applicationSetting;
            private set => SetProperty(ref applicationSetting, value);
        }

        public DelegateCommand CloseCommand => new (() =>
        {
            RequestClose?.Invoke(new DialogResult());
        });

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
            ApplicationSetting.SaveToXml(ApplicationSetting.SettingFileName);
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            ApplicationSetting = ApplicationSetting.LoadFromXml(ApplicationSetting.SettingFileName);
        }
    }
}