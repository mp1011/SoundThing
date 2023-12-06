using System;

namespace SoundThing.Services
{
    static class PostProcessor
    {
        const int DeclickThreshold = 100;

        public static short[] Declick(short[] samples)
        {
            return samples;

            for(int i = 1; i < samples.Length; i++)
            {
                var previous = samples[i - 1];
                var current = samples[i];
                var difference = current - previous;

                if(Math.Abs(difference) > DeclickThreshold)
                {
                    samples[i] = (short)(previous + (difference * 0.1));
                }
            }

            return samples;
        }
    }
}
