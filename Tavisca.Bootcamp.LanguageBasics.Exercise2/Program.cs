using System;
using System.Collections.Generic;
using System.Linq;

namespace Tavisca.Bootcamp.LanguageBasics.Exercise1
{
    public static class Program
    {
        static void Main(string[] args)
        {
            Test(new[] { "12:12:12" }, new[] { "few seconds ago" }, "12:12:12");
            Test(new[] { "23:23:23", "23:23:23" }, new[] { "59 minutes ago", "59 minutes ago" }, "00:22:23");
            Test(new[] { "00:10:10", "00:10:10" }, new[] { "59 minutes ago", "1 hours ago" }, "impossible");
            Test(new[] { "11:59:13", "11:13:23", "12:25:15" }, new[] { "few seconds ago", "46 minutes ago", "23 hours ago" }, "11:59:23");
            Console.ReadKey(true);
        }
        private static void Test(string[] postTimes, string[] showTimes, string expected)
        {
            var result = GetCurrentTime(postTimes, showTimes).Equals(expected) ? "PASS" : "FAIL";
            var postTimesCsv = string.Join(", ", postTimes);
            var showTimesCsv = string.Join(", ", showTimes);
            Console.WriteLine($"[{postTimesCsv}], [{showTimesCsv}] => {result}");
        }
        public static string GetCurrentTime(string[] exactPostTime, string[] showPostTime)
        {
            string CurrentTime = null;
            if (exactPostTime.Length == 1)
            {
                CurrentTime = singleIndex(exactPostTime[0], showPostTime[0]);
                return CurrentTime;
            }
            int Matches = 0;
            for (int i = 0; i < showPostTime.Length; i++)
            {
                for (int j = i + 1; j < showPostTime.Length; j++)
                {
                    if (exactPostTime[i] == exactPostTime[j])
                    {
                        Matches++;
                        if (showPostTime[i] != showPostTime[j])
                        {
                            return "impossible";
                        }
                    }
                }
            }
            if ((Matches * 2) == exactPostTime.Length)
            {
                CurrentTime = singleIndex(exactPostTime[0], showPostTime[0]);
                return CurrentTime;
            }
            string[] LowerBounds = findLowerBounds(exactPostTime, showPostTime);
            string[] UpperBounds = findUpperBounds(exactPostTime, showPostTime);
            List<string> AllTimes = new List<string>();
            for(int i=0;i<LowerBounds.Length;i++)
            {
                AllTimes.Add(exactPostTime[i]);
                AllTimes.Add(UpperBounds[i]);
                AllTimes.Add(LowerBounds[i]);

            }
            AllTimes = AllTimes.Distinct().ToList();
            AllTimes.Sort();
            foreach (var time in AllTimes)
            {
                int IntervalsMatched = 0;
                for (int i = 0; i < LowerBounds.Length; i++)
                {
                    if (CheckIntervals(LowerBounds[i], time, UpperBounds[i]) == 1)
                    {
                        IntervalsMatched++;
                    }
                }
                if (IntervalsMatched == LowerBounds.Length)//If the given time is falling in all intervals
                {
                    return time;
                }
            }
            return "impossible";
        }
        public static string[] findLowerBounds(string[] exactPostTime, string[] showPostTime)
        {//findLowerBounds will return a string array of time which serve as 
         //lower bounds of the intervals created by the showpostime and exactposttime
            string[] LowerBounds = new string[exactPostTime.Length];
            for (int i = 0; i < exactPostTime.Length; i++)
            {
                if (showPostTime[i].Contains("few"))
                {
                    LowerBounds[i] = exactPostTime[i];
                }
                else if (showPostTime[i].Contains("minutes"))
                {
                    String[] Minutes = showPostTime[i].Split(' ');
                    DateTime CurrentTime = Convert.ToDateTime(exactPostTime[i]);
                    CurrentTime = CurrentTime.AddMinutes(Int32.Parse(Minutes[0]));
                    LowerBounds[i] = CurrentTime.ToString().Split(' ')[1];
                }
                if (showPostTime[i].Contains("hours"))
                {
                    String[] Hours = showPostTime[i].Split(' ');
                    DateTime CurrentTime = Convert.ToDateTime(exactPostTime[i]);
                    CurrentTime = CurrentTime.AddHours(Int32.Parse(Hours[0]));
                    LowerBounds[i] = CurrentTime.ToString().Split(' ')[1];
                }
            }
            return LowerBounds;
        }
        //findUpperBounds will return a string array of time which serve as 
        //upper bounds of the intervals created by the showpostime and exactposttime
        public static string[] findUpperBounds(string[] exactPostTime, string[] showPostTime)
        {
            string[] UpperBounds = new string[exactPostTime.Length];
            for (int i = 0; i < exactPostTime.Length; i++)
            {
                if (showPostTime[i].Contains("few"))
                {
                    DateTime CurrentTime = Convert.ToDateTime(exactPostTime[i]);
                    CurrentTime = CurrentTime.AddSeconds(59);
                    UpperBounds[i] = CurrentTime.ToString().Split(' ')[1];
                }
                else if (showPostTime[i].Contains("minutes"))
                {
                    String[] Minutes = showPostTime[i].Split(' ');
                    DateTime CurrentTime = Convert.ToDateTime(exactPostTime[i]);
                    CurrentTime = CurrentTime.AddMinutes(Int32.Parse(Minutes[0]));
                    CurrentTime = CurrentTime.AddSeconds(59);
                    UpperBounds[i] = CurrentTime.ToString().Split(' ')[1];
                }
                if (showPostTime[i].Contains("hours"))
                {
                    String[] Hours = showPostTime[i].Split(' ');
                    DateTime CurrentTime = Convert.ToDateTime(exactPostTime[i]);
                    CurrentTime = CurrentTime.AddHours(Int32.Parse(Hours[0]));
                    CurrentTime = CurrentTime.AddMinutes(59);
                    CurrentTime = CurrentTime.AddSeconds(59);
                    UpperBounds[i] = CurrentTime.ToString().Split(' ')[1];
                }
            }
            return UpperBounds;
        }
        public static string singleIndex(String exactPostTime, string showPostTime)
        {//single handles the case when there is only one element in exactposttime 
         //or if all the elements in exactposttime and showposttime are exactly same
            if (showPostTime.Contains("few"))
            {
                return exactPostTime;
            }
            if (showPostTime.Contains("minutes"))
            {
                String[] Minutes = showPostTime.Split(' ');
                DateTime CurrentTime = Convert.ToDateTime(exactPostTime);
                CurrentTime = CurrentTime.AddMinutes(Int32.Parse(Minutes[0]));
                string answer = CurrentTime.ToString().Split(' ')[1];
                return answer;
            }
            if (showPostTime.Contains("hours"))
            {
                String[] Hours = showPostTime.Split(' ');
                DateTime CurrentTime = Convert.ToDateTime(exactPostTime);
                CurrentTime = CurrentTime.AddHours(Int32.Parse(Hours[0]));
                string answer = CurrentTime.ToString().Split(' ')[1];
                return answer;
            }
            return null;
        }
        public static int CheckIntervals(string LowerBound, string CurrentCheckTime, string UpperBound)
        {//CheckIntervals simply checks if a given time is in between given interval
            String[] UpperBoundterms = UpperBound.Split(':');
            int UpperBoundHour = Int32.Parse(UpperBoundterms[0]);
            int UpperBoundMinute = Int32.Parse(UpperBoundterms[1]);
            int UpperBoundSeconds = Int32.Parse(UpperBoundterms[2]);
            if (UpperBoundMinute == 0)
            {
                UpperBoundMinute = 60;
            }
            if (UpperBoundSeconds == 0)
            {
                UpperBoundSeconds = 60;
            }
            UpperBound = UpperBoundHour + ":" + UpperBoundMinute + ':' + UpperBoundSeconds;
            if (String.Compare(LowerBound, CurrentCheckTime) <= 0 && String.Compare(CurrentCheckTime, UpperBound) <= 0)
            {
                return 1;
            }
            return 0;
        }
    }
}
