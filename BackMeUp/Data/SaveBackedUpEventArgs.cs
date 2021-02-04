using System;

namespace BackMeUp.Data
{
    public class SaveBackedUpEventArgs : EventArgs
    {
        public string Game { get; set; }
        public DateTime DateTime { get; set; }
    }
}