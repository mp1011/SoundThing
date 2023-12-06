using Microsoft.Xna.Framework.Audio;
using SoundThing.Extensions;
using System.Linq;

namespace SoundThing.Services
{
    class Band
    {
        private Player[] _players;

        public Band(params Player[] players)
        {
            _players = players;
        }

        public SoundEffectInstance GenerateSound()
        {
            var totalSamples = _players.Max(p => p.TotalSamples);

            var soundGenerator = _players
                .Select(p => p.SoundGenerator())
                .Aggregate((a, b) => a.Add(b));


            var sfx = SoundEffectMaker.Create(totalSamples, 
                soundGenerator
                    .Clip(1.0));
  
            return sfx.CreateInstance();
        }
    }
}
