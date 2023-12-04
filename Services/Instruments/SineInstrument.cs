using SoundThing.Extensions;
using SoundThing.Models;
using System;

namespace SoundThing.Services.Instruments
{
    class SineInstrument : Instrument
    {
        protected override Func<int, NoteEvent, short> NoteGenerator =>
            (int sampleIndex, NoteEvent noteEvent) =>
                Generator.Sine
                         .AddOvertones(6)
                         (sampleIndex, noteEvent);
                    
    }
}
