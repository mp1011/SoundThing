using Microsoft.Xna.Framework;
using SoundThing.Services;
using SoundThing.Songs;
using SoundThing.UI.Services;
using System;

namespace SoundThing.UI.Elements
{
    class BPMDial : Dial
    {
        public BPMDial(Rectangle region, UIManager uiManager, MusicManager musicManager) 
            : base(region, uiManager, musicManager, 40, 300, "BPM", "0.0", v =>
            {
                if (musicManager.CurrentSong == null)
                    return;

                musicManager.CurrentSong.BPM = (int)v;
                musicManager.ResetSong();
            })
        {
        }

        public override void OnSongChanged(Song song)
        {
            Value = song.BPM;
        }
    }
}
