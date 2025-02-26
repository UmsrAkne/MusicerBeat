using MusicerBeat.Models;

namespace MusicerBeatTests.Models
{
    [TestFixture]
    public class SoundStorageTests
    {
        private static IEnumerable<TestCaseData> ParseM3U_TestCases()
        {
            yield return new TestCaseData(
                @"C:\\t\a.mp3" + "\r\n" +
                @"C:\\t\b.mp3" + "\r\n" +
                @"C:\\t\c.mp3" + "\r\n",
                new[] { @"C:\\t\a.mp3", @"C:\\t\b.mp3", @"C:\\t\c.mp3", }
            ).SetName("基本的なM3Uファイルのパース");

            yield return new TestCaseData(
                @"C:\\music\song1.mp3" + "\r\n" +
                @"C:\\music\song2.mp3",
                new[] { @"C:\\music\song1.mp3", @"C:\\music\song2.mp3", }
            ).SetName("2つのエントリだけのM3U");

            yield return new TestCaseData(
                @" # C:\\music\song1.mp3" + "\r\n" +
                @"C:\\music\song2.mp3 # comment" + "\r\n" +
                @"C:\\music\song3.mp3",
                new[] { @"C:\\music\song2.mp3", @"C:\\music\song3.mp3", }
            ).SetName("コメントアウトを含むM3U");

            yield return new TestCaseData(
                "", Array.Empty<string>()
            ).SetName("空のM3U");
        }

        [TestCaseSource(nameof(ParseM3U_TestCases))]
        public void ParseM3u_Test(string m3UData, string[] expected)
        {
            var storage = new SoundStorage();
            var results = storage.ParseM3U(m3UData).Select(s => s.FullName);

            CollectionAssert.AreEqual(expected, results);
        }
    }
}