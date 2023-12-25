using SoundThing.Extensions;
using SoundThing.Models;
using System;
using System.Collections.Generic;

namespace SoundThing.Services.Instruments
{
    class ParameterModTestInstrument : Instrument
    {
        public override Envelope? Envelope { get; } =
                       new Envelope(
                           sustainVolumePercent: 0.7,
                           attack: 0.0,
                           decay: 0.2,
                           release: 5.0);

        public Parameter GainAmount { get; } = new Parameter(
            "Gain",
            min: 0.5,
            max: 1.5,
            value: 1.0,
            format: "P");

        public Parameter SinFreq { get; } = new Parameter(
            "SinFreq",
            .001,
            1,
            0.3,
            format: "0.000");

        public override IEnumerable<Parameter> Parameters
        {
            get
            {
                yield return GainAmount;
                yield return SinFreq;
            }
        }

        protected override Func<int, NoteEvent, short> NoteGenerator =>
            (int sampleIndex, NoteEvent noteEvent) =>
                Generator.Saw
                         .Gain(GainAmount)
                         .ModifyParameter(GainAmount, Generator.SineWave(SinFreq, 0.1, 1.0))
                         (sampleIndex, noteEvent);
    }
}
