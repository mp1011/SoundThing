using Microsoft.Xna.Framework.Audio;
using System;

namespace SoundThing.Services
{
    class SoundEffectMaker
    {
        public static SoundEffect Create(double seconds, Func<int, short> generator)
        {
            return Create((int)(Constants.SamplesPerSecond * seconds), generator);
        }

        public static SoundEffect Create(int samples, Func<int, short> generator)
        {
            byte[] buffer = new byte[samples * 2];

            for (int i = 0; i < buffer.Length / 2; i++)
            {
                short value = generator(i);
                buffer[2 * i] = (byte)value;
                buffer[(2 * i) + 1] = (byte)(value >> 8);
            }

            return new SoundEffect(buffer, Constants.SamplesPerSecond, AudioChannels.Mono);
        }
    }
}
