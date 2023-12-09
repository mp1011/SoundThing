using SoundThing.Extensions;
using SoundThing.Models;
using System;

namespace SoundThing.Services.Instruments
{
    class PluckyInstrument : Instrument
    {
        public override Envelope? Envelope { get; } =
                        new Envelope(
                            sustainVolumePercent: 0.4,
                            attack: 0.0,
                            decay: 0.05,
                            release: 0.05);

        protected override Func<int, NoteEvent, short> NoteGenerator =>
            (int sampleIndex, NoteEvent noteEvent) =>
                Generator.Sine
                         .AddOvertones(6)
                         .Clip(1.0)
                         (sampleIndex, noteEvent);
                    
    }
}
