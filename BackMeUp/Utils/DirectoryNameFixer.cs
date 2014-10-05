using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BackMeUp.Utils
{
    public class DirectoryNameFixer
    {
        public string ReplaceInvalidCharacters(string path)
        {
            var root = Path.GetPathRoot(path);
            if (!string.IsNullOrEmpty(root))
            {
                path = path.Substring(root.Length);
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
                '*'
            };

            invalidChars.AddRange(Path.GetInvalidPathChars());

            foreach (var invalidCharacter in invalidChars)
            {
                if (path.Contains(invalidCharacter))
                {
                    path = path.Replace(invalidCharacter, '_');
                }
                
            }

            if (!string.IsNullOrEmpty(root))
            {
                path = Path.Combine(root, path);
            }
            return path;
        }
    }
}