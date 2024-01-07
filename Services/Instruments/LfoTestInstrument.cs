using SoundThing.Extensions;
using SoundThing.Models;
using System;
using System.Collections.Generic;

namespace SoundThing.Services.Instruments
{
    class LfoTestInstrument : Instrument
    {
        public Parameter Frequency { get; } = new Parameter(
            "Frequency", 
            0.001, 
            0.25, 
            0.07,
            "0.000");

        public override IEnumerable<IParameter> Parameters
        {
            get
            {
                yield return Frequency;
            }
        }

        protected override Func<int, NoteEvent, short> NoteGenerator =>
            (int sampleIndex, NoteEvent noteEvent) =>
                Generator.Saw
                         .Lfo(Frequency, 1.0, 0.5)
                         (sampleIndex, noteEvent);
                    
    }
}
