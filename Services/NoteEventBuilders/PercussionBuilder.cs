using SoundThing.Models;

namespace SoundThing.Services.NoteEventBuilders
{
    class PercussionBuilder : NoteEventBuilder<DrumPart>
    {
        public PercussionBuilder(int bpm, NoteType beatNote) : base(bpm, beatNote)
        {
        }

        protected override NoteInfo CreateNoteInfo(DrumPart note)
        {
            return new NoteInfo((MusicNote)note, 1, 1.0);
        }

        public PercussionBuilder Add(NoteType noteType, params DrumPart[] parts)
        {
            AddEvents(noteType, parts);
            return this;
        }

        public PercussionBuilder AddGroup(NoteType noteType, params DrumPart[] parts)
        {
            AddEventGroup(noteType, parts);
            return this;
        }

        public PercussionBuilder Repeat(int times)
        {
            RepeatEvents(times);
            return this;
        }

    }
}
