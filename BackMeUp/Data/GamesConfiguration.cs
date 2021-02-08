using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackMeUp.Data
{
    public class GamesConfiguration
    {
        public GamesConfiguration()
        {
            Games = new Dictionary<int, string>();
        }

        public IDictionary<int, string> Games { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((GamesConfiguration)obj);
        }

        protected bool Equals(GamesConfiguration other)
        {
            return Games.Count == other.Games.Count && Games.Equals(other);
        }

        public override int GetHashCode()
        {
            var hashCode = Games.GetHashCode();
            return hashCode;
        }
        
        public override string ToString()
        {            
            var gameStringBuilder = new StringBuilder();
            foreach (var game in Games)
            {
                gameStringBuilder.AppendFormat("{0}; ", game);
            }
            return $"GamesConfiguration: {gameStringBuilder}";
        }
    }
}