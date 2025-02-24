namespace MusicerBeat.Models
{
    public enum PlayingStatus
    {
        /// <summary>
        /// 複数のサウンドが再生状態でクロスフェードしている状態です。
        /// </summary>
        Fading,

        /// <summary>
        /// 単一のサウンドを再生していて、待機状態のサウンドが存在しない状態です。
        /// </summary>
        Playing,

        /// <summary>
        /// サウンドの再生を停止している状態です。
        /// </summary>
        Stopped,
    }
}