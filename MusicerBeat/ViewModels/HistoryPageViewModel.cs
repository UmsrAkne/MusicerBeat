using System;
using System.Collections.ObjectModel;
using MusicerBeat.Models;
using MusicerBeat.Models.Commands;
using MusicerBeat.Models.Databases;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace MusicerBeat.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class HistoryPageViewModel : BindableBase, IDialogAware
    {
        private readonly int pageSize = 50;
        private SoundFileService soundFileService;
        private int pageNumber = 1;

        public event Action<IDialogResult> RequestClose;

        public string Title => string.Empty;

        public ObservableCollection<ListenHistory> ListenHistories { get; } = new ();

        public int PageNumber { get => pageNumber; set => SetProperty(ref pageNumber, value); }

        public DelegateCommand<object> MovePageCommand => new ((pagingAction) =>
        {
            var pa = (PagingAction)pagingAction;
            var oldPageNumber = PageNumber;
            switch (pa)
            {
                case PagingAction.Next:
                    if (ListenHistories.Count < pageSize)
                    {
                        return;
                    }

                    PageNumber++;
                    break;
                case PagingAction.Prev:
                    if (PageNumber <= 1)
                    {
                        return;
                    }

                    PageNumber--;
                    break;
                case PagingAction.First:
                    PageNumber = 1;
                    break;
            }

            if (oldPageNumber == PageNumber)
            {
                return;
            }

            ReloadHistoriesAsyncCommand.Execute(null);
        });

        public DelegateCommand CloseCommand => new DelegateCommand(() =>
        {
            RequestClose?.Invoke(new DialogResult());
        });

        private AsyncDelegateCommand ReloadHistoriesAsyncCommand => new (async () =>
        {
            var l = await soundFileService.GetPagedListenHistoriesAsync(pageNumber, pageSize);
            ListenHistories.Clear();
            ListenHistories.AddRange(l);
        });

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            soundFileService = parameters.GetValue<SoundFileService>(nameof(SoundFileService));
            ReloadHistoriesAsyncCommand.Execute(null);
        }
    }
}