using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserList
{
    public static class DateTimeExtensions
    {
        public static int YearsPassedCnt(this DateTime thisDateTime, DateTime anotherDateTime)
        {
            return (thisDateTime.Year - anotherDateTime.Year) - (thisDateTime.DayOfYear >= anotherDateTime.DayOfYear ? 0 : 1);
        }
    }
}
