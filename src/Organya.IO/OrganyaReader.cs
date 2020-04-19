using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Organya.IO
{
    /// <summary>
    /// A stream reader that converts a <see cref="Stream"/> into instrument data.
    /// </summary>
    public class OrganyaReader : IDisposable
    {
        /// <summary>
        /// The stream reader for the internal stream.
        /// </summary>
        private BinaryReader StreamReader { get; }

        /// <summary>
        /// Constructs a new <see cref="OrganyaReader"/>.
        /// </summary>
        /// <param name="orgStream">The stream containing the data.</param>
        /// <exception cref="ArgumentNullException">
        /// orgStream is null.
        /// </exception>
        public OrganyaReader(Stream orgStream) : this(orgStream, false) { }

        /// <summary>
        /// Constructs a new <see cref="OrganyaReader"/>.
        /// </summary>
        /// <param name="orgStream">The stream containing the data.</param>
        /// <param name="leaveOpen">
        /// If true, do not dispose the underlying stream when this reader is
        /// disposed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// orgStream is null.
        /// </exception>
        public OrganyaReader(Stream orgStream, bool leaveOpen)
        {
            if (orgStream == null)
            {
                throw new ArgumentNullException(nameof(orgStream));
            }

            StreamReader = new BinaryReader(orgStream, Encoding.ASCII, leaveOpen);
        }

        /// <summary>
        /// Reads <see cref="Organya"/>s from the stream.
        /// </summary>
        /// <returns>The <see cref="Organya"/>s from the stream.</returns>
        public Organya Read()
        {
            OrganyaVersion version = ReadOrganyaVersion();
            TimeSpan clickLength = ReadClickLength();
            byte beatsPerMeasure = StreamReader.ReadByte();
            byte clicksPerBeat = StreamReader.ReadByte();
            uint loopStart = StreamReader.ReadUInt32();
            uint loopEnd = StreamReader.ReadUInt32();
            IEnumerable<int> trackNumbers = Enumerable.Range(0, 16);
            OrganyaTrack[] tracks = trackNumbers.Select(ReadTrack).ToArray();

            foreach (OrganyaTrack track in tracks)
            {
                ReadTrackEvents(track);
            }

            return new Organya(version, clickLength, beatsPerMeasure, clicksPerBeat, loopStart, loopEnd, tracks);
        }

        /// <summary>
        /// Reads the Organya version from the stream.
        /// </summary>
        /// <returns>The <see cref="OrganyaVersion"/> for the song.</returns>
        /// <exception cref="ArgumentException">
        /// The version in the stream is invalid.
        /// </exception>
        private OrganyaVersion ReadOrganyaVersion()
        {
            char[] versionChars = StreamReader.ReadChars(6);
            string versionString = new string(versionChars);

            return versionString switch
            {
                "Org-02" => OrganyaVersion.Org02,
                "Org-03" => OrganyaVersion.Org03,
                _ => throw new ArgumentException("The Organya version is not supported.")
            };
        }

        /// <summary>
        /// Reads the length of a click from the stream.
        /// </summary>
        /// <returns>The length of a click for the song.</returns>
        private TimeSpan ReadClickLength()
        {
            ushort clickLength = StreamReader.ReadUInt16();

            return TimeSpan.FromMilliseconds(clickLength);
        }

        /// <summary>
        /// Reads track data from the stream.
        /// </summary>
        /// <returns>The track data from the stream.</returns>
        private OrganyaTrack ReadTrack(int trackNumber)
        {
            ushort trackFrequency = StreamReader.ReadUInt16();
            byte trackInstrument = StreamReader.ReadByte();
            bool trackPi = StreamReader.ReadBoolean();
            ushort trackEvents = StreamReader.ReadUInt16();

            return new OrganyaTrack(trackFrequency, trackInstrument, trackPi, new OrganyaEvent[trackEvents]);
        }

        /// <summary>
        /// Reads track events into a track.
        /// </summary>
        /// <param name="track">The track to read events into.</param>
        private void ReadTrackEvents(OrganyaTrack track)
        {
            for (int i = 0; i < track.Events.Length; i++)
            {
                track.Events[i].EventPosition = StreamReader.ReadUInt32();
            }

            for (int i = 0; i < track.Events.Length; i++)
            {
                track.Events[i].Pitch = StreamReader.ReadByte();
            }

            for (int i = 0; i < track.Events.Length; i++)
            {
                track.Events[i].Duration = StreamReader.ReadByte();
            }

            for (int i = 0; i < track.Events.Length; i++)
            {
                track.Events[i].Volume = StreamReader.ReadByte();
            }

            for (int i = 0; i < track.Events.Length; i++)
            {
                track.Events[i].Pan = StreamReader.ReadByte();
            }
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
