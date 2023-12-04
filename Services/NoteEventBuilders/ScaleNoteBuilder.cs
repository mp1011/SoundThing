using SoundThing.Models;

namespace SoundThing.Services.NoteEventBuilders
{
    class ScaleNoteBuilder : NoteEventBuilder<int>
    {
        private Scale _scale;

        public ScaleNoteBuilder(Scale scale, int bpm, NoteType beatNote) 
            : base(bpm, beatNote)
        {
            _scale = scale;
        }

        public ScaleNoteBuilder AddEights(params int[] notes) => AddNotes(NoteType.Eight, notes);
        public ScaleNoteBuilder AddQuarters(params int[] notes) => AddNotes(NoteType.Quarter, notes);
        public ScaleNoteBuilder AddHalves(params int[] notes) => AddNotes(NoteType.Half, notes);
        public ScaleNoteBuilder AddWholes(params int[] notes) => AddNotes(NoteType.Whole, notes);


        public ScaleNoteBuilder AddNotes(NoteType noteType, params int[] notes)
        {
            AddEvents(noteType, notes);
            return this;
        }

        public ScaleNoteBuilder SetOctave(int octave)
        {
            _scale = _scale.ChangeOctave(octave);
            return this;
        }

        protected override NoteInfo CreateNoteInfo(int note)
        {
            return _scale.GetNote(note);
        }
    }
}
