using SoundThing.Extensions;
using SoundThing.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SoundThing.Services.NoteBuilder
{
    class NoteEventBuilder : IEnumerable<NoteEvent>
    {
        private double _time = 0;
        private readonly int _bpm;
        private Scale _scale;
        private NoteType _beatNote;
        private List<EventBlock> _blocks = new List<EventBlock>();

        public int Octave => _scale.Root.Octave;
        public NoteEventBuilder(int bpm, NoteType beatNote, Scale scale)
        {
            _bpm = bpm;
            _beatNote = beatNote;
            _scale = scale;
        }

        public NoteEventBuilder(NoteEventBuilder cloneSource, Func<EventBlock, EventBlock> transform)
        {
            _bpm = cloneSource._bpm;
            _beatNote = cloneSource._beatNote;
            _time = cloneSource._time;
            _scale = cloneSource._scale.Copy();

            _blocks = cloneSource._blocks
                .Select(transform)
                .ToList();
        }

        public NoteEventBuilder NewSection()
        {
            var newBuilder = new NoteEventBuilder(_bpm, _beatNote, _scale);
            newBuilder._time = _time;
            return newBuilder;
        }

        public NoteEventBuilder Add(NoteType noteType, params DrumPart[] drumParts)
            => AddEvents(noteType, EventAction.PlayDrumPart, drumParts.Cast<int>().ToArray());
        public NoteEventBuilder Add(NoteType noteType, params int[] notes)
           => AddEvents(noteType, EventAction.PlayScaleNote, notes);

        public NoteEventBuilder Add(string notes) => Add(EventAction.PlayScaleNote, notes);
        public NoteEventBuilder Add(EventAction action, string notes)
            => NoteParser.AddEvents(this, action, notes);

        public NoteEventBuilder AddEighths(params int[] notes) => Add(NoteType.Eighth, notes);
        public NoteEventBuilder AddQuarters(params int[] notes) => Add(NoteType.Quarter, notes);
        public NoteEventBuilder AddHalves(params int[] notes) => Add(NoteType.Half, notes);
        public NoteEventBuilder AddWholes(params int[] notes) => Add(NoteType.Whole, notes);

        public NoteEventBuilder AddEvents(NoteType noteType, EventAction action, params int[] arguments)
        {
            foreach (var arg in arguments)
            {
                var duration = noteType.GetDuration(_beatNote, _bpm);
                _blocks.Add(
                    EventBlock.Create(
                        action: action,
                        start: _time,
                        duration: duration,
                        argument: arg,
                        adjust: 0,
                        scale: _scale));
                _time += duration;
            }

            return this;
        }

        public NoteEventBuilder AddGroup(NoteType noteType, params DrumPart[] drums)
            => AddEventGroup(noteType, EventAction.PlayDrumPart, drums.Cast<int>().ToArray());
        public NoteEventBuilder AddChord(NoteType noteType, params int[] notes)
           => AddEventGroup(noteType, EventAction.PlayScaleNote, notes);

        public NoteEventBuilder AddChord(NoteType noteType, Chord chord)
            => AddEventGroup(noteType, EventAction.PlayScaleNote, chord.NoteIndices.ToArray());

        public NoteEventBuilder AddChords(NoteType noteType, params int[] chordNumbers)
        {
            foreach (var number in chordNumbers)
                AddEventGroup(noteType, EventAction.PlayScaleNote, _scale.GetChordIndices(number).ToArray());

            return this;
        }

        public NoteEventBuilder AddChordArpeggio(NoteType noteType, ArpeggioStyle style, params int[] chordNumbers)
        {
            foreach (var chordNumber in chordNumbers)
            {
                var chordNotes = _scale.GetChordIndices(chordNumber);
                AddArpeggio(noteType, style, chordNotes);
            }
            return this;
        }

        public NoteEventBuilder AddArpeggio(NoteType noteType, ArpeggioStyle style, Chord chord)
        {
            var chordNotes = chord.NoteIndices.ToArray();
            AddArpeggio(noteType, style, chordNotes);

            return this;
        }

        private NoteEventBuilder AddArpeggio(NoteType noteType, ArpeggioStyle style, int[] notes)
        {
            int direction = style switch
            {
                ArpeggioStyle.RiseAndFall => 1,
                ArpeggioStyle.Rising => 1,
                _ => -1
            };

            int index = style switch
            {
                ArpeggioStyle.RiseAndFall => 0,
                ArpeggioStyle.Rising => 0,
                _ => notes.Length - 1
            };

            int length = style switch
            {
                ArpeggioStyle.RiseAndFall => (notes.Count() * 2) - 1,
                ArpeggioStyle.FallAndRise => (notes.Count() * 2) - 1,
                _ => notes.Length
            };

            for (int i = 0; i < length; i++)
            {
                AddEvents(noteType, EventAction.PlayScaleNote, notes[index]);
                index += direction;
                if (index < 0 || index == notes.Length)
                {
                    direction *= -1;
                    index += direction * 2;
                }
            }

            return this;
        }

        public NoteEventBuilder AddSection(NoteEventBuilder section)
        {
            foreach (var e in section._blocks)
            {
                _blocks.Add(e.AddStartTime(_time).ChangeScale(_scale));
            }

            var sectionDuration = section._blocks.Max(p => p.End) - section._blocks.Min(p => p.Start);
            _time += sectionDuration;

            return this;
        }

        public NoteEventBuilder AddRounds(int voices, int beatOffset, NoteEventBuilder melody,
            Func<NoteEventBuilder, int, NoteEventBuilder> changePart)
        {
            var startTime = _time;

            AddSection(melody);

            int part = 0;
            var newBuilder = changePart(this, part);
            while (--voices > 0)
            {
                newBuilder._time = startTime + beatOffset * _beatNote.GetDuration(_beatNote, _bpm);
                startTime = _time;
                newBuilder.AddSection(melody);
                part++;
                newBuilder = changePart(this, part);
            }

            return this;
        }

        public NoteEventBuilder AddEventGroup(NoteType noteType, EventAction action,
            IEnumerable<ScaleNoteIndex> notes)
        {
            var duration = noteType.GetDuration(_beatNote, _bpm);

            foreach (var note in notes)
            {
                Scale noteScale = _scale;
              
                _blocks.Add(EventBlock.Create(
                        action: action,
                        start: _time,
                        duration: duration,
                        argument: note.Value,
                        adjust: (int)note.Accidental,
                        scale: noteScale));
            }

            _time += duration;

            return this;

        }

        public NoteEventBuilder AddEventGroup(NoteType noteType, EventAction action, params int[] arguments)
        {
            var duration = noteType.GetDuration(_beatNote, _bpm);

            foreach (var arg in arguments)
            {
                _blocks.Add(EventBlock.Create(
                        action: action,
                        start: _time,
                        duration: duration,
                        argument: arg,
                        adjust: 0,
                        scale: _scale));
            }

            _time += duration;

            return this;
        }

        public NoteEventBuilder Repeat(int times)
        {
            var block = _blocks.ToArray();
            var blockDuration = block.Max(p => p.End) - block.Min(p => p.Start);
            var shiftAmount = blockDuration;

            while (times-- > 0)
            {
                _blocks.AddRange(block.Select(p => p.AddStartTime(shiftAmount)));
                shiftAmount += blockDuration;
                _time += (blockDuration / (double)Constants.SamplesPerSecond);
            }

            return this;
        }
        public NoteEventBuilder SetOctave(int octave)
        {
            _scale = _scale.ChangeOctave(octave);
            return this;
        }
        public NoteEventBuilder SetScale(Scale scale)
        {
            _scale = scale;
            return this;
        }


        private IEnumerable<NoteEvent> Build()
        {
            var modifiers = _blocks
                .OfType<IModifierBlock>()
                .ToArray();

            var generators = _blocks
                .OfType<IGeneratorBlock>()
                .ToArray();

            var splicePoints = modifiers
                .SelectMany(p => new[] { p.Start, p.End })
                .OrderBy(p => p)
                .Distinct()
                .ToArray();

            modifiers = modifiers
                .SelectMany(p => p.SpliceAt(splicePoints))
                .OrderBy(p => p.Start)
                .OfType<IModifierBlock>()
                .ToArray();

            generators = generators
              .SelectMany(p => p.SpliceAt(splicePoints))
              .OrderBy(p => p.Start)
              .OfType<IGeneratorBlock>()
              .ToArray();

            return generators.SelectMany(generator =>
            {
                var modified = modifiers
                    .Where(p => p.Start == generator.Start)
                    .Aggregate(generator, (g, m) => m.Modify(g));

                bool keepOriginal = (modified == null ||
                    modifiers.Any(p => p.Start == generator.Start && !p.ReplaceOriginal));

                if (keepOriginal && modified == null)
                    return new[] { generator.CreateEvent() };
                else if (keepOriginal)
                {
                    return new[] {
                        generator.CreateEvent(),
                        modified.CreateEvent()
                    };
                }
                else if (modified != null)
                {
                    return new[] {
                        modified.CreateEvent()
                    };
                }
                else return Array.Empty<NoteEvent>();
            });

        }

        IEnumerator<NoteEvent> IEnumerable<NoteEvent>.GetEnumerator()
            => Build().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => Build().GetEnumerator();

        public static NoteEventBuilder operator +(NoteEventBuilder nb1, NoteEventBuilder nb2)
        {
            var newBuilder = new NoteEventBuilder(nb1, p => p.Copy());
            newBuilder.AddSection(nb2);
            return newBuilder;
        }

        public static NoteEventBuilder operator *(NoteEventBuilder nb1, NoteEventBuilder nb2)
        {
            var newBuilder = new NoteEventBuilder(nb1, p => p.Copy());

            var start = newBuilder._blocks.Min(p => p.Start);
            var end = newBuilder._blocks.Max(p => p.End);

            var time = start;
            while (time < end)
            {
                foreach (var block in nb2._blocks)
                {
                    newBuilder._blocks.Add(block.ChangeStartTime(time));
                    time += block.Duration;
                }
            }

            newBuilder._blocks.Sort();

            return newBuilder;
        }

        public static NoteEventBuilder operator *(NoteEventBuilder nb1, int times)
        {
            var newBuilder = new NoteEventBuilder(nb1, p => p.Copy());
            return newBuilder.Repeat(times - 1);
        }
    }
}
