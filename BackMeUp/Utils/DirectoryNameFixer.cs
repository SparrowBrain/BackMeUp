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

            var invalidChars = new List<char>()
            {
                '"',
                '<',
                '>',
                '|',
                '\b',
                '\0',
                '\t',
                '?',
                '*',
                ':',
                '\\',
                '/'
            };
            invalidChars.AddRange(Path.GetInvalidPathChars());

            foreach (var invalidCharacter in invalidChars)
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