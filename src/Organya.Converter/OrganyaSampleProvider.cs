using System;
using System.Collections.Generic;
using System.Linq;
using NAudio.Wave;
using RangeTree;

namespace Organya.Converter
{
    public class OrganyaSampleProvider : ISampleProvider
    {
        /// <inheritdoc />
        public WaveFormat WaveFormat { get; } = WaveFormat.CreateIeeeFloatWaveFormat(41100, 2);

        private Organya Organya { get; }

        private IRangeTree<uint, OrganyaNote> NoteTree { get; }

        private long SamplesPerClick { get; }

        private long CurrentSample { get; set; }

        private bool RightChannel { get; set; }

        private uint CurrentClick { get; set; }

        /// <summary>
        /// The point rate of notes.
        /// </summary>
        public static readonly int[] PointsPerSecond = new[]
        {
            33408, // C
            35584, // C#
            37632, // D
            39808, // D#
            42112, // E
            44672, // F
            47488, // F#
            50048, // G
            52992, // G#
            56320, // A
            59648, // A#
            63232  // B
        };

        public OrganyaSampleProvider(Organya organya, OrganyaInstrument[] instruments)
        {
            Organya = organya ?? throw new ArgumentNullException(nameof(organya));
            SamplesPerClick = (long) (WaveFormat.SampleRate * Organya.ClickLength.TotalSeconds);

            IEnumerable<OrganyaNote> notes = OrganyaConverter.Extract(organya, instruments);
            NoteTree = notes.ToRangeTree(note => note.NotePosition, note => note.NotePosition + note.NoteDuration - 1);
        }


        public int Read(float[] buffer, int offset, int count)
        {
            int read = 0;

            for (int i = 0; i < count; i++)
            {
                if (SamplesPerClick * Organya.LoopEnd <= CurrentSample)
                {
                    return read;
                }

                buffer[i + offset] = NoteTree.Query(CurrentClick).Aggregate(0.0f, (sample, note) => sample + GetNoteSample(note, CurrentSample));

                var timeToNext = SamplesPerClick - (CurrentSample % SamplesPerClick);
                var cutoff = 500f;

                if (timeToNext < cutoff)
                {
                    var next = NoteTree.Query(CurrentClick + 1).Aggregate(0.0f, (sample, note) => sample + GetNoteSample(note, CurrentSample));

                    buffer[i + offset] = buffer[i + offset] * timeToNext / cutoff + next * ((cutoff - timeToNext) / cutoff);
                }

                if (!RightChannel)
                {
                    RightChannel = true;
                }
                else
                {
                    RightChannel = false;
                    CurrentSample++;
                }

                if (!RightChannel & CurrentSample % SamplesPerClick == 0)
                {
                    CurrentClick++;
                }

                read++;
            }

            return read;
        }

        private long GetNoteStartingSample(OrganyaNote note)
        {
            return note.NotePosition * SamplesPerClick;
        }

        public float GetNoteSample(OrganyaNote note, long sample)
        {
            int octave = Math.DivRem(note.Pitch, 12, out int chroma);
            long startingSample = GetNoteStartingSample(note);
            long startingOffset = sample - startingSample;

            long pointRate = PointsPerSecond[chroma] + (Convert.ToInt32(note.Frequency) - 1000);

            double samplesRate = pointRate / (double)WaveFormat.SampleRate;
            double sampleOffset = (samplesRate * startingOffset);

            long point = GetPointIndex(octave, (long)sampleOffset);
            float waveValue = Convert.ToSingle(note.Instrument.Samples[point]);
            float sampleValue = map(waveValue, sbyte.MinValue, sbyte.MaxValue, -1.0f, 1.0f);

            float waveValue2 = Convert.ToSingle(note.Instrument.Samples[(point + 1) % 256]);
            float sampleValue2 = map(waveValue2, sbyte.MinValue, sbyte.MaxValue, -1.0f, 1.0f);

            var percentNext = (float)(sampleOffset - Math.Truncate(sampleOffset));

            sampleValue = lerp(sampleValue, sampleValue2, percentNext);

            float volume = (float)Math.Pow(10, note.Volume.Query(CurrentClick).First() - 1.0);

            var pan = note.Pan.Query(CurrentClick).First();

            if (RightChannel & pan < 0.5)
            {
                volume *= (float)Math.Pow(20, 2 * pan - 1);
            }
            else if (!RightChannel & pan > 0.5)
            {
                volume *= (float)Math.Pow(20, 1 - 2 * pan);
            }

            return volume * sampleValue;
        }

        private static float map(float x, float in_min, float in_max, float out_min, float out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        float lerp(float v0, float v1, float t)
        {
            return (1 - t) * v0 + t * v1;
        }

        public static long GetPointIndex(int octave, long point)
        {
            var index = octave switch
            {
                0 => point >> 2,
                1 => point >> 1,
                2 => (point - point % 2),
                3 => (point - point % 2) << 1,
                4 => (point - point % 2) << 2,
                5 => (point - point % 2) << 3,
                6 => (point - point % 2) << 4,
                7 => (point - point % 2) << 5,
                _ => throw new ArgumentOutOfRangeException(nameof(octave), "Octave is out of range.")
            } % 256;

            if (index < 0)
            {
                index += 256;
            }

            return index;
        }
    }
}
