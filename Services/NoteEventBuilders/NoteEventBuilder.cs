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
