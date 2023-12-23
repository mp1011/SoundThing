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
                .Select(ToUIElement)
                .ToArray();
        }
    }

    class VerticalButtonList<TData> : ElementList<TData, Button>
        where TData : IActivateable
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
                NextRegion(), 
                () => data.Activate(_musicManager));
    }

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
                .Select((p, index) =>
                {
                    if(index < _currentParameters.Length
                        && p.Name == _currentParameters[index].Name)
                    {
                        p.Value = _currentParameters[index].Value;
                    }
                    return p;
                })
                .ToArray();

            _currentParameters = newParameters;
            SetData(newParameters);
        }
    }
}
