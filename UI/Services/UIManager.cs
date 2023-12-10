using Microsoft.Xna.Framework.Graphics;
using SoundThing.UI.Elements;
using System.Collections.Generic;

namespace SoundThing.UI.Services
{
    class UIManager
    {
        private Input _input = new Input();

        public SpriteFont Font { get; set; }

        public Texture2D ElementBackground { get; set; }

        private List<UIElement> _uiElements = new List<UIElement>();


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
    }
}
