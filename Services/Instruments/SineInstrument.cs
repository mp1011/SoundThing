using SoundThing.Extensions;
using SoundThing.Models;
using System;

namespace SoundThing.Services.Instruments
{
    class SineInstrument : Instrument
    {
        public override Envelope? Envelope { get; } =
                          new Envelope(
                              sustainVolumePercent: 0.8,
                              attack: 0.1,
                              decay: 0.05,
                              release: 0.5);
        protected override Func<int, NoteEvent, short> NoteGenerator =>
            (int sampleIndex, NoteEvent noteEvent) =>
                Generator.Sine
                         .AddOvertones(3)
                         (sampleIndex, noteEvent);
                    
    }
}
