using System;
using System.ComponentModel.DataAnnotations.Schema;
using MusicerBeat.Models.Databases;

namespace MusicerBeat.Models
{
    public class ListenHistory : IEntity
    {
        public int Id { get; set; }

        public int SoundFileId { get; set; }

        public DateTime DateTime { get; set; }

        [NotMapped]
        public string SoundFileName { get; set; } = string.Empty;

        [NotMapped]
        public string DirectoryName { get; set; } = string.Empty;
    }
}