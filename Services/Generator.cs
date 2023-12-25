using SoundThing.Models;
using System;

namespace SoundThing.Services
{
    static class Generator
    {
        private static Random _rng = new Random();

        public static Func<int, SoundInfo, short> Sine = (sampleIndex, soundInfo) =>
        {
            double theta = (soundInfo.Frequency * 2 * Math.PI) / Constants.SamplesPerSecond;
            return (short)(Math.Sin(sampleIndex * theta) * soundInfo.Volume);
        };

        public static Func<int, double> SineWave(double frequency, double min, double max)
        {
            return (sampleIndex) =>
            {
                double theta = (frequency * 2 * Math.PI) / (double)Constants.SamplesPerSecond;
                var sineValue = Math.Sin(sampleIndex * theta);

                var range = max - min;
                return min + ((sineValue + 1.0) / 2.0) * range;
            };
        }

        public static Func<int, SoundInfo, short> Square = (sampleIndex, soundInfo) =>
        {
            var waveDuration = 1.0 / soundInfo.Frequency;
            var samplesPerWave = waveDuration * Constants.SamplesPerSecond;

            var value = sampleIndex % samplesPerWave;
            if (value >= samplesPerWave / 2)
                return (short)soundInfo.Volume;
            else
                return (short)-soundInfo.Volume;
        };

        public static Func<int, SoundInfo, short> Fuzz(double level) => (sampleIndex, soundInfo) =>
        {
            var waveDuration = 1.0 / soundInfo.Frequency;
            var samplesPerWave = waveDuration * Constants.SamplesPerSecond;
            var value = sampleIndex % samplesPerWave;

            if (value >= samplesPerWave * level)
                return 0;
            else
                return (short)(soundInfo.Volume * _rng.NextDouble());
        };

        public static Func<int, SoundInfo, short> PulseWidthModulation(Parameter percent) => (sampleIndex, soundInfo) =>
        {
            var waveDuration = 1.0 / soundInfo.Frequency;
            var samplesPerWave = waveDuration * Constants.SamplesPerSecond;
          
            var value = sampleIndex % samplesPerWave;

            if (value >= samplesPerWave * percent)
                return 0;

            return (short)(soundInfo.Volume);
        };

        public static Func<int, SoundInfo, short> Saw = (sampleIndex, soundInfo) =>
        {
            var waveDuration = 1.0 / soundInfo.Frequency;
            var samplesPerWave = waveDuration * Constants.SamplesPerSecond;

            var value = sampleIndex % samplesPerWave;
            return (short)(soundInfo.Volume * (value / samplesPerWave));
        };
    }
}
