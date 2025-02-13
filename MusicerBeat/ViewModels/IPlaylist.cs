using System.Collections.ObjectModel;
using MusicerBeat.Models;

namespace MusicerBeat.ViewModels
{
    /// <summary>
    /// 再生可能なサウンドファイルのリストを取得できるクラスに実装されるインターフェースです。
    /// </summary>
    public interface IPlaylist
    {
        public ReadOnlyObservableCollection<SoundFile> Sounds { get; set; }
    }
}