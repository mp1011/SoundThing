using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SoundThing.Services;
using SoundThing.UI.Services;
using System;

namespace SoundThing.UI.Elements
{
    class Dial : UIElement
    {
        private float _rotation = 0;
        private readonly float _min, _max;
        private Action<float> _valueChanged;
        private const float _rotationMin = -MathHelper.Pi + 0.5f;
        private const float _rotationMax = MathHelper.Pi - 0.5f;

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
        }
        
        public Dial(
            Rectangle region, 
            UIManager uiManager, 
            MusicManager musicManager,
            float min, 
            float max,
            Action<float> valueChanged)
            : base(region, uiManager, musicManager)
        {
            _min = min;
            _max = max;
            _valueChanged = valueChanged;
        }

        public override void Draw(SpriteBatch sprite)
        {
            sprite.Draw(
                texture: _uiManager.Textures[TextureKey.Dial],
                destinationRectangle: _region,
                sourceRectangle: null,
                color: Color.White,
                rotation: _rotation,
                origin: new Vector2(56,56),
                effects: SpriteEffects.None,
                layerDepth: 0);

            sprite.DrawString(_uiManager.Font, Value.ToString("0.00"), 
                new Vector2(_region.X, _region.Y + 32), Color.Black);

        }

        public override void Update(Input input)
        {
            if(input.LeftReleased)
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

            _rotation += input.MouseVector.X * 0.01f;

            if (_rotation < _rotationMin)
                _rotation = _rotationMin;
            else if (_rotation > _rotationMax)
                _rotation = _rotationMax;

            
        }

    }
}
