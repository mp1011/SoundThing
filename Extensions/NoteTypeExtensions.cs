using SoundThing.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoundThing.Extensions
{
    static class NoteTypeExtensions
    {
        public static double GetDuration(this NoteType noteType, NoteType beatNote, int bpm)
        {
            var secondsPerBeat = 60.0 / bpm;

            var noteFraction = 1.0 / (int)noteType;
            var beatNoteFraction = 1.0 / (int)beatNote;

            var ratio = noteFraction / beatNoteFraction;
            return secondsPerBeat * ratio;
        }
    }
}
