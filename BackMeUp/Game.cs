namespace BackMeUp
{
    public class Game
    {
        public Game(string name, int spool, int saveGame)
        {
            Name = name;
            SpoolNumber = spool;
            SaveGameNumber = saveGame;
        }

        public Game(int spool, int saveGame) : this(string.Concat(saveGame, "_Unidentified"), spool, saveGame)
        {
        }

        public string Name { get; set; }
        public int SpoolNumber { get; set; }
        public int SaveGameNumber { get; set; }

        public override string ToString()
        {
            return string.Format("Name: \"{0}\", Spool: {1}, GameSave: {2}", Name, SpoolNumber, SaveGameNumber);
        }
    }
}