using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Decoder = Filbert.Decoder;
using Encoder = Filbert.Encoder;
using System.IO;

namespace MapGen
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var inp = Console.OpenStandardInput())
            using (var outp = Console.OpenStandardOutput())
            using (var sin = new BinaryReader(inp))
            using (var sout = new BinaryWriter(outp))
                while (true)
                {
                    var length = sin.ReadBytes(4);
                    var tIn = Decoder.decode(sin.BaseStream);
                    sout.Write(length);
                    Encoder.encode(tIn, outp);
                }
        }
    }
}
