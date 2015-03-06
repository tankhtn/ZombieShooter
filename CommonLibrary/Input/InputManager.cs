using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CommonLibrary
{
    /// <summary>
    /// Quản lý tấc cả các thao tác liên quan đến input trong game.
    /// Không được tạo đối tượng MouseState/KeyboardState ở chỗ khác, muốn thực hiện
    /// một chức năng nào liên quan đến input (chẳng hạn kiểm tra một phím nào đó có được
    /// nhấn hay không) thì phải gọi hàm của lớp này.
    /// </summary>
    public class InputManager
    {
        #region Fields

        KeyboardState _currentKeyboardState;
        KeyboardState _lastKeyboardState;

        MouseState _currentMouseState;
        MouseState _lastMouseState;

        #endregion

        #region Properties

        public bool IsHoldingLeftMouse { get; private set; }

        #endregion

        #region Initialization

        public InputManager()
        {
            _currentKeyboardState = Keyboard.GetState();
            _lastKeyboardState = _currentKeyboardState;

            _currentMouseState = Mouse.GetState();
            _lastMouseState = _currentMouseState;

            IsHoldingLeftMouse = false;
        }

        #endregion

        #region Update

        public void Update(GameTime gameTime)
        {
            _lastKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();

            _lastMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetState();

            if (IsLeftButtonPressed())
                IsHoldingLeftMouse = true;
            if (IsLeftButtonReleased())
                IsHoldingLeftMouse = false;
        }

        #endregion

        #region Helper Methods

        public bool IsNewKeyPress(Keys key, bool continuous)
        {
            if (continuous)
            {
                return _currentKeyboardState.IsKeyDown(key);
            }
            else
            {
                return (_currentKeyboardState.IsKeyUp(key) &&
                    _lastKeyboardState.IsKeyDown(key));
            }
        }

        public bool IsLeftButtonPressed()
        {
            return (_currentMouseState.LeftButton == ButtonState.Pressed &&
                _lastMouseState.LeftButton == ButtonState.Released);
        }

        public bool IsLeftButtonReleased()
        {
            return (_currentMouseState.LeftButton == ButtonState.Released &&
                _lastMouseState.LeftButton == ButtonState.Pressed);
        }

        public bool IsRightButtonPressed()
        {
            return (_currentMouseState.RightButton == ButtonState.Pressed &&
                _lastMouseState.RightButton == ButtonState.Released);
        }

        public bool IsRightButtonReleased()
        {
            return (_currentMouseState.RightButton == ButtonState.Released &&
                _lastMouseState.RightButton == ButtonState.Pressed);
        }

        #endregion

        #region Keyboard Functions

        public bool IsMoveUp()
        {
            return IsNewKeyPress(Keys.W, true) || 
                IsNewKeyPress(Keys.Up, true);
        }

        public bool IsMoveDown()
        {
            return IsNewKeyPress(Keys.S, true) ||
                IsNewKeyPress(Keys.Down, true);
        }

        public bool IsMoveLeft()
        {
            return IsNewKeyPress(Keys.A, true) ||
                IsNewKeyPress(Keys.Left, true);
        }

        public bool IsMoveRight()
        {
            return IsNewKeyPress(Keys.D, true) ||
                IsNewKeyPress(Keys.Right, true);
        }

        public bool IsStopMove()
        {
            return !IsMoveUp() && !IsMoveDown() && !IsMoveLeft() && !IsMoveRight();
        }

        public bool IsZoomIn()
        {
            return IsNewKeyPress(Keys.Z, true);
        }

        public bool IsZoomOut()
        {
            return IsNewKeyPress(Keys.X, true);
        }

        public bool IsSelect()
        {
            return IsNewKeyPress(Keys.Space, false) ||
                   IsNewKeyPress(Keys.Enter, false);
        }

        public bool IsMenuCancel()
        {
            return IsNewKeyPress(Keys.Escape, false);
        }

        public bool IsPauseGame()
        {
            return IsNewKeyPress(Keys.Escape, false);
        }

        public string EnteredDigit()
        {
            return CheckPressDigit();
        }

        #endregion

        #region Mouse Functions

        public Point GetCurrentMousePosition()
        {
            return new Point(_currentMouseState.X, _currentMouseState.Y);
        }

        public int GetWheelDelta()
        {
            return _currentMouseState.ScrollWheelValue - _lastMouseState.ScrollWheelValue;
        }

        public Vector2 GetMouseMoveDelta()
        {
            return new Vector2(
                _currentMouseState.X - _lastMouseState.X,
                _currentMouseState.Y - _lastMouseState.Y);
        }

        #endregion

        #region Stuff Methods

        public string CheckPressDigit()
        {
            if (IsNewKeyPress(Keys.D0, false))
                return "0";
            if (IsNewKeyPress(Keys.D1, false))
                return "1";
            if (IsNewKeyPress(Keys.D2, false))
                return "2";
            if (IsNewKeyPress(Keys.D3, false))
                return "3";
            if (IsNewKeyPress(Keys.D4, false))
                return "4";
            if (IsNewKeyPress(Keys.D5, false))
                return "5";
            if (IsNewKeyPress(Keys.D6, false))
                return "6";
            if (IsNewKeyPress(Keys.D7, false))
                return "7";
            if (IsNewKeyPress(Keys.D8, false))
                return "8";
            if (IsNewKeyPress(Keys.D9, false))
                return "9";
            if (IsNewKeyPress(Keys.OemPeriod, false))
                return ".";
            if (IsNewKeyPress(Keys.OemMinus, false))
                return "-";
            if (IsNewKeyPress(Keys.Enter, false))
                return "Enter";

            return null;
        }

        #endregion
    }
}
