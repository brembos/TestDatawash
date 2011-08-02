using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using LinqToFlatFile;

namespace DatawashLibrary
{
    public class Washer
    {
        private Stream peopleStream;
        private Stream criteriasStream;
        private List<Criteria> personnummer;
        private readonly IDictionary<string, string[]> kommunenr = new Dictionary<string, string[]>();
        private int i;
        List<Person> duplicates = new List<Person>();

        public Washer(FileStream people, FileStream criterias)
        {
            peopleStream = people;
            criteriasStream = criterias;
            personnummer = ReadCriterias().ToList();
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

        public Stream Clean()
        {
            Debug.WriteLine("Clean");
            IList<Person> people = ReadPeople().ToList();
            Debug.WriteLine("People found:" + people.Count);

            var writer = new LinqToFlatFile.FixedFileWriter<Person>();
            foreach (var kommune in kommunenr)
            {
                var kommune1 = kommune;
                var list = people.Where(p => p.Kommunenr == kommune1.Key).ToList();
                Debug.WriteLine("Kommune:" + kommune1.Key + " found:" + list.Count);

                foreach (var krets in kommune1.Value)
                {
                    int krets1 = Int32.Parse(krets);
                    var kretsList = list.Where(p => Int32.Parse(p.Kretsnr) == krets1).Take(10);
                    foreach (var person in kretsList)
                    {
                        var personnr = GetNextNumber();
                        duplicates.AddRange(people.Where(p => p.Personnr == personnr && !p.Equals(person)));

                        person.Personnr = personnr;
                        Debug.WriteLine(i + "-" + kommune1.Key + "/" + krets1 + " " + person.Personnr);
                    }
                }
            }
            foreach (var duplicate in duplicates)
            {

                Debug.WriteLine("Removed duplicate :" + duplicate.Personnr);
                people.Remove(duplicate);

            }
            return writer.WriteFile(people, false);
        }

        private string GetNextNumber()
        {
            return personnummer[i++].Personnr;
        }

        private IEnumerable<Criteria> ReadCriterias()
        {
            var reader = new LinqToFlatFile.FixedFileReader<Criteria>();
            return reader.ReadFile(criteriasStream, false);
        }

        private IEnumerable<Person> ReadPeople()
        {
            var reader = new LinqToFlatFile.FixedFileReader<Person>();
            reader.TrimInput = TrimInputMode.NoTrim;
            return reader.ReadFile(peopleStream, true);
        }


    }
}
