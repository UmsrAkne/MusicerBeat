using System;
using System.IO;

namespace MusicerBeat.Utils
{
    public static class FileSystemWrapper
    {
        public static string[] GetSubDirectories(string path)
        {
            if (!Directory.Exists(path))
            {
                return Array.Empty<string>();
            }

            try
            {
                return Directory.GetDirectories(path);
            }
            catch (UnauthorizedAccessException)
            {
                return Array.Empty<string>();
            }
            catch (IOException)
            {
                return Array.Empty<string>();
            }
        }

        public static string[] GetFiles(string path)
        {
            if (!Directory.Exists(path))
            {
                return Array.Empty<string>();
            }

            try
            {
                return Directory.GetFiles(path);
            }
            catch (UnauthorizedAccessException)
            {
                return Array.Empty<string>();
            }
            catch (IOException)
            {
                return Array.Empty<string>();
            }
        }
    }
}