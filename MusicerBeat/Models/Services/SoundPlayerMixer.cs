using System;
using System.Collections.Generic;
using System.Linq;
using MusicerBeat.Models.Interfaces;

namespace MusicerBeat.Models.Services
{
    public class SoundPlayerMixer
    {
        public SoundPlayerMixer(ISoundPlayerFactory soundPlayerFactory)
        {
            SoundPlayerFactory = soundPlayerFactory;
        }

        private List<ISoundPlayer> SoundPlayers { get; set; }

        private ISoundPlayerFactory SoundPlayerFactory { get; }

        public PlayingStatus GetStatus()
        {
            if (SoundPlayers.Count == 0)
            {
                return PlayingStatus.Stopped;
            }

            if (SoundPlayers.Count == 1)
            {
                if (!SoundPlayers.First().IsPlaying)
                {
                    throw new InvalidOperationException("Invalid Status");
                }

                return PlayingStatus.Playing;
            }

            if (SoundPlayers.Count == 2)
            {
                if (SoundPlayers.All(p => p.IsPlaying))
                {
                    // リストの中のプレイヤーが両方動いている。
                    return PlayingStatus.Fading;
                }
            }

            throw new InvalidOperationException("Invalid Status");
        }

        private void Play(SoundFile soundFile)
        {
            var newPlayer = SoundPlayerFactory.CreateSoundPlayer();
            newPlayer.SoundEnded += RemoveAndPlay;
            SoundPlayers.Add(newPlayer);
            newPlayer.PlaySound(soundFile);

            newPlayer.Volume = GetStatus() switch
            {
                PlayingStatus.Playing => 1.0f,
                PlayingStatus.Fading => 0f,
                _ => newPlayer.Volume,
            };
        }

        private void RemoveAndPlay(object sender, EventArgs e)
        {
            if (sender is ISoundPlayer p)
            {
                SoundPlayers.Remove(p);
            }

            if (GetStatus() == PlayingStatus.Stopped)
            {
                Play(null);
            }
        }
    }
}