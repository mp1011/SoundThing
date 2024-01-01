using Microsoft.Xna.Framework.Audio;
using SoundThing.Extensions;
using SoundThing.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoundThing.Songs
{
    abstract class Song
    {
        protected abstract IEnumerable<Player> CreatePlayers();
        public Player[] Players { get; private set; }

        protected abstract int DefaultBPM { get; }

        private int _bpm;
        public int BPM
        {
            get
            {
                if (_bpm == 0)
                    return DefaultBPM;
                else
                    return _bpm;
            }
            set
            {
                _bpm = value;
            }
        }

        private Scale _scale;
        public Scale Scale
        {
            get => _scale ?? DefaultScale;
            set => _scale = value;
        }

        protected abstract Scale DefaultScale { get; }

        public DynamicAudio[] CreateSounds()
        {
            return Band.CreateSounds(Players);
        }

        public void ResetPlayers()
        {
            Players = CreatePlayers().ToArray();
        }

        public static IEnumerable<Song> GetAll()
        {
            foreach(var type in typeof(Song).Assembly.GetTypes())
            {
                if(typeof(Song).IsAssignableFrom(type)
                    && !type.IsAbstract)
                {
                    if(type != typeof(ChordSong)) //hack
                        yield return (Song)Activator.CreateInstance(type);
                }
            }
        }

        public override string ToString() =>
            GetType().Name.AddSpacesAtCapitals();
    }
}
