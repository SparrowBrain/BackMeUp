namespace BackMeUp.Wrappers
{
    public interface IFile
    {
        void Copy(string sourceFileName, string destFileName);
        byte[] ReadAllBytes(string path);
    }
}