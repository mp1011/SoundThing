using SoundThing.Extensions;
using SoundThing.Models;
using System;

namespace SoundThing.Services
{
    abstract class Instrument
    {   //fix me
        public virtual short NoteValue(int sampleIndex, NoteEvent noteEvent) =>
            NoteGenerator
                .ApplyEnvelope(Envelope, noteEvent)
                (sampleIndex, noteEvent);
        //fix me
        public virtual short NoteValue(int sampleIndex, ActiveNoteEvent noteEvent) =>
           NoteGenerator
               .ApplyEnvelope(noteEvent.MaybeEnvelope, noteEvent.Event)
               (sampleIndex, noteEvent.Event);

        public virtual Envelope? Envelope => null;

        protected abstract Func<int, NoteEvent, short> NoteGenerator { get; }
    }
}
