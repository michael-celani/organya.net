using System;

namespace Organya
{
    /// <summary>
    /// An Organya, which represents a piece of music in the Studio Pixel
    /// game Cave Story.
    /// </summary>
    public class Organya
    {
        /// <summary>
        /// The version of this Organaya.
        /// </summary>
        public OrganyaVersion Version { get; }

        /// <summary>
        /// The length of a click.
        /// </summary>
        public TimeSpan ClickLength { get; }

        /// <summary>
        /// The number of beats in a measure.
        /// </summary>
        public byte BeatsPerMeasure { get; }

        /// <summary>
        /// The number of clicks in a beat.
        /// </summary>
        public byte ClicksPerBeat { get; }

        /// <summary>
        /// The position of the loop start, in clicks.
        /// </summary>
        public uint LoopStart { get; }

        /// <summary>
        /// The position of the loop end, in clicks.
        /// </summary>
        public uint LoopEnd { get; }

        /// <summary>
        /// The tracks in the song.
        /// </summary>
        public OrganyaTrack[] Tracks { get; }

        /// <summary>
        /// Constructs a new <see cref="Organya"/>.
        /// </summary>
        /// <param name="version">The version of the Organya.</param>
        /// <param name="clickLength">The time one click lasts.</param>
        /// <param name="beatsPerMeasure">The amount of beats in a measure.</param>
        /// <param name="clicksPerBeat">The amount of clicks in a beat.</param>
        /// <param name="start">The click the track's loop point begins.</param>
        /// <param name="end">The click the track's loop point ends.</param>
        /// <param name="tracks">The tracks included in this Organya.</param>
        public Organya(OrganyaVersion version, TimeSpan clickLength, byte beatsPerMeasure, byte clicksPerBeat, uint start, uint end, OrganyaTrack[] tracks)
        {
            Version = version;
            ClickLength = clickLength;
            BeatsPerMeasure = beatsPerMeasure;
            ClicksPerBeat = clicksPerBeat;
            LoopStart = start;
            LoopEnd = end;
            Tracks = tracks;
        }
    }
}
