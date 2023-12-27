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
    class ChordList : ElementList<Chord, Button>
    {
        public ChordList(Rectangle firstPosition, UIManager uiManager, MusicManager musicManager)
            : base(firstPosition, 20, Array.Empty<Chord>(), uiManager, musicManager)
        {
        }

        protected override Orientation Orientation => Orientation.Vertical;

        protected override Button ToUIElement(Chord data)
        {
            return new Button(_uiManager, _musicManager, data.ToString(), NextRegion(),
                () =>
                {
                    var scale = new ChromaticScale(new ScaleParameters(data.Notes.First()));
                    var song = new ChordSong(new ScaleChord(scale, 0, data));                       
                    _musicManager.Play(song);
                });
        }

        public override void OnSongChanged(Song song)
        {
            SetData(Chord.CreateChordTypes(song.Scale.Root));
        }
    }

}
