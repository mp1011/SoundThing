using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SoundThing.Models;
using SoundThing.Services;
using SoundThing.Songs;
using SoundThing.UI.Models;
using SoundThing.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoundThing.UI.Elements
{
    abstract class ElementList<TData, TElement> : UIElement
        where TElement : UIElement
    {
        private TElement[] _elements;
        private Rectangle _nextRegion;
        private Rectangle _firstRegion;
        private int _spacing;

        public delegate void LeftClickedItem(UIElement element, TData data);

        public LeftClickedItem OnLeftClickedItem;

        protected abstract Orientation Orientation { get; }

        public ElementList(
            Rectangle firstPosition,
            int spacing,
            IEnumerable<TData> data,
            UIManager uiManager,
            MusicManager musicManager) : base(Rectangle.Empty, uiManager, musicManager)
        {
            _spacing = spacing;
            _nextRegion = firstPosition;
            _firstRegion = firstPosition;
            SetData(data);
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

            if (Orientation == Orientation.Vertical)
            {
                _nextRegion = new Rectangle(
                    _nextRegion.X,
                    _nextRegion.Bottom + _spacing,
                    _nextRegion.Width,
                    _nextRegion.Height);
            }
            else
            {
                _nextRegion = new Rectangle(
                    _nextRegion.Right + _spacing,
                    _nextRegion.Y,
                    _nextRegion.Width,
                    _nextRegion.Height);
            }
            return thisRegion;
        }

        public void SetData(IEnumerable<TData> data)
        {
            _nextRegion = _firstRegion;
            _elements = data
                .Select(data =>
                {
                    var element = ToUIElement(data);
                    element.OnLeftClicked += (UIElement e) =>
                    {
                        OnLeftClickedItem?.Invoke(e, data);
                    };
                    return element;
                })
                .ToArray();
        }
    }

    class VerticalButtonList<TData> : ElementList<TData, Button>
    {
        protected override Orientation Orientation => Orientation.Vertical;

        public VerticalButtonList(Rectangle firstPosition, int spacing, IEnumerable<TData> data, UIManager uiManager, MusicManager musicManager) 
            : base(firstPosition, spacing, data, uiManager, musicManager)
        {
        }

        protected override Button ToUIElement(TData data) =>
            new Button(
                _uiManager, 
                _musicManager, 
                data.ToString(), 
                NextRegion());
    }

   
}
