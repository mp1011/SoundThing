using SoundThing.Models;
using SoundThing.Services;
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

        public static Func<int, SoundInfo, short> Add(this Func<int, SoundInfo, short> generator, Func<int, SoundInfo, short> generator2)
        {
            return (int sampleIndex, SoundInfo soundInfo) =>
            {
                return (short)(generator(sampleIndex, soundInfo) + generator2(sampleIndex, soundInfo));
            };
        }

        public static Func<int, SoundInfo, short> Multiply(this Func<int, SoundInfo, short> generator, Func<int, short> generator2)
        {
            return (int sampleIndex, SoundInfo soundInfo) =>
            {
                return (short)(generator(sampleIndex, soundInfo) * generator2(sampleIndex));
            };
        }

        public static Func<int, SoundInfo, short> Multiply(this Func<int, SoundInfo, short> generator, Func<int, SoundInfo, short> generator2)
        {
            return (int sampleIndex, SoundInfo soundInfo) =>
            {
                return (short)(generator(sampleIndex, soundInfo) * generator2(sampleIndex, soundInfo));
            };
        }

        public static Func<int, SoundInfo, short> Multiply(this Func<int, SoundInfo, short> generator, double amount)
        {
            return (int sampleIndex, SoundInfo soundInfo) =>
            {
                return (short)(generator(sampleIndex, soundInfo) * amount);
            };
        }

        public static Func<int, short> Add(this Func<int, short> generator, Func<int, short> generator2)
        {
            return (int sampleIndex) =>
            {
                return (short)(generator(sampleIndex) + generator2(sampleIndex));
            };
        }

        public static Func<int, short> Multiply(this Func<int, short> generator, Func<int, short> generator2)
        {
            return (int sampleIndex) =>
            {
                return (short)(generator(sampleIndex) * generator2(sampleIndex));
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
                    soundInfo = new SoundInfo(soundInfo.Frequency * (double)(i + 2), soundInfo.VolumePercent / 2);
                    value += generator(sampleIndex, soundInfo);
                }
                return value;
            };
        }

        public static Func<int, SoundInfo, short> Lfo(this Func<int, SoundInfo, short> generator, 
            double frequency,
            double volumeA,
            double volumeB)
        {
            var samplesPerPulse = frequency * (double)Constants.SamplesPerSecond;

            return (int sampleIndex, SoundInfo soundInfo) =>
            {
                var pulseIndex = sampleIndex % (samplesPerPulse * 2);
                if (pulseIndex > samplesPerPulse)
                    return generator(sampleIndex, soundInfo.ChangeVolumePercent(volumeA));
                else
                    return generator(sampleIndex, soundInfo.ChangeVolumePercent(volumeB));
            };

        }

        public static Func<int, SoundInfo, short> Gain(this Func<int, SoundInfo, short> generator, double gainPercent)
        {
            return (int sampleIndex, SoundInfo soundInfo) =>
            {
                return generator(sampleIndex, new SoundInfo(soundInfo.Frequency, soundInfo.VolumePercent * gainPercent));
            };
        }

        public static Func<int, SoundInfo, short> Abs(this Func<int, SoundInfo, short> generator)
        {
            return (int sampleIndex, SoundInfo soundInfo) =>
            {
                var value = generator(sampleIndex, soundInfo);
                if (value < 0)
                    return (short)-value;
                else
                    return value;
            };
        }

        public static Func<int, SoundInfo, short> AbsNeg(this Func<int, SoundInfo, short> generator)
        {
            return (int sampleIndex, SoundInfo soundInfo) =>
            {
                var value = generator(sampleIndex, soundInfo);
                if (value > 0)
                    return (short)-value;
                else
                    return value;
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

        public static Func<int, short> Clip(this Func<int, short> generator, double clipPercent)
        {
            short max = (short)(clipPercent * Constants.MaxVolume);

            return (int sampleIndex) =>
            {
                var value = generator(sampleIndex);
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

      

    }

}
