using System.IO;
using BackMeUp.Wrappers;

namespace BackMeUp.Utils
{
    public class Comparer
    {
        private ICrc16 Crc { get; set; }
        private IDirectory Directory { get; set; }
        public Comparer(ICrc16 crc, IDirectory directory)
        {
            Crc = crc;
            Directory = directory;
        }

        public bool CompareDirectories(string saveDir, string backupDir)
        {
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

        public ushort CalculateChecksum(string filePath)
        {
            byte[] bytes;
            using (var file = new FileStream(filePath, FileMode.Open))
            {
                bytes = new byte[file.Length];
                file.Read(bytes, 0, (int) file.Length);
            }
            return Crc.ComputeChecksum(bytes);
        }
    }
}