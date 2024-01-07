using SoundThing.Extensions;
using SoundThing.Models;
using System;
using System.Collections.Generic;

namespace SoundThing.Services.Instruments
{
    class PWMInstrument : Instrument
    {
        public override Envelope? Envelope { get; } =
                        new Envelope(
                            sustainVolumePercent: 0.7,
                            attack: 0.0,
                            decay: 0.2,
                            release: 5.0);

        public Parameter Percent { get; } = new Parameter(
            "Percent",
            min: 0,
            max: 1.0,
            value: 0.75,
            format: "P");

        public Parameter SinFreq { get; } = new Parameter(
            "SinFreq",
            .001,
            1,
            0.3,
            format: "0.000");

        public override IEnumerable<IParameter> Parameters
        {
            get
            {
                yield return Percent;
                yield return SinFreq;
            }
        }

        protected override Func<int, NoteEvent, short> NoteGenerator =>
            (int sampleIndex, NoteEvent noteEvent) =>
                Generator.PulseWidthModulation(Percent)   
                         .ModifyParameter(Percent, Generator.SineWave(SinFreq, 0.1, 1.0))
                         (sampleIndex, noteEvent);
                    
    }
}
