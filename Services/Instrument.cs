using SoundThing.Extensions;
using SoundThing.Models;
using System;

namespace SoundThing.Services
{
    abstract class Instrument
    {
        public short NoteValue(int sampleIndex, NoteEvent noteEvent) =>
            NoteGenerator
                .ApplyEnvelope(Envelope, noteEvent)
                (sampleIndex, noteEvent);

        public virtual Envelope? Envelope => null;

        protected abstract Func<int, NoteEvent, short> NoteGenerator { get; }
    }
}
