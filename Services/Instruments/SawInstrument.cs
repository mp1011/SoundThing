using SoundThing.Extensions;
using SoundThing.Models;
using System;

namespace SoundThing.Services.Instruments
{
    class SawInstrument : Instrument
    {
        public override Envelope? Envelope { get; } =
                   new Envelope(
                       sustainVolumePercent: 0.2,
                       attack: 0.1,
                       decay: 0.2,
                       release: 0.01);
        protected override Func<int, NoteEvent, short> NoteGenerator =>
            (int sampleIndex, NoteEvent noteEvent) =>
                Generator.Saw
                         .AddOvertones(4)
                         (sampleIndex, noteEvent);
                    
    }
}
