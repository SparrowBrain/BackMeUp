using System.IO;
using System.Linq;

namespace BackMeUp.Utils
{
    public interface IDirectoryNameFixer
    {
        string RemoveInvalidCharacters(string directoryName);
    }

    public class DirectoryNameFixer : IDirectoryNameFixer
    {
        public string RemoveInvalidCharacters(string directoryName)
        {
            if (string.IsNullOrEmpty(directoryName))
            {
                return directoryName;
            }

            foreach (var invalidCharacter in Path.GetInvalidFileNameChars())
            {
                if (directoryName.Contains(invalidCharacter))
                {
                    directoryName = string.Concat(directoryName.Split(invalidCharacter));
                }
            }

            return directoryName;
        }
    }
}