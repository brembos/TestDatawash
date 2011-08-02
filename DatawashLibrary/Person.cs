using LinqToFlatFile;

namespace DatawashLibrary
{
    public class Person
    {
        [FixedPosition(37, 47)]
        public string Personnr { get; set; }

        [FixedPosition(239, 242)]
        public string Kommunenr { get; set; }

        [FixedPosition(380, 384)]
        public string Kretsnr { get; set; }


        [FixedPosition(0, 36)]
        public string A { get; set; }
        [FixedPosition(48, 238)]
        public string B { get; set; }
        [FixedPosition(239, 379)]
        public string C { get; set; }
        [FixedPosition(385, 508)]
        public string D { get; set; }


        public override string ToString()
        {
            return Personnr;
        }
    }
}