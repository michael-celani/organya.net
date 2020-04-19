using System.Collections.Generic;
using System.Linq;
using RangeTree;

namespace Organya.Converter
{
    public static class OrganyaConverter
    {
        public static IEnumerable<OrganyaNote> Extract(Organya organya, OrganyaInstrument[] instruments)
        {
            return organya.Tracks.SelectMany(track => ExtractNotes(track, instruments));
        }

        private static IEnumerable<OrganyaNote> ExtractNotes(OrganyaTrack track, OrganyaInstrument[] instruments)
        {
            return track.Events.SplitBefore(IsNoteStart).Select(noteEvents => ExtractNote(noteEvents, track, instruments));
        }

        private static OrganyaNote ExtractNote(IEnumerable<OrganyaEvent> noteEvents, OrganyaTrack track, OrganyaInstrument[] instruments)
        {
            OrganyaEvent startingEvent = noteEvents.First();
            OrganyaEvent endingEvent = noteEvents.Last();

            IRangeTree<uint, float> volumeTree = new RangeTree<uint, float>();

            OrganyaEvent prevEvent = startingEvent;
            foreach (OrganyaEvent orgEvent in noteEvents.Skip(1))
            {
                if (orgEvent.Volume == 255)
                {
                    continue;
                }

                // Volume has changed:
                volumeTree.Add(prevEvent.EventPosition, orgEvent.EventPosition - 1, prevEvent.Volume / 254f);
                prevEvent = orgEvent;
            }

            if (endingEvent.EventPosition > startingEvent.EventPosition + startingEvent.Duration)
            {
                startingEvent.Duration = (byte) (endingEvent.EventPosition - startingEvent.EventPosition);
            }

            volumeTree.Add(prevEvent.EventPosition, uint.MaxValue, prevEvent.Volume / 254f);
            volumeTree.Add(0, startingEvent.EventPosition, startingEvent.Volume / 254f);

            IRangeTree<uint, float> panTree = new RangeTree<uint, float>();

            prevEvent = startingEvent;
            foreach (OrganyaEvent orgEvent in noteEvents.Skip(1))
            {
                if (orgEvent.Pan == 255)
                {
                    continue;
                }

                var panVal = prevEvent.Pan / 12.0f;

                // Pan has changed:
                panTree.Add(prevEvent.EventPosition, orgEvent.EventPosition - 1, panVal);
                prevEvent = orgEvent;
            }

            if (endingEvent.EventPosition > startingEvent.EventPosition + startingEvent.Duration)
            {
                startingEvent.Duration = (byte)(endingEvent.EventPosition - startingEvent.EventPosition);
            }

            panTree.Add(prevEvent.EventPosition, uint.MaxValue, (prevEvent.Pan) / 12.0f);
            panTree.Add(0, startingEvent.EventPosition, (prevEvent.Pan) / 12.0f);


            return new OrganyaNote
            {
                NotePosition = startingEvent.EventPosition,
                NoteDuration = startingEvent.Duration,
                Frequency = track.Frequency,
                Pitch = startingEvent.Pitch,
                Instrument = instruments[track.Instrument],
                Volume = volumeTree,
                Pan = panTree
            };
        }

        private static bool IsNoteStart(OrganyaEvent ev) => ev.Pitch != 255; 
    }
}
