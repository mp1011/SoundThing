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
                (sampleIndex, noteEvent.Note.NoteInfo);

        public virtual Envelope? Envelope => null;

        protected abstract Func<int, SoundInfo, short> NoteGenerator { get; }
    }
}
