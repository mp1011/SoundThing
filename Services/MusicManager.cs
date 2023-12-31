﻿using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Linq;
using Song = SoundThing.Songs.Song;

namespace SoundThing.Services
{
    class MusicManager
    {
        public delegate void SongChangedEvent(Song newSong);

        public SongChangedEvent SongChanged;

        private DynamicAudio[] _sounds = new DynamicAudio[] { };
        
        public Song[] Songs { get; }

        public Song CurrentSong { get; private set; }

        public MusicManager()
        {
            Songs = Song.GetAll().ToArray();
        }

        public void Update()
        {
            if (_sounds.All(p => p.State == SoundState.Stopped))
            {
                foreach (var sound in _sounds)
                    sound.Play();
            }
        }

        public void Play(Song song)
        {             
            CurrentSong = song;
            song.ResetPlayers(); 
            SongChanged?.Invoke(song);
            foreach(var sound in _sounds)
                sound.Stop();

            foreach (var player in song.Players)
                player.AdjustEventsToEnvelope();

            _sounds = song.CreateSounds();
        }

        public void ResetSong() => Play(CurrentSong);
    }
}
