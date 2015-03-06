using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary;
using Microsoft.Xna.Framework;

namespace ZombieShooter
{
    public class ProductGroup : IVisibleGameEntity
    {
        public List<Product> _productList;

        public ProductGroup()
        {
            _productList = new List<Product>();
        }

        public void AddProduct(Product product)
        {
            _productList.Add(product);
        }

        public void RemoveProduct(Product product)
        {
            _productList.Remove(product);
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            for (int i = 0; i < _productList.Count; i++)
                _productList[i].Update(gameTime);
        }

        public void HandleInput(InputManager input)
        {
        }

        public void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            for (int i = 0; i < _productList.Count; i++)
                _productList[i].Draw(gameTime);
        }

        public BoundingSphere BoundingSphere
        {
            get { throw new NotImplementedException(); }
        }
    }
}
