using SoundThing.Extensions;
using SoundThing.Models;
using System;

namespace SoundThing.Services.Instruments
{
    class SquareInstrument : Instrument
    {
        public override Envelope? Envelope { get; } =
                   new Envelope(
                       sustainVolumePercent: 0.8,
                       attack: 0.01,
                       decay: 0.2,
                       release: 0.1);
        protected override Func<int, NoteEvent, short> NoteGenerator =>
            (int sampleIndex, NoteEvent noteEvent) =>
                Generator.Square
                         (sampleIndex, noteEvent);
                    
    }
}
