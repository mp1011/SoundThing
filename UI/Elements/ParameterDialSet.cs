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
    class ParameterDialSet : ElementList<Parameter, Dial>
    {
        private Parameter[] _currentParameters = Array.Empty<Parameter>();

        protected override Orientation Orientation => Orientation.Horizontal;
        public ParameterDialSet(Rectangle firstPosition, int spacing, UIManager uiManager, MusicManager musicManager)
           : base(firstPosition, spacing, Array.Empty<Parameter>(), uiManager, musicManager)
        {
        }

        protected override Dial ToUIElement(Parameter data) =>
            new ParameterDial(data,
                NextRegion(),
                _uiManager,
                _musicManager);

        public override void OnSongChanged(Song song)
        {
            var newParameters = song
                .Players
                .SelectMany(p => p.Instrument.Parameters)
                .OfType<Parameter>()
                .Select((p, index) =>
                {
                    if (index < _currentParameters.Length
                        && p.Name == _currentParameters[index].Name)
                    {
                        p.Value = _currentParameters[index].Value;
                        p.Mod = 1.0;
                    }
                    return p;
                })
                .ToArray();

            _currentParameters = newParameters;
            SetData(newParameters);
        }
    }
}
