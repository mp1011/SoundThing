using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SoundThing.Services;
using SoundThing.UI.Services;
using System.Collections.Generic;

namespace SoundThing.UI.Elements
{
    class Dropdown<TData> : UIElement
    {
        private VerticalButtonList<TData> _items;
        private Button _button;
        private State _state;
        private bool _clickHandled = false;

        public delegate void ItemSelected(TData data);

        public ItemSelected OnItemSelected;

        enum State
        {
            Closed,
            Open
        }

        public Dropdown(
            Rectangle position,
            int spacing,
            IEnumerable<TData> data,
            UIManager uiManager,
            MusicManager musicManager)
            : base(position, uiManager, musicManager)
        {
            _items = new VerticalButtonList<TData>(position, spacing, data, uiManager, musicManager);
            _button = new Button(uiManager, musicManager, "Please select...", position);

            OnLeftClicked += (UIElement e) =>
            {
                if (_state == State.Closed)
                    _state = State.Open;
                else
                    _state = State.Closed;
            };

            _items.OnLeftClickedItem += (UIElement e, TData data) =>
            {
                OnItemSelected?.Invoke(data);
                _state = State.Closed;
                _button.Text = data.ToString();
                _clickHandled = true;
            };
        }

        public override void Draw(SpriteBatch sprite)
        {
            if (_state == State.Closed)
                _button.Draw(sprite);
            else if(_state == State.Open)
                _items.Draw(sprite);
        }

        public override void Update(Input input)
        {
            _clickHandled = false;
            if (_state == State.Open)
                _items.Update(input);

            if(!_clickHandled)
                base.Update(input);
        }
    }
}
