namespace BackMeUp.Data
{
    public class Game
    {
        private Game()
        {
        }

        public Game(string name, int saveGame)
        {
            Name = name;
            SaveGameNumber = saveGame;
        }

        public Game(int saveGame) : this(string.Concat(saveGame, "_Unidentified"), saveGame)
        {
        }

        public string Name { get; set; }
        public int SaveGameNumber { get; set; }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0)*397) ^ SaveGameNumber;
            }
        }

        public override string ToString()
        {
            return string.Format("Name: \"{0}\", GameSave: {1}", Name, SaveGameNumber);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Game) obj);
        }

        protected bool Equals(Game other)
        {
            return string.Equals(Name, other.Name) && SaveGameNumber == other.SaveGameNumber;
        }
    }
}