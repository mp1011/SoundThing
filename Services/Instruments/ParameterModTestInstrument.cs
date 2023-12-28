using SoundThing.Extensions;
using SoundThing.Models;
using System;
using System.Collections.Generic;

namespace SoundThing.Services.Instruments
{
    class ParameterModTestInstrument : Instrument
    {
        public override Envelope? Envelope
        {
            get => new Envelope(
                    sustainVolumePercent: SustainVolumePercent,
                    attack: Attack,
                    decay: Decay,
                    release: Release);
        }

        public Parameter SustainVolumePercent { get; } = new Parameter(
            name: "Sus",
            min: 0.0,
            max: 1.0,
            value: 0.8,
            format: "P");

        public Parameter Attack { get; } = new Parameter(
            name: "Att",
            min: 0.0,
            max: 10.0,
            value: 0.0,
            format: "0.00");

        public Parameter Decay { get; } = new Parameter(
           name: "Dec",
           min: 0.0,
           max: 10.0,
           value: 0.2,
           format: "0.00");

        public Parameter Release { get; } = new Parameter(
            name: "Rel",
            min: 0.0,
            max: 10.0,
            value: 0.2,
            format: "0.00");

        public Parameter PulseWidthPercent { get; } = new Parameter(
            "PW",
            min: 0.0,
            max: 1.0,
            value: 0.5,
            format: "P");

        public Parameter GainAmount { get; } = new Parameter(
            "Gain",
            min: 0.5,
            max: 5.0,
            value: 1.0,
            format: "P");

        public Parameter ClipAmount { get; } = new Parameter(
           "Clip",
           min: 0.5,
           max: 5.0,
           value: 5.0,
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
                yield return Attack;
                yield return Decay;
                yield return SustainVolumePercent;
                yield return Release;
                yield return PulseWidthPercent;
                yield return GainAmount;
                yield return ClipAmount;
                yield return SinFreq;
            }
        }

        protected override Func<int, NoteEvent, short> NoteGenerator =>
            (int sampleIndex, NoteEvent noteEvent) =>
                Generator.PulseWidthModulation(PulseWidthPercent)
                         .Add(Generator.Sine)
                         .Gain(GainAmount)
                         .Clip(ClipAmount)
                         .ModifyParameter(PulseWidthPercent, Generator.SineWave(SinFreq, 0.1, 1.0))
                         (sampleIndex, noteEvent);
    }
}
