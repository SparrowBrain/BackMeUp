using System;
using System.IO;
using BackMeUp.Wrappers;

namespace BackMeUp.Utils
{
    public class Comparer
    {
        public Comparer(ICrc16 crc, IDirectory directory, IFile file)
        {
            Crc = crc;
            Directory = directory;
            File = file;
        }

        private ICrc16 Crc { get; set; }
        private IDirectory Directory { get; set; }
        public IFile File { get; set; }

        public bool CompareDirectories(string saveDir, string backupDir)
        {
            if (string.IsNullOrEmpty(saveDir))
            {
                throw new ArgumentException("saveDir");
            }
            if (string.IsNullOrEmpty(backupDir))
            {
                throw new ArgumentException("backupDir");
            }

            var saveFiles = Directory.GetFiles(saveDir);
            var backupFiles = Directory.GetFiles(backupDir);

            if (saveFiles.Length != backupFiles.Length)
            {
                return false;
            }

            for (var i = 0; i < saveFiles.Length; i++)
            {
                if (!CompareFiles(saveFiles[i], backupFiles[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private bool CompareFiles(string file1, string file2)
        {
            if (string.IsNullOrEmpty(file1))
            {
                throw new ArgumentException("file1");
            }
            if (string.IsNullOrEmpty(file2))
            {
                throw new ArgumentException("file2");
            }

            if (Path.GetFileName(file1) != Path.GetFileName(file2))
            {
                return false;
            }

            if (CalculateChecksum(file1) != CalculateChecksum(file2))
            {
                return false;
            }
            return true;
        }

        private ushort CalculateChecksum(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("filePath");
            }

            var bytes = ReadFile(filePath);
            return Crc.ComputeChecksum(bytes);
        }

        private byte[] ReadFile(string filePath)
        {
            var bytes = File.ReadAllBytes(filePath);
            return bytes;
        }
    }
}