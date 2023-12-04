using SoundThing.Extensions;
using SoundThing.Models;
using System;

namespace SoundThing.Services.Instruments
{
    abstract class DrumKit : Instrument
    {
        protected abstract Instrument Kick { get; }
        protected abstract Instrument Snare { get; }
        protected abstract Instrument HiHat { get; }
    }

    class TestDrumKit : DrumKit
    {
        protected override Instrument Kick { get; } = new KickDrum();

        protected override Instrument Snare { get; } = new Snare();

        protected override Instrument HiHat { get; } = new HiHat();

        protected override Func<int, NoteEvent, short> NoteGenerator =>
            (int sampleIndex, NoteEvent noteEvent) =>
                DrumNoteGenerator(sampleIndex, noteEvent);

        private short DrumNoteGenerator(int sampleIndex, NoteEvent noteEvent)
        {
            return noteEvent.DrumPart switch
            {
                DrumPart.Kick => Kick.NoteValue(sampleIndex, noteEvent),
                DrumPart.HiHat => HiHat.NoteValue(sampleIndex, noteEvent),
                DrumPart.Snare => Snare.NoteValue(sampleIndex, noteEvent),
                _ => 0
            };
        }
    }

    class HiHat : Instrument
    {
        public override Envelope? Envelope { get; } =
                          new Envelope(
                              sustainVolumePercent: 0.0,
                              attack: 0.0,
                              decay: 0.1,
                              release: 0.0);
        protected override Func<int, NoteEvent, short> NoteGenerator =>
              (int sampleIndex, NoteEvent noteEvent) =>
                  Generator.Fuzz(0.98)
                           .Gain(0.5)
                           (sampleIndex, noteEvent);
    }

    class Snare : Instrument
    {
        public override Envelope? Envelope { get; } =
                          new Envelope(
                              sustainVolumePercent: 0.0,
                              attack: 0.0,
                              decay: 0.3,
                              release: 0.0);
        protected override Func<int, NoteEvent, short> NoteGenerator =>
              (int sampleIndex, NoteEvent noteEvent) =>
                  Generator.Fuzz(0.9)
                           (sampleIndex, noteEvent);
    }

    class KickDrum : Instrument
    {
        public override Envelope? Envelope { get; } =
                          new Envelope(
                              sustainVolumePercent: 0.0,
                              attack: 0.0,
                              decay: 0.1,
                              release: 0.0);

        protected override Func<int, NoteEvent, short> NoteGenerator =>
              (int sampleIndex, NoteEvent noteEvent) =>
                  Generator.Sine
                           .Gain(3.0)
                           (sampleIndex, noteEvent);
    }
}
