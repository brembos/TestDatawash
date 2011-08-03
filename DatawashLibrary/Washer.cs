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
        private readonly List<string> personnummer;
        private readonly Stream peopleStream;
        private IDictionary<string, int> locationCounter;
        private int pnrCounter;
        private int i;

        public Washer(FileStream people, FileStream criterias)
        {
            peopleStream = people;
            personnummer = ReadCriterias(criterias).Select(c => c.Personnr).ToList();
            locationCounter = new LocationCounter().GetList();
        }


        public void CleanTo(FileStream file)
        {
            var encoding = Encoding.GetEncoding(1252);
            var personDecoder = new FixedFileReader<Person>();
            personDecoder.TrimInput = TrimInputMode.NoTrim;
            var writer = new FixedFileWriter<Person>();
            var streamWriter = new StreamWriter(file);
            using (var reader = new StreamReader(peopleStream, encoding))
            {
                string line;
                //skip first row
                reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    var person = personDecoder.ReadLine(line);
                    Debug.WriteLine("");
                    Debug.Write(i++);
                    if (personnummer.Contains(person.Personnr))
                    {
                        Debug.Write("Ignore duplicate " + person.Personnr);
                        continue;
                    }

                    if (pnrCounter >= personnummer.Count - 1)
                    {
                        Debug.Write("Ingen ledige personnr");
                        continue;
                    }

                    string key = person.Kommunenr + "-" + person.Kretsnr;
                    if (locationCounter.ContainsKey(key))
                    {
                        int value = locationCounter[key];

                        if (value <= 40)
                        {
                            locationCounter[key] = value + 1;
                            person.Personnr = GetNextNumber();
                            Debug.Write(" " + key + " " + person.Personnr + "(" + pnrCounter + ")");
                        }
                    }
                    streamWriter.WriteLine(writer.MakeLine(person));

                }
            }

        }

        private string GetNextNumber()
        {
            return personnummer[pnrCounter++];
        }

        private IEnumerable<Criteria> ReadCriterias(Stream criteriasStream)
        {
            var reader = new FixedFileReader<Criteria>();
            return reader.ReadFile(criteriasStream, false);
        }


    }
}