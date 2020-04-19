namespace Organya
{
    /// <summary>
    /// An event, or "note", in an Organya song.
    /// </summary>
    public struct OrganyaEvent
    {
        /// <summary>
        /// The position of the event, in clicks.
        /// </summary>
        public uint EventPosition { get; set; }

        /// <summary>
        /// The pitch of the event.
        /// </summary>
        /// <remarks>
        /// 0 is the lowest possible pitch.
        /// 45 is A440.
        /// 95 is the highest possible pitch.
        /// 255 signifies that no change should be made from the previous event.
        /// Each step represents a single semitone. 0 - 11 is the first octave;
        /// 12 - 23 is the second octave; and so on.
        /// </remarks>
        public byte Pitch { get; set; }

        /// <summary>
        /// The duration of the event, in clicks.
        /// </summary>
        public byte Duration { get; set; }

        /// <summary>
        /// The volume of the event.
        /// </summary>
        /// <remarks>
        /// 0 is a silent event.
        /// 200 is the default in ORGMaker.
        /// 254 is the loudest event.
        /// 255 signifies that no change should be made from the previous event.
        /// </remarks>
        public byte Volume { get; set; }

        /// <summary>
        /// The pan of the event.
        /// </summary>
        /// <remarks>
        /// 0 is fully panned to the left.
        /// 6 is panned to the center.
        /// 12 is fully panned to the right.
        /// 255 signifies that no change should be made from the previous event.
        /// </remarks>
        public byte Pan { get; set; }
    }
}
