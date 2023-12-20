using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SoundThing.UI.Services
{
    class Input
    {
        private MouseState _lastMouseState, _currentMouseState;
        private KeyboardState _lastKeyboardState, _currentKeyboardState;
        private Point _lastMousePosition;

        public Point MousePosition => _currentMouseState.Position;

        public Point MouseVector => MousePosition - _lastMousePosition;

        public bool LeftClick => _currentMouseState.LeftButton == ButtonState.Pressed 
            && _lastMouseState.LeftButton == ButtonState.Released;
        public bool LeftReleased => _currentMouseState.LeftButton == ButtonState.Released
            && _lastMouseState.LeftButton == ButtonState.Pressed;

        public bool LeftDown => _currentMouseState.LeftButton == ButtonState.Pressed;
        public void Update()
        {
            _lastKeyboardState = _currentKeyboardState;
            _lastMouseState = _currentMouseState;
            _lastMousePosition = MousePosition;
            _currentKeyboardState = Keyboard.GetState();
            _currentMouseState = Mouse.GetState();
        }
    }
}
