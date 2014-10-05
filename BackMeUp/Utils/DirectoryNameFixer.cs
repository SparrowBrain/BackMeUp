using System.IO;
using System.Linq;

namespace BackMeUp.Utils
{
    public class DirectoryNameFixer
    {
        public string ReplaceInvalidCharacters(string path)
        {
            foreach (var invalidCharacter in Path.GetInvalidPathChars())
            {
                if (path.Contains(invalidCharacter))
                {
                    path = path.Replace(invalidCharacter, '_');
                }
            }
            return path;
        }
    }
}