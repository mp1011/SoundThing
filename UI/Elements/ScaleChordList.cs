using Microsoft.Xna.Framework;
using SoundThing.Models;
using SoundThing.Services;
using SoundThing.Songs;
using SoundThing.UI.Models;
using SoundThing.UI.Services;
using System;
using System.Linq;

namespace SoundThing.UI.Elements
{
    class ScaleChordList : ElementList<ScaleChord, Button>
    {
        public ScaleChordList(Rectangle firstPosition, UIManager uiManager, MusicManager musicManager) 
            : base(firstPosition, 20, Array.Empty<ScaleChord>(), uiManager, musicManager)
        {
        }

        protected override Orientation Orientation => Orientation.Vertical;

        protected override Button ToUIElement(ScaleChord data)
        {
            return new Button(_uiManager, _musicManager, data.ToString(), NextRegion(),
                () =>
                {
                    var song = new ChordSong(data);
                    _musicManager.Play(song);
                });
        }

        public override void OnSongChanged(Song song)
        {
            SetData(Enumerable.Range(1, song.Scale.Steps.Length)
                .Select(i => new ScaleChord(song.Scale, i, song.Scale.GetChord(i))));
        }
    }
}
