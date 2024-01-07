using SoundThing.Extensions;
using SoundThing.Models;
using System;
using System.Collections.Generic;

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

        public Parameter Overtones { get; } = new Parameter(
            name: "Overtones",
            min: 0,
            max: 10,
            value: 2,
            format: "0");

        public override IEnumerable<IParameter> Parameters
        {
            get
            {
                yield return Overtones;
            }
        }


        protected override Func<int, NoteEvent, short> NoteGenerator =>
            (int sampleIndex, NoteEvent noteEvent) =>
                Generator.Sine
                         .AddOvertones(Overtones)
                         .Clip(1.0)
                         (sampleIndex, noteEvent);
                    
    }
}
