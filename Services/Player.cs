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

        public Player(Instrument instrument, IEnumerable<NoteEvent> events)
        {
            _instrument = instrument;
            _events = events
                .AdjustToEnvelope(_instrument.Envelope)
                .OrderBy(p => p.SampleIndexStart)
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

        private Func<int, short> BaseSoundGenerator()
        {
            List<ActiveNoteEvent> playingNotes = new List<ActiveNoteEvent>();

            return (int sampleIndex) => {
                short noteValue = 0;

                playingNotes.AddRange(_events
                            .Where(p => p.SampleIndexStart == sampleIndex)
                            .Select(ToActiveNoteEvent));

                playingNotes.RemoveAll(p => sampleIndex > p.SampleIndexEnd);

                foreach (var noteEvent in playingNotes)
                {
                    noteValue += _instrument.NoteValue(sampleIndex, noteEvent);
                }

                return noteValue;
            };
        }

        private ActiveNoteEvent ToActiveNoteEvent(NoteEvent noteEvent)
        {
            var nextNote = _events.FirstOrDefault(p => p.SampleIndexStart > noteEvent.SampleIndexStart
                                                       && !p.IsRest);

            if (_instrument.Envelope == null 
                || nextNote.SampleIndexStart == 0
                || nextNote.SampleIndexStart > noteEvent.SampleIndexEnd)
                return new ActiveNoteEvent(noteEvent, _instrument.Envelope);

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

            return new ActiveNoteEvent(noteEvent.ChangeSampleDuration(targetSampleDuration), adjustedEnvelope);             
        }

        public Func<int, short> SoundGenerator()
        {
            return BaseSoundGenerator();
                 //  .AddEcho(0.5);
        }
    }
}
