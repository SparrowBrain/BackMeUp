using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackMeUp.Data
{
    public class GameConfiguration
    {
        public GameConfiguration()
        {
            Games = new List<Game>();
        }

        public List<Game> Games { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((GameConfiguration)obj);
        }

        protected bool Equals(GameConfiguration other)
        {
            return Games.Count == other.Games.Count &&
                   Games.Select((t, i) => t.Equals(other.Games[i])).All(equals => equals);
        }

        public override int GetHashCode()
        {
            var hashCode = Games.GetHashCode();
            return hashCode;
        }
        
        public override string ToString()
        {
            var gameListBuilder = new StringBuilder();
            foreach (var game in Games)
            {
                gameListBuilder.AppendFormat("{0};", game);
            }
            return "GameList=[" + gameListBuilder + "]";
        }
    }
}