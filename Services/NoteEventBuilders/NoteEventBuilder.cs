using SoundThing.Extensions;
using SoundThing.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SoundThing.Services.NoteEventBuilders
{
    abstract class NoteEventBuilder<T> : IEnumerable<NoteEvent>
    {
        private double _time = 0;
        private readonly int _bpm;
        private NoteType _beatNote;
        private List<NoteEvent> _noteEvents = new List<NoteEvent>();

        public NoteEventBuilder(int bpm, NoteType beatNote)
        {
            _bpm = bpm;
            _beatNote = beatNote;
        }

        public NoteEventBuilder(NoteEventBuilder<T> cloneSource, Func<NoteEvent,NoteEvent> transform)
        {
            _bpm = cloneSource._bpm;
            _beatNote = cloneSource._beatNote;
            _time = cloneSource._time;

            _noteEvents = cloneSource._noteEvents
                .Select(transform)
                .ToList();
        }

        protected void AddEvents(NoteType noteType, params T[] notes)
        {
            foreach (var note in notes)
            {
                var duration = noteType.GetDuration(_beatNote, _bpm);
                _noteEvents.Add(
                    new NoteEvent(
                        new PlayedNoteInfo(
                            noteInfo: CreateNoteInfo(note),
                            duration: duration),
                        startTime: _time));
                _time += duration;
            }
        }

        protected void AddEventGroup(NoteType noteType, params T[] notes)
        {
            var duration = noteType.GetDuration(_beatNote, _bpm);

            foreach (var note in notes)
            {
                _noteEvents.Add(
                    new NoteEvent(
                        new PlayedNoteInfo(
                            noteInfo: CreateNoteInfo(note),
                            duration: duration),
                        startTime: _time));
            }

            _time += duration;
        }

        protected void RepeatEvents(int times)
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
        }

        protected abstract NoteInfo CreateNoteInfo(T note);
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
