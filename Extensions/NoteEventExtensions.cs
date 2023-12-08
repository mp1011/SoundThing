using SoundThing.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoundThing.Extensions
{
    static class NoteEventExtensions
    {
        public static IEnumerable<NoteEvent> ToNoteEvents(this IEnumerable<NoteInfo> notes, double timePerNote, double startTime)
        {
            return notes.Select((noteInfo,index) =>
                 new NoteEvent(new PlayedNoteInfo(noteInfo, timePerNote), startTime + (timePerNote * index)));               
        }

        public static IEnumerable<NoteEvent> AdjustToEnvelope (this IEnumerable<NoteEvent> noteEvents, Envelope? maybeEnvelope)
        {
            if (maybeEnvelope == null)
                return noteEvents;

            var envelope = maybeEnvelope.Value;
            var minEnvelopeSamples = envelope.AttackSamples + envelope.DecaySamples + envelope.ReleaseSamples;
            return noteEvents.Select(p =>
            {
                if (minEnvelopeSamples > p.Note.SampleDuration)
                    return p.AdjustToEnvelope(envelope);
                else
                    return p;
            });
        }

        private static NoteEvent AdjustToEnvelope(this NoteEvent noteEvent, Envelope envelope)
        {
            var adjustedDuration = noteEvent.Note.Duration + envelope.Release;

            return new NoteEvent(
                note: new PlayedNoteInfo(
                    noteInfo: noteEvent.Note.NoteInfo,
                    duration: adjustedDuration),
                startIndex: noteEvent.SampleIndexStart);
        }

        public static Func<int, NoteEvent, short> ApplyEnvelope(this Func<int, NoteEvent, short> generator,
          Envelope? maybeEnvelope,
          NoteEvent noteEvent)
        {
            if (maybeEnvelope == null)
                return generator;

            var envelope = maybeEnvelope.Value;

            return (int sampleIndex, NoteEvent noteEvent) =>
            {
                SoundInfo soundInfo = noteEvent.Note.NoteInfo;
                if (sampleIndex < noteEvent.SampleIndexStart)
                    return generator(sampleIndex, noteEvent);
                if (sampleIndex > noteEvent.SampleIndexEnd)
                    return generator(sampleIndex, noteEvent);

                var sustainSamples = noteEvent.Note.SampleDuration -
                    (envelope.AttackSamples + envelope.DecaySamples + envelope.ReleaseSamples);

                var envelopeSampleIndex = sampleIndex - noteEvent.SampleIndexStart;

                if (envelopeSampleIndex < envelope.AttackSamples)
                {
                    var attackPercent = envelopeSampleIndex / (double)(envelope.AttackSamples);
                    return generator(sampleIndex, noteEvent.ChangeVolume(soundInfo.VolumePercent * attackPercent));
                }
                else if (envelopeSampleIndex < (envelope.AttackSamples + envelope.DecaySamples))
                {
                    int decayIndex = envelopeSampleIndex - envelope.AttackSamples;
                    var decayPercent = decayIndex / (double)envelope.DecaySamples;

                    var sustainVolume = soundInfo.VolumePercent * envelope.SustainVolumePercent;
                    var sustainVolumeDrop = soundInfo.VolumePercent - sustainVolume;
                    var decayVolume = soundInfo.VolumePercent - (sustainVolumeDrop * decayPercent);

                    return generator(sampleIndex, noteEvent.ChangeVolume(decayVolume));
                }
                else if (envelopeSampleIndex < (envelope.AttackSamples + envelope.DecaySamples + sustainSamples))
                {
                    var sustainVolume = soundInfo.VolumePercent * envelope.SustainVolumePercent;
                    return generator(sampleIndex, noteEvent.ChangeVolume(sustainVolume));
                }
                else
                {
                    int releaseIndex = envelopeSampleIndex - (envelope.AttackSamples + envelope.DecaySamples + sustainSamples);
                    var releasePercent = releaseIndex / (double)envelope.ReleaseSamples;
                    if (releasePercent > 1.0)
                        releasePercent = 1.0;

                    var sustainVolume = soundInfo.VolumePercent * envelope.SustainVolumePercent;
                    var releaseVolume = sustainVolume * (1.0 - releasePercent);
                    return generator(sampleIndex, noteEvent.ChangeVolume(releaseVolume));
                }
            };
        }
    }
}
