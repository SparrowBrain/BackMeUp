namespace BackMeUp.Wrappers
{
    public interface IFile
    {
        void Copy(string sourceFileName, string destFileName);
        bool Exists(string path);
        //FileStream OpenRead(string path);
        //FileStream OpenWrite(string path);
        byte[] ReadAllBytes(string path);
    }
}