using System.Collections.Generic;

namespace BackMeUp.Data
{
    public class GameName
    {
        private readonly IDictionary<int, string> _games;

        public GameName(IDictionary<int, string> games)
        {
            _games = games;
        }

        public string FromId(int id)
        {
            return _games.TryGetValue(id, out var name) ? name : $"{id}_Unidentified";
        }
    }
}