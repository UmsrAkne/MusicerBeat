using System;
using System.Collections.Generic;
using MusicerBeat.Models;

namespace MusicerBeat.ViewModels
{
    public interface ISoundCollectionSource
    {
        /// <summary>
        /// サウンドリストが更新されたときに発生するイベント。
        /// </summary>
        event EventHandler SoundsSourceUpdated;

        /// <summary>
        /// 現在のサウンドリストを取得する。
        /// </summary>
        /// <returns>
        /// 現在のサウンドリスト。
        /// </returns>
        IEnumerable<SoundFile> GetSounds();
    }
}