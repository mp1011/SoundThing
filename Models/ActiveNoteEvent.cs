namespace SoundThing.Models
{
    struct ActiveNoteEvent
    {
        public ActiveNoteEvent(NoteEvent @event, Envelope? maybeEnvelope)
        {
            Event = @event;
            MaybeEnvelope = maybeEnvelope;
        }

        public int SampleIndexEnd => Event.SampleIndexEnd;

        public NoteEvent Event { get; }
        public Envelope? MaybeEnvelope { get; }


    }
}
