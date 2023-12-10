using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SoundThing.UI.Services
{
    class Input
    {
        private MouseState _lastMouseState, _currentMouseState;
        private KeyboardState _lastKeyboardState, _currentKeyboardState;

        public Point MousePosition => _currentMouseState.Position;

        public bool LeftClick => _currentMouseState.LeftButton == ButtonState.Pressed 
            && _lastMouseState.LeftButton == ButtonState.Released;

        public void Update()
        {
            _lastKeyboardState = _currentKeyboardState;
            _lastMouseState = _currentMouseState;
            _currentKeyboardState = Keyboard.GetState();
            _currentMouseState = Mouse.GetState();
        }
    }
}
