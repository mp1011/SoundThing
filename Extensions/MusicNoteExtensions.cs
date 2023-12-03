using SoundThing.Models;
using System;
using System.Collections.Generic;

namespace SoundThing.Extensions
{
    public static class MusicNoteExtensions
    {
        public static ushort GetFrequency(this MusicNote note, int octave)
        {
            int semitone = (int)note;
            var thisOctave = 110 * Math.Pow(2, octave - 2);
            var frequency = thisOctave * Math.Pow(2, (double)semitone / 12.0);
            return (ushort)frequency;
        }

        public static (MusicNote, int) GetNote(this double frequency) =>
            ((ushort)frequency).GetNote();

        private static Dictionary<ushort, (MusicNote, int)> _frequencyToNote = new Dictionary<ushort, (MusicNote, int)>();

        public static (MusicNote, int) GetNote(this ushort frequency)
        {
            (MusicNote, int) cachedResult;
            if (_frequencyToNote.TryGetValue(frequency, out cachedResult))
                return cachedResult;

            double leastDifference = double.MaxValue;
            MusicNote bestMatch = MusicNote.A;
            int bestOctave = 0;

            int octave = 0;
            while (octave < Constants.MaxOctave)
            {
                foreach (MusicNote note in Enum.GetValues(typeof(MusicNote)))
                {
                    var noteFrequency = note.GetFrequency(octave);
                    var difference = Math.Abs(noteFrequency - frequency);
                    if (difference < leastDifference)
                    {
                        leastDifference = difference;
                        bestMatch = note;
                        bestOctave = octave;
                    }
                }
                octave += 1;
            }

            _frequencyToNote.Add(frequency, (bestMatch, bestOctave));
            return (bestMatch, bestOctave);
        }
    }

}
