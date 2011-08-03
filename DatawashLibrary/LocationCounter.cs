using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatawashLibrary
{
    public class LocationCounter
    {
        private readonly IDictionary<string, string[]> kommunenr = new Dictionary<string, string[]>();
        public LocationCounter()
        {
            kommunenr.Add("0437", new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" });
            kommunenr.Add("0716", new[] { "0", "1", "2", "3", "4", "5", "6" });
            kommunenr.Add("1002", new[] { "0", "1", "2", "3", "6" });
            kommunenr.Add("1102", new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13" });
            kommunenr.Add("1260", new[] { "0", "1", "2", "3", "4" });
            kommunenr.Add("1438", new[] { "0", "1", "2", "7", "10", "13", "17", "20", "23" });
            kommunenr.Add("1504", new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12" });
            kommunenr.Add("1804", new[] { "0", "1", "6", "8", "9", "12", "22", "25", "30", "35", "40", "45", "50", "60" });
            kommunenr.Add("1824", new[] { "0", "1", "2", "3", "4", "5", "6", "7", "10", "11", "12", "14", "15", "16" }); ;
            kommunenr.Add("2004", new[] { "0", "1", "4", "9", "10", "12", "13" });

        }

        public IDictionary<string, int> GetList()
        {
            IDictionary<string, int> locationCounter = new Dictionary<string, int>();
            foreach (var kommune in kommunenr)
            {
                foreach (var krets in kommune.Value)
                {
                    locationCounter.Add(kommune.Key+"-"+krets.PadLeft(5,'0'),0);
                }
            }
            return locationCounter;
        }
    }
}
