using SoundThing.Songs;
using System;

namespace SoundThing.Services
{
    class SongChanger
    {
        private readonly Action<Song> _action;
        private readonly string _label;

        public SongChanger(string label, Action<Song> action)
        {
            _action = action;
            _label = label;
        }

        public void Activate(MusicManager manager)
        {
            _action(manager.CurrentSong);
            manager.Play(manager.CurrentSong);
        }

        public override string ToString() => _label;
    }
}
