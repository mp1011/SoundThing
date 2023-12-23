using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SoundThing.Models;
using SoundThing.Songs;
using SoundThing.UI.Elements;
using System.Collections.Generic;
using System.Linq;

namespace SoundThing.UI.Services
{
    enum TextureKey
    {
        ElementBackground,
        Dial
    }

    class UIManager
    {
        private Input _input = new Input();
        private List<UIElement> _uiElements = new List<UIElement>();

        public SpriteFont Font { get; set; }
        public Dictionary<TextureKey, Texture2D> Textures { get; }

        public UIManager()
        {
            Textures = new Dictionary<TextureKey, Texture2D>();
        }

        public void Add(UIElement element)
        {
            _uiElements.Add(element);
        }

        public void Update()
        {
            _input.Update();
            foreach (var element in _uiElements)
                element.Update(_input);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var element in _uiElements)
                element.Draw(spriteBatch);
        }

        public void OnSongChanged(Song song)
        {
            foreach (var element in _uiElements)
                element.OnSongChanged(song);
        }
    }
}
