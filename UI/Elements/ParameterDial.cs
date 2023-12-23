using Microsoft.Xna.Framework;
using SoundThing.Models;
using SoundThing.Services;
using SoundThing.UI.Services;
using System;

namespace SoundThing.UI.Elements
{
    class ParameterDial : Dial
    {
        public ParameterDial(
            Parameter parameter, 
            Rectangle region, 
            UIManager uiManager, 
            MusicManager musicManager) : 
            base(
                region, 
                uiManager,
                musicManager,
                (float)parameter.Min,
                (float)parameter.Max,
                parameter.Name, 
                parameter.Format, 
                v=> { 
                    parameter.Value = v;
                    musicManager.ResetSong();                
                })
        {
            Value = parameter;
        }

        public Parameter Parameter { get; }
    }
}
