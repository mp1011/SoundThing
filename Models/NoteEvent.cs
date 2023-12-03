namespace SoundThing.Models
{
    struct NoteEvent
    {
        public NoteEvent(PlayedNoteInfo note, double startTime)
        {
            Note = note;
            SampleIndexStart = (int)(startTime * Constants.SamplesPerSecond);
        }

        public PlayedNoteInfo Note { get; }
        public int SampleIndexStart { get; }
        public int SampleIndexEnd => SampleIndexStart + Note.SampleDuration;
    }
}
