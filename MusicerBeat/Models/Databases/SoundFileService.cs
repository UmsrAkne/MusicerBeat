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

            var enumerable = filtered as SoundFile[] ?? filtered.ToArray();
            foreach (var s in enumerable)
            {
                s.LoadDuration();
            }

            await soundFileRepository.AddRangeAsync(enumerable);
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
        public async Task AddListenHistoryAsync(SoundFile soundFile)
        {
            var record = soundFile;

            if (soundFile.Id == 0)
            {
                var all = await soundFileRepository.GetAllAsync();
                var ss = all.FirstOrDefault(s => s.FullName == soundFile.FullName);
                record = ss ?? record;
            }

            record.ListenCount++;
            soundFile.ListenCount = record.ListenCount;
            await soundFileRepository.UpdateAsync(record);

            await listenHistoryRepository.AddAsync(new ListenHistory()
            {
                SoundFileId = record.Id,
                DateTime = DateTime.Now,
            });
        }

        public async Task<List<(int ListenCount, int TotalSec)>> LoadListenCount(IEnumerable<SoundFile> soundFiles)
        {
            var all = await GetSoundFilesAsync();
            var dic = all.ToDictionary(s => s.FullName, s => s);

            var results = new List<(int, int)>();

            foreach (var soundFile in soundFiles)
            {
                if (dic.TryGetValue(soundFile.FullName, out var s))
                {
                    results.Add((s.ListenCount, s.TotalMilliSeconds));
                }
            }

            return results;
        }

        public async Task LoadSoundInfo(IEnumerable<SoundFile> soundFiles)
        {
            var all = await GetSoundFilesAsync();
            var dic = all.ToDictionary(s => s.FullName, s => s);

            foreach (var soundFile in soundFiles)
            {
                if (!dic.TryGetValue(soundFile.FullName, out var s))
                {
                    continue;
                }

                soundFile.ListenCount = s.ListenCount;
                soundFile.TotalMilliSeconds = s.TotalMilliSeconds;
            }
        }

        public async Task ToggleSkip(SoundFile soundFile)
        {
            var all = await soundFileRepository.GetAllAsync();
            var s = all.FirstOrDefault(sound => sound.FullName == soundFile.FullName);
            if (s != null)
            {
                s.IsSkip = soundFile.IsSkip;
            }

            await soundFileRepository.UpdateAsync(s);
        }
    }
}