using System.Linq;

namespace Organya.Converter
{
    public class OrganyaInstrument
    {
        /// <summary>
        /// The instrument number.
        /// </summary>
        public int InstrumentNumber { get; }

        /// <summary>
        /// The samples used to define the instrument.
        /// </summary>
        public sbyte[] Samples { get; }

        public OrganyaInstrument(int instrumentNum, sbyte[] samples)
        {
            InstrumentNumber = instrumentNum;
            Samples = samples.ToArray();
        }
    }
}
