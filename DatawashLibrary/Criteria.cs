using LinqToFlatFile;

namespace DatawashLibrary
{
    public class Criteria
    {
        [FixedPosition(0, 11)]
        public string Personnr { get; set; }

        public override string ToString()
        {
            return Personnr;
        }
    }
}