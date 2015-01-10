using System;

namespace BackMeUp.Wrappers
{
    public class DateTimeWrapper:IDateTime
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}