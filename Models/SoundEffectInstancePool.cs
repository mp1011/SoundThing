using Microsoft.Xna.Framework.Audio;
using System.Linq;

namespace SoundThing.Models
{
    class SoundEffectInstancePool
    {
        private SoundEffectInstance[] _instances;

        public SoundEffectInstancePool(SoundEffect effect, int maxInstances)
        {
            _instances = Enumerable.Range(0, maxInstances)
                .Select(p => effect.CreateInstance())
                .ToArray();
        }

        public void Play()
        {
            var firstFree = _instances.FirstOrDefault(p => p.State == SoundState.Stopped);
            if (firstFree == null)
                return;

            firstFree.Play();
        }
    }
}
