using Microsoft.Xna.Framework.Audio;
using SoundThing.Extensions;
using SoundThing.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoundThing.Services
{
    class Player
    {
        private Instrument _instrument;
        private NoteEvent[] _events;
        public int Channel { get; }

        public Instrument Instrument  => _instrument;

        public Player(Instrument instrument, int channel, IEnumerable<NoteEvent> events)
        {
            Channel = channel;
            _instrument = instrument;
            _events = events
                .OrderBy(p => p.SampleIndexStart)
                .AdjustToEnvelope(_instrument.Envelope)                
                .ToArray();

            _events = _events
                .Select(ApplyCutoff)
                .ToArray();
        }

        public int TotalSamples
        {
            get
            {
                var lastEvent = _events.Last();
                return lastEvent.SampleIndexStart + lastEvent.Note.SampleDuration;
            }
        }

        private NoteEvent ApplyCutoff(NoteEvent noteEvent)
        {
            var nextNote = _events.FirstOrDefault(p => p.SampleIndexStart > noteEvent.SampleIndexStart
                                                       && !p.IsRest);

            if (_instrument.Envelope == null
                || nextNote.SampleIndexStart == 0
                || nextNote.SampleIndexStart > noteEvent.SampleIndexEnd)
                return noteEvent;

            var targetSampleDuration = nextNote.SampleIndexStart - noteEvent.SampleIndexStart;
            var envelope = _instrument.Envelope.Value;
            var sustainSamples = noteEvent.SampleDuration - envelope.MinEnvelopeSamples;

            var adjustedReleaseSamples = targetSampleDuration - (envelope.AttackSamples + envelope.DecaySamples + sustainSamples);
            if (adjustedReleaseSamples == 0)
                adjustedReleaseSamples = (int)(0.1 * Constants.SamplesPerSecond);

            var limit = targetSampleDuration - (envelope.AttackSamples + envelope.DecaySamples);
            if (adjustedReleaseSamples > limit)
                adjustedReleaseSamples = limit;

            var adjustedEnvelope = new Envelope(
                sustainVolumePercent: envelope.SustainVolumePercent,
                attack: envelope.Attack,
                decay: envelope.Decay,
                release: adjustedReleaseSamples / (double)Constants.SamplesPerSecond);

            return noteEvent.ChangeSampleDuration(targetSampleDuration)
                            .SetEnvelope(adjustedEnvelope);                
        }

        public Func<int, short> SoundGenerator()
        {
            return (int sampleIndex) => 
                (short)_events
                    .Where(p => p.SampleIndexStart <= sampleIndex 
                                && p.SampleIndexEnd >= sampleIndex)
                    .Sum(p => _instrument.NoteValue(sampleIndex, p));                           
        }
    }
}
