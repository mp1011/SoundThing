using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SoundThing.Services;
using SoundThing.UI.Services;
using System;

namespace SoundThing.UI.Elements
{
    class Button : UIElement
    {
        private string _text;
        public Action _onClick;
        private Vector2 _pad = new Vector2(4, 4);

        public Button(UIManager uiManager, MusicManager musicManager, string text, Rectangle region, Action onClick)
            :base(region, uiManager, musicManager)
        {
            _text = text;
            _onClick = onClick;
        }

        protected override void OnLeftClick()
        {
            _onClick();
        }

        public override void Draw(SpriteBatch sprite)
        {
            if(MouseOver)
                sprite.Draw(_uiManager.Textures[TextureKey.ElementBackground], _region, Color.LightGreen);
            else
                sprite.Draw(_uiManager.Textures[TextureKey.ElementBackground], _region, Color.White);

            sprite.DrawString(_uiManager.Font, _text, new Vector2(_region.X + _pad.X, _region.Y + _pad.Y), Color.Black);
        }
    }
}
