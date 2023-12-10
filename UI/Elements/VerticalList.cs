using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SoundThing.Services;
using SoundThing.UI.Services;
using System.Collections.Generic;
using System.Linq;

namespace SoundThing.UI.Elements
{
    abstract class VerticalList<TData, TElement> : UIElement
        where TElement : UIElement
        where TData : IActivateable
    {
        private TElement[] _elements;
        private Rectangle _nextRegion;
        private int _spacing;

        public VerticalList(
            Rectangle firstPosition,
            int spacing,
            IEnumerable<TData> data,
            UIManager uiManager,
            MusicManager musicManager) : base(Rectangle.Empty, uiManager, musicManager)
        {
            _spacing = spacing;
            _nextRegion = firstPosition;
            _elements = data
                .Select(ToUIElement)
                .ToArray();


        }

        protected abstract TElement ToUIElement(TData data);

        public override void Update(Input input)
        {
            foreach (var element in _elements)
                element.Update(input);
        }

        public override void Draw(SpriteBatch sprite)
        {
            foreach (var element in _elements)
                element.Draw(sprite);
        }

        protected Rectangle NextRegion()
        {
            var thisRegion = _nextRegion;
            _nextRegion = new Rectangle(
                _nextRegion.X, 
                _nextRegion.Bottom + _spacing, 
                _nextRegion.Width,
                _nextRegion.Height);

            return thisRegion;
        }
    }

    class VerticalButtonList<TData> : VerticalList<TData, Button>
        where TData : IActivateable
    {
        public VerticalButtonList(Rectangle firstPosition, int spacing, IEnumerable<TData> data, UIManager uiManager, MusicManager musicManager) 
            : base(firstPosition, spacing, data, uiManager, musicManager)
        {
        }

        protected override Button ToUIElement(TData data) =>
            new Button(
                _uiManager, 
                _musicManager, 
                data.ToString(), 
                NextRegion(), 
                () => data.Activate(_musicManager));
    }
}
