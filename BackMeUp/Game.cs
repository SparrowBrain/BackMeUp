namespace BackMeUp
{
    public class Game
    {
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

        public override string ToString()
        {
            return string.Format("Name: \"{0}\", GameSave: {1}", Name, SaveGameNumber);
        }
    }
}