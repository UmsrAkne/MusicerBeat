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
        /// サウンドを再生中で、次に鳴らすサウンドが待機している状態です。
        /// </summary>
        PlayingAndWaiting,

        /// <summary>
        /// サウンドの再生を停止している状態です。
        /// </summary>
        Stopped,
    }
}