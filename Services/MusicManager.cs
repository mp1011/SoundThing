using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Linq;
using Song = SoundThing.Songs.Song;

namespace SoundThing.Services
{
    class MusicManager
    {
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
            foreach(var sound in _sounds)
                sound.Stop();

            _sounds = song.CreateSounds();
        }
    }
}
