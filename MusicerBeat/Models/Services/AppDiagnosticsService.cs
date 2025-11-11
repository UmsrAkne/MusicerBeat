namespace MusicerBeat.Models.Services
{
    /// <summary>
    /// アプリの診断用情報を保持するサービス。
    /// アプリのどこからでも簡便に参照できるよう static なプロパティで公開します。
    /// </summary>
    public static class AppDiagnosticsService
    {
        /// <summary>
        /// 実際に使用されているデータベースファイルのフルパス。
        /// </summary>
        public static string DatabaseFullPath { get; set; }

        /// <summary>
        /// 表示用に整形したデータベースパスの説明文を返します。
        /// </summary>
        /// <returns>
        /// データベースのパスが記録されていればパスを含むアナウンスを返し、そうでなければ (未初期化 / 未作成) と返します。
        /// </returns>
        public static string GetDatabasePathDisplayText()
        {
            var path = string.IsNullOrWhiteSpace(DatabaseFullPath)
                ? "(未初期化 / 未作成)"
                : DatabaseFullPath;
            return $"Database file path:\n{path}";
        }
    }
}