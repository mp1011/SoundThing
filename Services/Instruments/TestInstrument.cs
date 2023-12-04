using SoundThing.Extensions;
using SoundThing.Models;
using System;

namespace SoundThing.Services.Instruments
{
    class TestInstrument : Instrument
    {
        public override Envelope? Envelope { get; } =
                        new Envelope(
                            sustainVolumePercent: 0.7,
                            attack: 0.0,
                            decay: 0.2,
                            release: 0.2);

        protected override Func<int, NoteEvent, short> NoteGenerator =>
            (int sampleIndex, NoteEvent noteEvent) =>
                Generator.Square
                         .AddOvertones(6)
                         //.PowerChord()
                       // .Gain(1.5)
                        //.Clip(1.0)
                         (sampleIndex, noteEvent);
                    
    }
}
