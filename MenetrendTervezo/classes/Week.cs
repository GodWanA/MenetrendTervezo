using System;
using System.Collections.Generic;
using System.Linq;

namespace MenetrendTervezo.classes
{
    class Week
    {
        public int N { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public Week(int n, DateTime start, DateTime end)
        {
            this.N = n;
            this.Start = start;
            this.End = end;
        }

        public override string ToString()
        {
            return this.N.ToString().PadLeft(2, ' ') + " (" + Start.ToString("MMMM dd.") + " - " + End.ToString("MMMM dd.") + ")";
        }

        public static List<Week> Weeks { get; private set; } = new List<Week>();

        public static List<Week> getWeeks()
        {
            Weeks.Clear();

            var start = new DateTime(DateTime.Today.Year, 1, 1);
            var eddig = new DateTime(DateTime.Today.Year + 1, 1, 1);
            int n = 1;

            while (start < eddig)
            {
                while (start.DayOfWeek != DayOfWeek.Monday)
                {
                    start = start.AddDays(-1);
                }

                var end = start.AddDays(6);

                Weeks.Add(new Week(n, start, end));

                n++;
                start = end.AddDays(1);
            }

            return Weeks;
        }

        public static Week getCurrentWeek()
        {
            return Weeks.Where(x => x.Start <= DateTime.Today && DateTime.Today <= x.End).FirstOrDefault();
        }

        //var jan1 = new DateTime(DateTime.Today.Year, 1, 1);
        ////beware different cultures, see other answers
        //var startOfFirstWeek = jan1.AddDays(1 - (int)(jan1.DayOfWeek));
        //var weeks =
        //    Enumerable
        //        .Range(0, 54)
        //        .Select(i => new {
        //            weekStart = startOfFirstWeek.AddDays(i * 7)
        //        })
        //        .TakeWhile(x => x.weekStart.Year <= jan1.Year)
        //        .Select(x => new {
        //            x.weekStart,
        //            weekFinish = x.weekStart.AddDays(4)
        //        })
        //        .SkipWhile(x => x.weekFinish < jan1.AddDays(1))
        //        .Select((x, i) => new {
        //            x.weekStart,
        //            x.weekFinish,
        //            weekNum = i + 1
        //        });
    }
}
