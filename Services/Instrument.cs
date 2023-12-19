using SoundThing.Extensions;
using SoundThing.Models;
using System;

namespace SoundThing.Services
{
    abstract class Instrument
    { 
        public virtual short NoteValue(int sampleIndex, NoteEvent noteEvent) =>
            NoteGenerator
                .ApplyEnvelope(noteEvent.MaybeEnvelope ?? Envelope, noteEvent)
                (sampleIndex, noteEvent);

        public virtual Envelope? Envelope => null;

        protected abstract Func<int, NoteEvent, short> NoteGenerator { get; }
    }
}
