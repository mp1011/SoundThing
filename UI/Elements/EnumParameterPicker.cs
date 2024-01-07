using Microsoft.Xna.Framework;
using SoundThing.Models;
using SoundThing.Services;
using SoundThing.Songs;
using SoundThing.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoundThing.UI.Elements
{
    class EnumParameterPicker<T> : Dropdown<T>
    {
        private T _value;
        
        public EnumParameterPicker(Rectangle position, UIManager uiManager, MusicManager musicManager) 
            : base(position, 16, GetValues(), uiManager, musicManager)
        {
            OnItemSelected += d =>
            {
                _value = d;
                musicManager.ResetSong();
            };
        }

        private static IEnumerable<T> GetValues()
        {
            foreach(T value in Enum.GetValues(typeof(T)))
            {
                yield return value;
            }
        }

        public override void OnSongChanged(Song song)
        {
            var player = song.Players.First();
            var parameter = player
                .Instrument
                .Parameters
                .OfType<Parameter<T>>()
                .FirstOrDefault();

            if (parameter != null)
                parameter.Value = _value;

            base.OnSongChanged(song);
        }

    }
}
