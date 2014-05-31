namespace BackMeUp
{
    public class Game
    {
        public string Name { get; set; }
        public int SpoolNumber { get; set; }
        public int GameSaveNumber { get; set; }

        public override string ToString()
        {
            return string.Format("Name: \"{0}\", Spool: {1}, GameSave: {2}", Name, SpoolNumber, GameSaveNumber);
        }
    }
}