using Microsoft.Xna.Framework.Audio;
using SoundThing.Extensions;
using SoundThing.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoundThing.Songs
{
    abstract class Song : IActivateable
    {
        protected abstract IEnumerable<Player> Players { get; }
        protected abstract int BPM { get; }

        private Scale _scale;
        public Scale Scale
        {
            get => _scale ?? DefaultScale;
            set => _scale = value;
        }

        protected abstract Scale DefaultScale { get; }

        public DynamicAudio[] CreateSounds()
        {
            return Band.CreateSounds(Players.ToArray());
        }

        public static IEnumerable<Song> GetAll()
        {
            foreach(var type in typeof(Song).Assembly.GetTypes())
            {
                if(typeof(Song).IsAssignableFrom(type)
                    && !type.IsAbstract)
                {
                    yield return (Song)Activator.CreateInstance(type);
                }
            }
        }

        public override string ToString() =>
            GetType().Name.AddSpacesAtCapitals();

        public void Activate(MusicManager manager)
            => manager.Play(this);
    }
}
