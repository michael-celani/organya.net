using System.IO;
using NAudio.Wave;
using Organya.IO;

namespace Organya.Converter
{
    class Program
    {
        static void Main(string[] args)
        {
            using Stream orgStream = new FileStream("/Users/mcelani/Downloads/all/pb2nodrum.org", FileMode.Open);
            using Stream insStream = new FileStream("/Users/mcelani/Downloads/organya player/orgsamp.dat", FileMode.Open);
            using OrganyaReader reader = new OrganyaReader(orgStream);
            using InstrumentReader instReader = new InstrumentReader(insStream);

            Organya org = reader.Read();
            OrganyaInstrument[] instruments = instReader.Read();

            var sampleProv = new OrganyaSampleProvider(org, instruments);

            using (var outputDevice = new WaveOutEvent())
            {
                WaveFileWriter.CreateWaveFile("/Users/mcelani/Desktop/check.wav", sampleProv.ToWaveProvider());
            }
        }
    }
}
