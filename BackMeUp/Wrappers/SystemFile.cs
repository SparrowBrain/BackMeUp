namespace BackMeUp.Wrappers
{
    public class SystemFile : IFile
    {
        public void Copy(string sourceFileName, string destFileName)
        {
            System.IO.File.Copy(sourceFileName, destFileName);
        }
    }
}