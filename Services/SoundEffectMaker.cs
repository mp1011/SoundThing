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
            short[] sampleBuffer = new short[samples];
            for(int i = 0; i < sampleBuffer.Length; i++)
                sampleBuffer[i] = generator(i);

            sampleBuffer = PostProcessor.Declick(sampleBuffer);

            byte[] buffer = new byte[samples * 2];
            for (int i = 0; i < buffer.Length / 2; i++)
            {
                short value = sampleBuffer[i];
                buffer[2 * i] = (byte)value;
                buffer[(2 * i) + 1] = (byte)(value >> 8);
            }


            System.IO.File.WriteAllBytes("sound.dat", buffer);
            return new SoundEffect(buffer, Constants.SamplesPerSecond, AudioChannels.Mono);
        }
    }
}
