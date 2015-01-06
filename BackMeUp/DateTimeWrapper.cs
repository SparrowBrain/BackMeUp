using System;

namespace BackMeUp
{
    public interface IDateTime
    {
        DateTime Now();
    }

    public class DateTimeWrapper:IDateTime
    {
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}