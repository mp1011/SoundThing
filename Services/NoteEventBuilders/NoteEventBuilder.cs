using SoundThing.Extensions;
using SoundThing.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SoundThing.Services.NoteEventBuilders
{
    class NoteEventBuilder : IEnumerable<NoteEvent>
    {
        private double _time = 0;
        private readonly int _bpm;
        private Scale _scale;
        private NoteType _beatNote;
        private List<NoteEvent> _noteEvents = new List<NoteEvent>();

        public int Octave => _scale.Root.Octave;
        public NoteEventBuilder(int bpm, NoteType beatNote, Scale scale)
        {
            _bpm = bpm;
            _beatNote = beatNote;
            _scale = scale;
        }

        public NoteEventBuilder(NoteEventBuilder cloneSource, Func<NoteEvent,NoteEvent> transform)
        {
            _bpm = cloneSource._bpm;
            _beatNote = cloneSource._beatNote;
            _time = cloneSource._time;

            _noteEvents = cloneSource._noteEvents
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
            => AddEvents(noteType, drumParts.Select(ToNoteInfo).ToArray());
        public NoteEventBuilder Add(NoteType noteType, params int[] notes)
           => AddEvents(noteType, notes.Select(ToNoteInfo).ToArray());

        public NoteEventBuilder Add(string notes)
        {
            foreach (var note in notes.Split(' '))
            {
                bool dotted = note.EndsWith('.');

                var num = int.Parse(note.Substring(0, 1));

                if(dotted)
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
                    switch(note[1])
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

        private NoteEventBuilder AddEvents(NoteType noteType, params NoteInfo[] notes)
        {
            foreach (var note in notes)
            {
                var duration = noteType.GetDuration(_beatNote, _bpm);
                _noteEvents.Add(
                    new NoteEvent(
                        new PlayedNoteInfo(
                            noteInfo: note,
                            duration: duration),
                        startTime: _time));
                _time += duration;
            }

            return this;
        }

        public NoteEventBuilder AddGroup(NoteType noteType, params DrumPart[] drums)
            => AddEventGroup(noteType, drums.Select(ToNoteInfo).ToArray());
        public NoteEventBuilder AddChord(NoteType noteType, params int[] notes)
           => AddEventGroup(noteType, notes.Select(ToNoteInfo).ToArray());

        public NoteEventBuilder AddChord(NoteType noteType, Chord chord)
            => AddEventGroup(noteType, chord.Notes.ToArray());

        public NoteEventBuilder AddChords(NoteType noteType, params int[] chordNumbers)
        {
            foreach(var number in chordNumbers)            
                AddEventGroup(noteType, _scale.GetChord(number));
            
            return this;
        }

        public NoteEventBuilder AddArpeggio(NoteType noteType, ArpeggioStyle style, params int[] chordNumbers)
        {
            foreach(var chordNumber in chordNumbers)
            {
                var chordNotes = _scale.GetChord(chordNumber);
                AddArpeggio(noteType, style, chordNotes);
            }
            return this;
        }

        public NoteEventBuilder AddArpeggio(NoteType noteType, ArpeggioStyle style, Chord chord)
        {
            var chordNotes = chord.Notes.ToArray();
            AddArpeggio(noteType, style, chordNotes);
            
            return this;
        }

        private NoteEventBuilder AddArpeggio(NoteType noteType, ArpeggioStyle style, NoteInfo[] notes)
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

            for(int i = 0; i < length;i++)
            {
                AddEvents(noteType, notes[index]);
                index += direction;
                if(index < 0 || index == notes.Length)
                {
                    direction *= -1;
                    index += direction*2;
                }
            }

            return this;
        }

        public NoteEventBuilder AddSection(NoteEventBuilder section)
        {
            foreach(var e in section._noteEvents)
            {

                _noteEvents.Add(e.ChangeStartTime(_time).ChangeOctave(_scale.Root.Octave));
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

        private NoteEventBuilder AddEventGroup(NoteType noteType, params NoteInfo[] notes)
        {
            var duration = noteType.GetDuration(_beatNote, _bpm);

            foreach (var note in notes)
            {
                _noteEvents.Add(
                    new NoteEvent(
                        new PlayedNoteInfo(
                            noteInfo: note,
                            duration: duration),
                        startTime: _time));
            }

            _time += duration;

            return this;
        }

        public NoteEventBuilder Repeat(int times)
        {
            var block = _noteEvents.ToArray();
            var blockDuration = block.Max(p => p.SampleIndexEnd) - block.Min(p => p.SampleIndexStart);
            var shiftAmount = blockDuration;

            while (times-- > 0)
            {
                _noteEvents.AddRange(block.Select(p => p.ShiftStart(shiftAmount)));
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

        private NoteInfo ToNoteInfo(int number) => _scale.GetNote(number);

        public IEnumerator<NoteEvent> GetEnumerator()
        {
            return ((IEnumerable<NoteEvent>)_noteEvents).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_noteEvents).GetEnumerator();
        }


    }
}
