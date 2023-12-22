using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SoundThing.Services;
using SoundThing.Songs;
using SoundThing.UI.Services;
using System;

namespace SoundThing.UI.Elements
{
    class Dial : UIElement
    {
        private readonly int _activeRegionPad = 16;
        private readonly string _format;
        private readonly string _label;
        private float _rotation = 0;
        private readonly float _min, _max;
        private Action<float> _valueChanged;
        private const float _rotationMin = -MathHelper.Pi + 0.5f;
        private const float _rotationMax = MathHelper.Pi - 0.5f;

        private Point _mouseClickInitialPosition;

        private float _lastValue;
        public float Value
        {
            get
            {
                var range = _max - _min;
                var rotationRange = _rotationMax - _rotationMin;

                var rotationValue = (_rotation - _rotationMin) / rotationRange;
                return _min + (range * rotationValue);
            }
            private set
            {
                var range = _max - _min;
                var rotationRange = _rotationMax - _rotationMin;

                var valuePercent = (value - _min) / range;
                _rotation = _rotationMin + (rotationRange * valuePercent);
            }
        }
        
        public Dial(
            Rectangle region, 
            UIManager uiManager, 
            MusicManager musicManager,
            float min, 
            float max,
            string label,
            string format,
            Action<float> valueChanged)
            : base(region, uiManager, musicManager)
        {
            _format = format;
            _label = label;
            _min = min;
            _max = max;
            _valueChanged = valueChanged;
        }

        public override void Draw(SpriteBatch sprite)
        {
           // sprite.Draw(_uiManager.Textures[TextureKey.ElementBackground], _region, Color.LightGreen);

            var drawRegion = new Rectangle(_region.X + (_region.Width/2), _region.Y + (_region.Height / 2), _region.Width, _region.Height);
            sprite.Draw(
                texture: _uiManager.Textures[TextureKey.Dial],
                destinationRectangle: drawRegion,
                sourceRectangle: null,
                color: Color.White,
                rotation: _rotation,
                origin: new Vector2(56,56),
                effects: SpriteEffects.None,
                layerDepth: 0);

            sprite.DrawString(_uiManager.Font, $"{_label}={Value.ToString(_format)}",
                new Vector2(_region.X, _region.Y + _region.Height), Color.Black);
        }

        public override void Update(Input input)
        {
            if (input.LeftReleased)
            {
                if (Value != _lastValue)
                {
                    _valueChanged(Value);
                    _lastValue = Value;
                }
                return;
            }

            if (!input.LeftDown)
                return;

            if (input.LeftClick)
                _mouseClickInitialPosition = input.MousePosition;

            var activeRegion = new Rectangle(
                _region.X - +_activeRegionPad,
                _region.Y - _activeRegionPad,
                _region.Width + (_activeRegionPad * 2),
                _region.Height + (_activeRegionPad * 2));

            if (!activeRegion.Contains(_mouseClickInitialPosition))
                return;

            var mouseDialVector = input.MousePosition - _region.Center;

            var mouseRadians = Math.Atan2(mouseDialVector.Y, mouseDialVector.X)
                               + MathHelper.Pi / 2.0;

            _rotation += (float)((input.MouseVector.X * Math.Cos(mouseRadians) * 0.05f) +
                                 (input.MouseVector.Y * Math.Sin(mouseRadians) * 0.05f));

            if (_rotation < _rotationMin)
                _rotation = _rotationMin;
            else if (_rotation > _rotationMax)
                _rotation = _rotationMax;
        }


        public override void OnSongChanged(Song song)
        {
            Value = song.BPM;
        }

    }
}
