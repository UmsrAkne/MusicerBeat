using System.Collections.Generic;
using System.Threading.Tasks;

namespace MusicerBeat.Models.Databases
{
    public class SoundFileService
    {
        private readonly IRepository<SoundFile> soundFileRepository;
        private readonly IRepository<ListenHistory> listenHistoryRepository;

        public SoundFileService(IRepository<SoundFile> soundFileRepository, IRepository<ListenHistory> listenHistoryRepository)
        {
            this.soundFileRepository = soundFileRepository;
            this.listenHistoryRepository = listenHistoryRepository;
        }

        public Task<IEnumerable<SoundFile>> GetSoundFilesAsync()
        {
            return soundFileRepository.GetAllAsync();
        }

        public Task AddSoundFileAsync(SoundFile soundFile)
        {
            return soundFileRepository.AddAsync(soundFile);
        }

        public Task AddListenHistoryAsync(ListenHistory listenHistory)
        {
            return listenHistoryRepository.AddAsync(listenHistory);
        }
    }
}