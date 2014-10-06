using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BackMeUp.Utils
{
    public class DirectoryNameFixer
    {
        public string ReplaceInvalidCharacters(string directoryName)
        {
            if (string.IsNullOrEmpty(directoryName))
            {
                return directoryName;
            }

            foreach (var invalidCharacter in Path.GetInvalidFileNameChars())
            {
                if (directoryName.Contains(invalidCharacter))
                {
                    directoryName = directoryName.Replace(invalidCharacter, '_');
                }
            }

            return directoryName;
        }
    }
}