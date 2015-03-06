using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using CommonLibrary;

namespace ZombieShooter
{
    public abstract class GameLevel
    {
        #region Fields

        protected ScreenManager _screenManager;
        protected ContentManager _content;

        protected List<IVisibleGameEntity> _visibleEntities = new List<IVisibleGameEntity>();
        protected List<IInvisibleGameEntity> _invisibleEntities = new List<IInvisibleGameEntity>();

        protected SpriteBatch _spriteBatch;
        protected GraphicsDevice _device;

        protected float _gameTime;
        #endregion

        #region Initialization

        public GameLevel(ScreenManager screenManager, GraphicsDevice device)
        {
            _screenManager = screenManager;
            _device = device;
            _spriteBatch = screenManager.SpriteBatch;
        }

        public virtual void LoadContent()
        {
            if (_content == null)
                _content = new ContentManager(_screenManager.Game.Services, "Content");
        }

        public virtual void UnloadContent()
        {
            _content.Unload();
        }

        #endregion

        #region Update and Draw

        /// <summary>
        /// This method is always called regardless of whether gameplay screen is hidden by pop-up screen
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime)
        {
            /*foreach (IVisibleGameEntity entity in _visibleEntities)
            {
                entity.Update(gameTime);
            }*/

            for (int i = 0; i < _visibleEntities.Count; i++)
                _visibleEntities[i].Update(gameTime);

            foreach (IInvisibleGameEntity entity in _invisibleEntities)
            {
                entity.Update(gameTime);
            }
        }

        /// <summary>
        /// This method is called only if gameplay screen is active
        /// </summary>
        /// <param name="inputState"></param>
        public virtual void UpdateInput(InputManager input)
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

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (IVisibleGameEntity entity in _visibleEntities)
            {
                entity.Draw(gameTime);
            }
        }

        #endregion
    }
}
