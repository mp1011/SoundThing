using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SoundThing.Services;
using SoundThing.UI.Services;
using System;

namespace SoundThing.UI.Elements
{
    class Button : UIElement
    {
        private Vector2 _pad = new Vector2(4, 4);

        public string Text { get; set; }

        public Button(UIManager uiManager, MusicManager musicManager, string text, Rectangle region)
            :base(region, uiManager, musicManager)
        {
            Text = text;
        }

        public override void Draw(SpriteBatch sprite)
        {
            if(MouseOver)
                sprite.Draw(_uiManager.Textures[TextureKey.ElementBackground], _region, Color.LightGreen);
            else
                sprite.Draw(_uiManager.Textures[TextureKey.ElementBackground], _region, Color.White);

            sprite.DrawString(_uiManager.Font, Text, new Vector2(_region.X + _pad.X, _region.Y + _pad.Y), Color.Black);
        }
    }
}
