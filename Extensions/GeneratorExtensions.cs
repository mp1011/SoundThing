using SoundThing.Models;
using System;

namespace SoundThing.Extensions
{
     static class GeneratorExtensions
    {
        public static Func<int, SoundInfo, short> Add(this Func<int, SoundInfo, short> generator, Func<int, short> generator2)
        {
            return (int sampleIndex, SoundInfo soundInfo) =>
            {
                return (short)(generator(sampleIndex, soundInfo) + generator2(sampleIndex));
            };
        }

        public static Func<int, SoundInfo, short> RootAndFifth(this Func<int, SoundInfo, short> generator)
        {
            return (int sampleIndex, SoundInfo soundInfo) =>
            {
                var root = generator(sampleIndex, soundInfo);
                var fifth = generator(sampleIndex, soundInfo.ToNoteInfo().Fifth());

                return (short)(root + fifth);
            };
        }

        public static Func<int, SoundInfo, short> PowerChord(this Func<int, SoundInfo, short> generator)
        {
            return (int sampleIndex, SoundInfo soundInfo) =>
            {
                var root = generator(sampleIndex, soundInfo);
                var fifth = generator(sampleIndex, soundInfo.ToNoteInfo().Fifth());
                var octave = generator(sampleIndex, soundInfo.ToNoteInfo().Octave());

                return (short)(root + fifth + octave);
            };
        }


        public static Func<int, SoundInfo, short> AddOvertones(this Func<int, SoundInfo, short> generator, int overtones)
        {
            return (int sampleIndex, SoundInfo soundInfo) =>
            {
                short value = generator(sampleIndex, soundInfo);
                for (int i = 0; i < overtones; i++)
                {
                    soundInfo = new SoundInfo(soundInfo.Frequency * (i + 2), soundInfo.VolumePercent / 2);
                    value += generator(sampleIndex, soundInfo);
                }
                return value;
            };
        }

        public static Func<int, SoundInfo, short> Gain(this Func<int, SoundInfo, short> generator, double gainPercent)
        {
            return (int sampleIndex, SoundInfo soundInfo) =>
            {
                return generator(sampleIndex, new SoundInfo(soundInfo.Frequency, soundInfo.VolumePercent * gainPercent));
            };
        }

        public static Func<int, SoundInfo, short> Clip(this Func<int, SoundInfo, short> generator, double clipPercent)
        {
            short max = (short)(clipPercent * Constants.MaxVolume);

            return (int sampleIndex, SoundInfo soundInfo) =>
            {
                var value = generator(sampleIndex, soundInfo);
                if (value > max)
                    return max;
                else if (value < -max)
                    return (short)-max;
                else
                    return value;
            };
        }

        public static Func<int, short> AddEcho(this Func<int, short> generator, double secondsDelay)
        {
            var echoSamples = (int)(secondsDelay * Constants.SamplesPerSecond);

            return (int sampleIndex) =>
            {
                var value = generator(sampleIndex);

                if(sampleIndex >= echoSamples)
                {
                    var echoIndex = sampleIndex - echoSamples;
                    value += (short)(generator(echoIndex) / 4);
                }

                return value;
            };
        }

        public static Func<int, SoundInfo, short> ApplyEnvelope(this Func<int, SoundInfo, short> generator, 
            Envelope? maybeEnvelope, 
            NoteEvent noteEvent)
        {
            if (maybeEnvelope == null)
                return generator;

            var envelope = maybeEnvelope.Value;

            return (int sampleIndex, SoundInfo soundInfo) =>
            {
                if (sampleIndex < noteEvent.SampleIndexStart)
                    return generator(sampleIndex, soundInfo);
                if (sampleIndex > noteEvent.SampleIndexEnd)
                    return generator(sampleIndex, soundInfo);

                var sustainSamples = noteEvent.Note.SampleDuration - 
                    (envelope.AttackSamples + envelope.DecaySamples + envelope.ReleaseSamples);

                var envelopeSampleIndex = sampleIndex - noteEvent.SampleIndexStart;

                if (envelopeSampleIndex < envelope.AttackSamples)
                {
                    var attackPercent = envelopeSampleIndex / (envelope.AttackSamples);
                    return generator(sampleIndex, new SoundInfo(soundInfo.Frequency, soundInfo.VolumePercent * attackPercent));
                }
                else if (envelopeSampleIndex < (envelope.AttackSamples + envelope.DecaySamples))
                {
                    int decayIndex = envelopeSampleIndex - envelope.AttackSamples;
                    var decayPercent = decayIndex / envelope.DecaySamples;

                    var sustainVolume = soundInfo.VolumePercent * envelope.SustainVolumePercent;
                    var sustainVolumeDrop = soundInfo.VolumePercent - sustainVolume;
                    var decayVolume = soundInfo.VolumePercent - (sustainVolumeDrop * decayPercent);

                    return generator(sampleIndex, new SoundInfo(soundInfo.Frequency, decayVolume));
                }
                else if (envelopeSampleIndex < (envelope.AttackSamples + envelope.DecaySamples + sustainSamples))
                {
                    var sustainVolume = soundInfo.VolumePercent * envelope.SustainVolumePercent;
                    return generator(sampleIndex, new SoundInfo(soundInfo.Frequency, sustainVolume));
                }
                else
                {
                    int releaseIndex = envelopeSampleIndex - (envelope.AttackSamples + envelope.DecaySamples + sustainSamples);
                    var releasePercent = releaseIndex / (double)envelope.ReleaseSamples;
                    if (releasePercent > 1.0)
                        releasePercent = 1.0;

                    var sustainVolume = soundInfo.VolumePercent * envelope.SustainVolumePercent;
                    var releaseVolume = sustainVolume * (1.0 - releasePercent);
                    return generator(sampleIndex, new SoundInfo(soundInfo.Frequency, releaseVolume));
                }
            };
        }

    }

}
