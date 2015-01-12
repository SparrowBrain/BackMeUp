using System;

namespace BackMeUp.Wrappers
{
    public class SystemTime
    {
        private static DateTime _dateTime;

        public static void SetDateTime(DateTime dateTime)
        {
            _dateTime = dateTime;
        }

        public static void Reset()
        {
            _dateTime = DateTime.MinValue;
        }
        
        public static DateTime Now()
        {
            return _dateTime == DateTime.MinValue ? DateTime.Now : _dateTime;
        }
    }
}