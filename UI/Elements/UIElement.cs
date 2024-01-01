using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SoundThing.Services;
using SoundThing.Songs;
using SoundThing.UI.Services;

namespace SoundThing.UI.Elements
{
    abstract class UIElement
    {
        protected bool MouseOver { get; private set; }

        protected Rectangle _region;
        protected UIManager _uiManager;
        protected MusicManager _musicManager;

        public delegate void LeftClicked(UIElement sender);

        public LeftClicked OnLeftClicked;

        public UIElement(Rectangle region, UIManager uiManager, MusicManager musicManager)
        {
            _region = region;
            _uiManager = uiManager;
            _musicManager = musicManager;
        }

        public virtual void OnSongChanged(Song song)
        {

        }

        public virtual void Update(Input input)
        {
            MouseOver = _region.Contains(input.MousePosition);

            if (MouseOver && input.LeftClick)
                OnLeftClicked?.Invoke(this);
        }

        public abstract void Draw(SpriteBatch sprite);
    }
}
