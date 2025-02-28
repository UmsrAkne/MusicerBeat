using System;
using MusicerBeat.Models.Databases;

namespace MusicerBeat.Models
{
    public class ListenHistory : IEntity
    {
        public int Id { get; set; }

        public int SoundFileId { get; set; }

        public DateTime DateTime { get; set; }
    }
}