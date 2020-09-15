using Game_Server.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game_Server.Web.Json
{
    public class TopicMasteryStat
    {
        [JsonProperty("topicId")]
        public int TopicId { get; set; }
        [JsonProperty("meanCorrectness")]
        public double MeanCorrectness
        {
            get
            {
                if (StudentsStats.Where(student => student.TotalAttempts > 0).Count() == 0)
                    return 0;
                return StudentsStats.Select(x => x.Correctness).Average();
            }
        }
        [JsonProperty("medianCorrectness")]
        public double MedianCorrectness
        {
            get
            {
                if (StudentsStats.Where(student => student.TotalAttempts > 0).Count() == 0)
                    return 0;
                var count = StudentsStats.Where(student => student.TotalAttempts > 0).Count();
                int half = count / 2;
                var sorted = StudentsStats.Where(student => student.TotalAttempts > 0).Select(x => x.Correctness).OrderBy(n => n);
                if ((count % 2) == 0)
                {
                    return (sorted.ElementAt(half) + sorted.ElementAt(half - 1)) / 2;
                }
                else
                {
                    return sorted.ElementAt(half);
                }
            }
        }
        [JsonProperty("sd")]
        public double StandardDeviation
        {
            get
            {
                if (StudentsStats.Where(student => student.TotalAttempts > 0).Count() == 0)
                    return 0;
                return StudentsStats.Where(student => student.TotalAttempts > 0).Select(x => x.Correctness).StdDev();
            }
        }
        [JsonProperty("highestCorrectness")]
        public double HighestCorrectness
        {
            get
            {
                if (StudentsStats.Where(student => student.TotalAttempts > 0).Count() == 0)
                    return 0;
                return StudentsStats.Where(student => student.TotalAttempts > 0).Select(x => x.Correctness).Max();
            }
        }
        [JsonProperty("lowestCorrectness")]
        public double LowestCorrectness
        {
            get
            {
                if (StudentsStats.Where(student => student.TotalAttempts > 0).Count() == 0)
                    return 0;
                return StudentsStats.Where(student => student.TotalAttempts > 0).Select(x => x.Correctness).Min();
            }
        }

        [JsonProperty("studentsStats")]
        public StudentStat[] StudentsStats
        {
            get
            {
                return StudentStatsKeyValuePair.Values.ToArray();
            }
        }

        [JsonIgnore]
        public Dictionary<string, StudentStat> StudentStatsKeyValuePair = new Dictionary<string, StudentStat>();
    }
}
