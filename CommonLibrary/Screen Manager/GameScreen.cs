using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CommonLibrary
{
    /// <summary>
    /// Lớp cha cho tấc cả các screen trong game.
    /// </summary>
    public abstract class GameScreen
    {
        #region Fields

        protected List<IVisibleGameEntity> _visibleEntities = new List<IVisibleGameEntity>();
        protected List<IInvisibleGameEntity> _invisibleEntities = new List<IInvisibleGameEntity>();

        #endregion

        #region Properties

        /// <summary>
        /// Cho biết đây có phải là màn hình pop-up hay không
        /// (chẳng hạn như màn hình thông báo)
        /// </summary>
        public bool IsPopup
        {
            get { return _isPopup; }
            protected set { _isPopup = value; }
        }

        bool _isPopup = false;

        public ScreenState ScreenState
        {
            get { return _screenState; }
            protected set { _screenState = value; }
        }

        ScreenState _screenState = ScreenState.Active;

        /// <summary>
        /// Khi một màn hình không còn cần dùng nữa, nó sẽ bị loại bỏ ra khỏi ScreenList.
        /// Và thuộc tính này biểu thị có phải nó sẽ bị loại bỏ hay không.
        /// </summary>
        public bool IsExiting
        {
            get { return _isExiting; }
            protected internal set { _isExiting = value; }
        }

        bool _isExiting = false;

        /// <summary>
        /// Kiểm tra xem màn hình có active không để nhận input từ user
        /// </summary>
        public bool IsActive
        {
            get
            {
                return !_otherScreenHasFocus &&
                       _screenState == ScreenState.Active;
            }
        }

        // Biểu thị có phải ứng dụng game của chúng ta đang là ứng dụng active hay không
        bool _otherScreenHasFocus;

        public ScreenManager ScreenManager
        {
            get { return _screenManager; }
            internal set { _screenManager = value; }
        }

        ScreenManager _screenManager;

        #endregion

        #region Overrided Methods

        public virtual void LoadContent() { }
        public virtual void UnloadContent() { }

        /// <summary>
        /// Hàm này chỉ được gọi khi screen active.
        /// </summary>
        public virtual void HandleInput(InputManager input)
        {
            foreach (IVisibleGameEntity entity in _visibleEntities)
            {
                entity.HandleInput(input);
            }

            foreach (IInvisibleGameEntity entity in _invisibleEntities)
            {
                entity.HandleInput(input);
            }
        }

        public virtual void Draw(GameTime gameTime)
        {
            foreach (IVisibleGameEntity entity in _visibleEntities)
            {
                entity.Draw(gameTime);
            }
        }

        #endregion

        #region Update

        public virtual void Update(GameTime gameTime,
            bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            this._otherScreenHasFocus = otherScreenHasFocus;

            if (_isExiting)
            {
                ScreenManager.RemoveScreen(this);
            }
            else if (coveredByOtherScreen)
            {
                _screenState = ScreenState.Hidden;
            }
            else
            {
                _screenState = ScreenState.Active;
            }

            foreach (IVisibleGameEntity entity in _visibleEntities)
            {
                entity.Update(gameTime);
            }

            foreach (IInvisibleGameEntity entity in _invisibleEntities)
            {
                entity.Update(gameTime);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Yêu cầu screen "tự" loại bỏ khỏi ScreenList
        /// </summary>
        public void ExitScreen()
        {
            _isExiting = true;
        }

        #endregion
    }
}
