using SoundThing.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace SoundThing.Extensions
{
    static class NoteTypeExtensions
    {
        private static Dictionary<string, NoteType> _map;

        static NoteTypeExtensions()
        {
            _map = new Dictionary<string, NoteType>();

            foreach(NoteType value in Enum.GetValues(typeof(NoteType)))
            {
                var name = Enum.GetName(typeof(NoteType), value);
                var field = typeof(NoteType).GetField(name);
                var desc = field.GetCustomAttributes(false)
                                .OfType<DescriptionAttribute>()
                                .First();
                _map[desc.Description] = value;
            }
        }
        public static NoteType? TryParseNoteType(this char chr)
            => TryParseNoteType(chr.ToString());

        public static NoteType? TryParseNoteType(this string str)
        {
            NoteType noteType;
            if (_map.TryGetValue(str, out noteType))
                return noteType;
            else
                return null;
        }

        public static double GetDuration(this NoteType noteType, NoteType beatNote, int bpm)
        {
            var secondsPerBeat = 60.0 / bpm;

            var noteFraction = noteType.GetNoteFraction();
            var beatNoteFraction = beatNote.GetNoteFraction();

            var ratio = noteFraction / beatNoteFraction;
            return secondsPerBeat * ratio;
        }

        public static double GetNoteFraction(this NoteType noteType) =>
            noteType switch
            {
                NoteType.Whole => 1.0,
                NoteType.Half => 0.5,
                NoteType.Quarter => 0.25,
                NoteType.Eighth => 1.0 / 8.0,
                NoteType.Sixteenth => 1.0 / 16.0,
                NoteType.DottedWhole => NoteType.Whole.GetNoteFraction() * 1.5,
                NoteType.DottedHalf => NoteType.Half.GetNoteFraction() * 1.5,
                NoteType.DottedQuarter => NoteType.Quarter.GetNoteFraction() * 1.5,
                NoteType.DottedEighth => NoteType.Eighth.GetNoteFraction() * 1.5,
                NoteType.DottedSixteenth => NoteType.Sixteenth.GetNoteFraction() * 1.5,
                _ => 0.0
            };
    }
}
