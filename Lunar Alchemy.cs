using System;
using System.IO;
using System.Threading;

namespace LA
{
    
    class LAMain
    {

        public LAMain(string ROM)
        {
            using (FileStream fs = File.Open(ROM, FileMode.Open, FileAccess.ReadWrite, FileShare.Read)) { using (BinaryReader reader = new BinaryReader(fs)) { using (BinaryWriter writer = new BinaryWriter(fs))
            {
                        FileInfo info = new FileInfo(ROM);

                        Console.WriteLine(BitConverter.ToString(reader.ReadBytes(200)).Replace("-", String.Empty));
            } } }
        }
    }

    class MainClass
    { 
        public static void Main(string[] args)
        { 
            LAMain LAobj = new LAMain(args[0]);
        }
    }
}