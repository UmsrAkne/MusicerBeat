using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task AddSoundFileCollectionAsync(IEnumerable<SoundFile> soundFiles)
        {
            var all = await GetSoundFilesAsync();
            var dic = all.ToDictionary(s => s.FullName, s => s);
            var filtered = soundFiles.Where(s => !dic.ContainsKey(s.FullName));
            await soundFileRepository.AddRangeAsync(filtered);
        }

        /// <summary>
        /// サウンドの視聴履歴をデータベースに記録します。
        /// </summary>
        /// <remarks>
        /// 内部でサウンドファイルの視聴回数のインクリメントして記録。<br/>
        /// 同時に新しい `ListenHistory` を現在の時刻とともに記録します。
        /// </remarks>
        /// <param name="soundFile">記録に使用するサウンドファイルを入力します。</param>
        /// <returns>非同期処理を表すタスク</returns>
        public Task AddListenHistoryAsync(SoundFile soundFile)
        {
            soundFile.ListenCount++;
            soundFileRepository.UpdateAsync(soundFile);

            return listenHistoryRepository.AddAsync(new ListenHistory()
            {
                SoundFileId = soundFile.Id,
                DateTime = DateTime.Now,
            });
        }
    }
}