using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace Clinic.Utilities.Convertors
{
    public static class DateConvertor
    {
        public static string ToHijriSlashed(this DateTime value)
        {
            PersianCalendar pc = new PersianCalendar();

            return pc.GetYear(value) + "/" + pc.GetMonth(value).ToString("00") + "/" +
                   pc.GetDayOfMonth(value).ToString("00");
        }

        public static string ToHijriFarsi(this DateTime value)
        {
            var pc = new PersianDateTime(value);
            return pc.ToString("dddd d MMMM yyyy");
        }
        
        public static string ToHijriHour(this DateTime value)
        {
            var pc = new PersianDateTime(value);
            return pc.ToString("ساعت HH");
        }
        
        public static string ToHijriFarsiWithHour(this DateTime value)
        {
            var pc = new PersianDateTime(value);
            return pc.ToString("dddd d MMMM yyyy ساعت HH");
        }

        public static string ElapsedTime(this DateTime value)
        {
            var now = DateTime.Now;
            TimeSpan elapsed = now.Subtract(value);

            var result = "";
            if (elapsed.Days < 1)
            {
                if (elapsed.Hours < 1)
                {
                    result = "در " + Math.Round(elapsed.TotalMinutes) + " دقیقه قبل";
                }
                else
                {
                    result = "در " + Math.Round(elapsed.TotalHours) + " ساعت قبل";
                }
            }
            else
            {
                result = "در " + Math.Round(elapsed.TotalDays) + " روز قبل";
            }

            return result;
        }
    }
}
