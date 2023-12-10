using SoundThing.Extensions;
using SoundThing.Models;
using System;

namespace SoundThing.Services.Instruments
{
    class PadInstrument : Instrument
    {
        public override Envelope? Envelope { get; } =
                        new Envelope(
                            sustainVolumePercent: 0.7,
                            attack: 0.5,
                            decay: 0.05,
                            release: 0.5);

        protected override Func<int, NoteEvent, short> NoteGenerator =>
            (int sampleIndex, NoteEvent noteEvent) =>
                Generator.Square
                         (sampleIndex, noteEvent);

    }
}
