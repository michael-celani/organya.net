using System;
using System.Collections.Generic;
using System.Linq;
using RangeTree;

namespace Organya.Converter
{
    public class OrganyaConverter
    {
        public IReadOnlyList<OrganyaInstrument> Instruments { get; }

        public OrganyaConverter(IEnumerable<OrganyaInstrument> instruments)
        {
            Instruments = instruments.ToList().AsReadOnly();
        }

        public IEnumerable<OrganyaNote> Extract(Organya organya)
        {
            return organya.Tracks.SelectMany(ExtractTrack);
        }

        private IEnumerable<OrganyaNote> ExtractTrack(OrganyaTrack track)
        {
            OrganyaNote ExtractTrackNotes(IEnumerable<OrganyaEvent> events)
            {
                return ExtractNotes(events, track);
            }

            return track.Events.SplitBefore(IsNoteStart).Select(ExtractTrackNotes);
        }

        private OrganyaNote ExtractNotes(IEnumerable<OrganyaEvent> noteEvents, OrganyaTrack track)
        {
            IList<OrganyaEvent> notes = noteEvents.ToList();

            OrganyaEvent firNote = notes[0];
            OrganyaEvent endNote = notes[^1];

            // It's possible to change the volume of a note after its duration has lapsed.
            // Percussion can do this to change the volume of a note mid-play without having
            // to select a range.
            if (endNote.EventPosition > firNote.EventPosition + firNote.Duration)
            {
                firNote.Duration = (byte) (endNote.EventPosition - firNote.EventPosition);
            }

            return new OrganyaNote
            {
                NotePosition = firNote.EventPosition,
                NoteDuration = firNote.Duration,
                Frequency = track.Frequency,
                Pitch = firNote.Pitch,
                Instrument = Instruments[track.Instrument],
                Volume = ExtractTree(notes, IsVolumeChange, NormalizeVolume),
                Pan = ExtractTree(notes, IsPanChange, NormalizePan)
            };
        }

        private static IRangeTree<uint, float> ExtractTree(
            IList<OrganyaEvent> noteEvents,
            Func<OrganyaEvent, bool> changeFunc,
            Func<OrganyaEvent, float> normalizeFunc
        )
        {
            IRangeTree<uint, float> tree = new RangeTree<uint, float>();

            OrganyaEvent startingEvent = noteEvents[0];
            OrganyaEvent prevEvent = startingEvent;

            foreach (OrganyaEvent currEvent in noteEvents.Skip(1).Where(changeFunc))
            {
                tree.Add(prevEvent.EventPosition, currEvent.EventPosition - 1, normalizeFunc(prevEvent));
                prevEvent = currEvent;
            }


            tree.Add(0, startingEvent.EventPosition, normalizeFunc(startingEvent));
            tree.Add(prevEvent.EventPosition, uint.MaxValue, normalizeFunc(prevEvent));

            return tree;
        }

        private static bool IsNoteStart(OrganyaEvent ev) => ev.Pitch != 255;

        private static bool IsVolumeChange(OrganyaEvent ev) => ev.Volume != 255;

        private static bool IsPanChange(OrganyaEvent ev) => ev.Pan != 255;

        private static float NormalizeVolume(OrganyaEvent ev) => ev.Volume / 254f;

        private static float NormalizePan(OrganyaEvent ev) => ev.Pan / 12f;
    }
}
