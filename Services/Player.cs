﻿using Microsoft.Xna.Framework.Audio;
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
                .OrderBy(p => p.SampleIndexStart)
                .ToArray();
        }

        private Func<int, short> BaseSoundGenerator()
        {
            List<NoteEvent> playingNotes = new List<NoteEvent>();

            return (int sampleIndex) => {
                short noteValue = 0;

                playingNotes.AddRange(_events
                            .Where(p => p.SampleIndexStart == sampleIndex));

                playingNotes.RemoveAll(p => sampleIndex > p.SampleIndexEnd);

                foreach (var noteEvent in playingNotes)
                {
                    noteValue += _instrument.NoteValue(sampleIndex, noteEvent);
                }

                return noteValue;
            };
        }
        private Func<int, short> SoundGenerator()
        {
            return BaseSoundGenerator();
                 //  .AddEcho(0.5);
        }


        public SoundEffectInstance GenerateSound()
        {
            var lastEvent = _events.Last();
            var totalSamples = lastEvent.SampleIndexStart + lastEvent.Note.SampleDuration;
            var sfx = SoundEffectMaker.Create(totalSamples, SoundGenerator());
            return sfx.CreateInstance();
        }
    }
}
