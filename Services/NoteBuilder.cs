using SoundThing.Extensions;
using SoundThing.Models;
using System.Collections;
using System.Collections.Generic;

namespace SoundThing.Services
{
    class NoteBuilder : IEnumerable<NoteEvent>
    {
        private double _time = 0;
        private readonly int _bpm;
        private NoteType _beatNote;
        public readonly Scale _scale;

        private List<NoteEvent> _noteEvents = new List<NoteEvent>();

        public NoteBuilder(Scale scale, int bpm, NoteType beatNote)
        {
            _scale = scale;
            _bpm = bpm;
            _beatNote = beatNote;
        }


        public NoteBuilder AddEights(params int[] notes) => AddNotes(NoteType.Eight, notes);
        public NoteBuilder AddQuarters(params int[] notes) => AddNotes(NoteType.Quarter, notes);
        public NoteBuilder AddHalves(params int[] notes) => AddNotes(NoteType.Half, notes);
        public NoteBuilder AddWholes(params int[] notes) => AddNotes(NoteType.Whole, notes);


        public NoteBuilder AddNotes(NoteType noteType, params int[] notes)
        {
            foreach(var note in notes)
            {
                var duration = noteType.GetDuration(_beatNote, _bpm);
                _noteEvents.Add(
                    new NoteEvent(
                        new PlayedNoteInfo(
                            noteInfo: _scale.GetNote(note),
                            duration: duration),
                        startTime: _time));
                _time += duration;
            }

            return this;
        }

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
