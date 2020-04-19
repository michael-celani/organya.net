using RangeTree;

namespace Organya.Converter
{
    /// <summary>
    /// Represents a note in an Organya file.
    /// </summary>
    public class OrganyaNote
    {
        /// <summary>
        /// The starting position of the note, in clicks.
        /// </summary>
        public uint NotePosition { get; set; }

        /// <summary>
        /// The amount of additional clicks the note is held for.
        /// </summary>
        public uint NoteDuration { get; set; }

        /// <summary>
        /// The frequency offset of the note.
        /// </summary>
        public uint Frequency { get; set; }

        /// <summary>
        /// The pitch of the note, ranged from 0 to 95.
        /// </summary>
        public byte Pitch { get; set; }

        /// <summary>
        /// The instrument used to play the note.
        /// </summary>
        public OrganyaInstrument Instrument { get; set; }

        /// <summary>
        /// The volume at the given steps, normalized from 0.0 to 1.0.
        /// </summary>
        public IRangeTree<uint, float> Volume { get; set; }

        /// <summary>
        /// The pan at the given steps, normalized from 0.0 to 1.0.
        /// </summary>
        public IRangeTree<uint, float> Pan { get; set; }
    }
}
