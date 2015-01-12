using System;
using System.IO;

namespace BackMeUp.Wrappers
{
    public interface IDirectory
    {
        DirectoryInfo CreateDirectory(string path);
        bool Exists(string path);
        string[] GetDirectories(string path);
        string[] GetDirectories(string path, string searchPattern, SearchOption searchOption);
        string[] GetFiles(string path);
        string[] GetFileSystemEntries(string path);
        DateTime GetLastWriteTime(string path);
    }
}