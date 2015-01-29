using System.IO;

namespace BackMeUp.Wrappers
{
    public class SystemFile : IFile
    {
        public void Copy(string sourceFileName, string destFileName)
        {
            File.Copy(sourceFileName, destFileName);
        }

        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        //public FileStream OpenRead(string path)
        //{
        //    return File.OpenRead(path);
        //}

        //public FileStream OpenWrite(string path)
        //{
        //    return File.OpenWrite(path);
        //}

        public byte[] ReadAllBytes(string path)
        {
            return File.ReadAllBytes(path);
        }
    }
}