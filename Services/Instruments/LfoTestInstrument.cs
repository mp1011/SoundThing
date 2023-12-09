using SoundThing.Extensions;
using SoundThing.Models;
using System;

namespace SoundThing.Services.Instruments
{
    class LfoTestInstrument : Instrument
    {
        protected override Func<int, NoteEvent, short> NoteGenerator =>
            (int sampleIndex, NoteEvent noteEvent) =>
                Generator.Saw
                         .Lfo(0.07, 1.0, 0.5)
                         (sampleIndex, noteEvent);
                    
    }
}
