namespace Organya
{
    /// <summary>
    /// A track in an Organya song.
    /// </summary>
    public class OrganyaTrack
    {
        /// <summary>
        /// The frequency of events in this track.
        /// </summary>
        public uint Frequency { get; }

        /// <summary>
        /// The instrument used by events in this track.
        /// </summary>
        public byte Instrument { get; }

        /// <summary>
        /// If Pi is checked for this track.
        /// </summary>
        public bool PiChecked { get; }

        /// <summary>
        /// The events in this track.
        /// </summary>
        public OrganyaEvent[] Events { get; }

        /// <summary>
        /// Constructs a new <see cref="OrganyaTrack"/>.
        /// </summary>
        /// <param name="frequency">The frequency of events in the track.</param>
        /// <param name="instrument">The identifier for the instrument used in the track.</param>
        /// <param name="pi">Whether pi-sampling is enabled.</param>
        /// <param name="events">The events in the track.</param>
        public OrganyaTrack(uint frequency, byte instrument, bool pi, OrganyaEvent[] events)
        {
            Frequency = frequency;
            Instrument = instrument;
            PiChecked = pi;
            Events = events;
        }
    }
}
