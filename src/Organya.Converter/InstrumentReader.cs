using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Organya.Converter
{
    /// <summary>
    /// A stream reader that converts a <see cref="Stream"/> into instrument data.
    /// Currently uses orgsamp.dat.
    /// </summary>
    public class InstrumentReader : IDisposable
    {
        /// <summary>
        /// The stream reader for the internal stream.
        /// </summary>
        private BinaryReader StreamReader { get; }

        /// <summary>
        /// Constructs a new <see cref="InstrumentReader"/>.
        /// </summary>
        /// <param name="instrumentStream">The stream containing the data.</param>
        /// <exception cref="ArgumentNullException">
        /// instrumentStream is null.
        /// </exception>
        public InstrumentReader(Stream instrumentStream) : this(instrumentStream, false) { }

        /// <summary>
        /// Constructs a new <see cref="InstrumentReader"/>.
        /// </summary>
        /// <param name="instrumentStream">The stream containing the data.</param>
        /// <param name="leaveOpen">
        /// If true, do not dispose the underlying stream when this reader is
        /// disposed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// instrumentStream is null.
        /// </exception>
        public InstrumentReader(Stream instrumentStream, bool leaveOpen)
        {
            if (instrumentStream == null)
            {
                throw new ArgumentNullException(nameof(instrumentStream));
            }

            StreamReader = new BinaryReader(instrumentStream, Encoding.ASCII, leaveOpen);
        }

        /// <summary>
        /// Reads <see cref="Organya"/>s from the stream.
        /// </summary>
        /// <returns>The <see cref="Organya"/>s from the stream.</returns>
        public OrganyaInstrument[] Read()
        {
            byte numInstruments = StreamReader.ReadByte();

            // Skip three bytes:
            StreamReader.ReadByte();
            StreamReader.ReadByte();
            StreamReader.ReadByte();

            return Enumerable.Range(0, numInstruments).Select(ReadInstrument).ToArray();
        }

        public OrganyaInstrument ReadInstrument(int instrumentNum)
        {
            sbyte[] bytes = new sbyte[256];

            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = StreamReader.ReadSByte();
            }

            return new OrganyaInstrument(instrumentNum, bytes);
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
            {
                return;
            }

            if (disposing)
            {
                StreamReader.Dispose();
            }

            disposedValue = true;
        }

        public void Dispose() => Dispose(true);

        #endregion
    }
}
