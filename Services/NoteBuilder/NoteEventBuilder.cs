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

        public NoteEventBuilder Add(string notes)
        {
            foreach (var note in notes.Split(' '))
            {
                bool dotted = note.EndsWith('.');

                var num = int.Parse(note.Substring(0, 1));

                if (dotted)
                {
                    switch (note[1])
                    {
                        case 's':
                            Add(NoteType.DottedSixteenth, num);
                            break;
                        case 'e':
                            Add(NoteType.DottedEighth, num);
                            break;
                        case 'q':
                            Add(NoteType.DottedQuarter, num);
                            break;
                        case 'h':
                            Add(NoteType.DottedHalf, num);
                            break;
                        default:
                            Add(NoteType.DottedWhole, num);
                            break;
                    };
                }
                else
                {
                    switch (note[1])
                    {
                        case 's':
                            Add(NoteType.Sixteenth, num);
                            break;
                        case 'e':
                            Add(NoteType.Eighth, num);
                            break;
                        case 'q':
                            Add(NoteType.Quarter, num);
                            break;
                        case 'h':
                            Add(NoteType.Half, num);
                            break;
                        default:
                            Add(NoteType.Whole, num);
                            break;
                    };
                }
            }

            return this;
        }

        public NoteEventBuilder AddEighths(params int[] notes) => Add(NoteType.Eighth, notes);
        public NoteEventBuilder AddQuarters(params int[] notes) => Add(NoteType.Quarter, notes);
        public NoteEventBuilder AddHalves(params int[] notes) => Add(NoteType.Half, notes);
        public NoteEventBuilder AddWholes(params int[] notes) => Add(NoteType.Whole, notes);

        private NoteEventBuilder AddEvents(NoteType noteType, EventAction action, params int[] arguments)
        {
            foreach (var arg in arguments)
            {
                var duration = noteType.GetDuration(_beatNote, _bpm);
                _blocks.Add(
                    new EventBlock(
                        start: _time,
                        duration: duration,
                        argument: arg,
                        scale: _scale,
                        action: action));
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
                _blocks.Add(e.ChangeStartTime(_time).ChangeScale(_scale));
                _time += e.Duration;
            }
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

        private NoteEventBuilder AddEventGroup(NoteType noteType, EventAction action, params int[] arguments)
        {
            var duration = noteType.GetDuration(_beatNote, _bpm);

            foreach (var arg in arguments)
            {
                _blocks.Add(
                    new EventBlock(
                        start: _time,
                        duration: duration,
                        argument: arg,
                        scale: _scale,
                        action: action));
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

        private NoteInfo ToNoteInfo(DrumPart p) => new NoteInfo((MusicNote)p, _scale.Root.Octave, _scale.Root.VolumePercent);

        private IEnumerable<NoteEvent> Build()
            => _blocks
                .OrderBy(p => p.Start)
                .Select(GenerateEvent);

        private NoteEvent GenerateEvent(EventBlock block)
            => block.Action switch
            {
                EventAction.PlayScaleNote => new NoteEvent(
                            new PlayedNoteInfo(
                                noteInfo: block.Scale.GetNote(block.Argument),
                                duration: block.Duration),
                            startTime: block.Start),

                EventAction.PlayDrumPart => new NoteEvent(
                            new PlayedNoteInfo(
                                noteInfo: ToNoteInfo((DrumPart)block.Argument),
                                duration: block.Duration),
                            startTime: block.Start),

                _ => throw new Exception($"Unexpected event type {block.Action}")
            };

        IEnumerator<NoteEvent> IEnumerable<NoteEvent>.GetEnumerator()
            => Build().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => Build().GetEnumerator();
    }
}
