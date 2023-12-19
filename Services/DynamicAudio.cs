using Microsoft.Xna.Framework.Audio;
using System;

namespace SoundThing.Services
{
    class DynamicAudio
    {
        private const int SamplesPerBatch = Constants.SamplesPerSecond;
        private readonly Func<int, short> _generator;
        private readonly int _totalSamples;
        private readonly DynamicSoundEffectInstance _dynamicSound;

        private int _sampleIndex = 0;

        public SoundState State => _dynamicSound.State;

        public DynamicAudio(int samples, Func<int, short> generator)
        {
            _generator = generator;
            _totalSamples = samples;
            _dynamicSound = new DynamicSoundEffectInstance(Constants.SamplesPerSecond, AudioChannels.Mono);
            _dynamicSound.BufferNeeded += _dynamicSound_BufferNeeded;

            QueueNextBuffer();
            QueueNextBuffer();
        }

        private void _dynamicSound_BufferNeeded(object sender, System.EventArgs e)
        {
            QueueNextBuffer();
            QueueNextBuffer();
        }

        private void QueueNextBuffer()
        {
            var buffer = CreateBuffer(_sampleIndex);
            _sampleIndex += SamplesPerBatch;
            if (_sampleIndex >= _totalSamples)
                _sampleIndex = 0;

            _dynamicSound.SubmitBuffer(buffer);
        }

        public void Play()
        {
            _dynamicSound.Play();
        }

        public void Stop()
        {
            _dynamicSound.Stop();
        }

        private byte[] CreateBuffer(int start)
        {
            short[] sampleBuffer = new short[SamplesPerBatch];
            for (int i = start; i < start + sampleBuffer.Length; i++)
                sampleBuffer[i-start] = _generator(i);

            byte[] buffer = new byte[SamplesPerBatch*2];
            for (int i = 0; i < buffer.Length / 2; i++)
            {
                short value = sampleBuffer[i];
                buffer[2 * i] = (byte)value;
                buffer[(2 * i) + 1] = (byte)(value >> 8);
            }

            return buffer;
        }
    }
}
