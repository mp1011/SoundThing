using Microsoft.Xna.Framework.Audio;
using SoundThing.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace SoundThing.Services
{
    class Band
    {
        private Player[] _players;

        public static SoundEffectInstance[] CreateSounds(params Player[] players)
        {
            var band = new Band(players);
            return band.GenerateSounds();
        }

        public Band(params Player[] players)
        {
            _players = players;
        }

        public SoundEffectInstance[] GenerateSounds() =>          
            _players
                .GroupBy(p => p.Channel)
                .Select(GenerateSound)
                .ToArray();

        private SoundEffectInstance GenerateSound(IEnumerable<Player> players)
        {
            var totalSamples = _players.Max(p => p.TotalSamples);

            var soundGenerator = players
                .Select(p => p.SoundGenerator())
                .Aggregate((a, b) => a.Add(b));


            var sfx = SoundEffectMaker.Create(totalSamples,
                soundGenerator);

            return sfx.CreateInstance();
        }

    }
}
