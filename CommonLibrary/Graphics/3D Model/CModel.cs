using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace CommonLibrary.Graphics
{
    public class CModel : IVisibleGameEntity
    {
        #region Fields

        private GraphicsDevice _graphicsDevice;
        private Matrix[] _modelTransforms;
        private Camera _camera;

        private BoundingSphere _boundingSphere;

        private float _elapsedTime = 0;

        #endregion

        #region Properties

        /*********** Test **********/
        public Camera Camera { get { return _camera; } }
        /*********** Test **********/

        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public Matrix? AbsoluteWorld { get; set; }

        public Model Model { get; set; }

        public BoundingSphere BoundingSphere
        {
            get
            {
                Matrix worldTranform = Matrix.CreateScale(Scale) * Matrix.CreateTranslation(Position);
                BoundingSphere transformed = _boundingSphere;
                transformed = transformed.Transform(worldTranform);

                return transformed;
            }
        }

        public GraphicsDevice Device { get { return _graphicsDevice; } }
        public string Name { get; set; }

        #endregion

        #region Construction

        public CModel(Model model, Vector3 position, Vector3 rotation,
            Vector3 scale, Camera camera, GraphicsDevice graphicsDevice)
        {
            Model = model;

            _modelTransforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(_modelTransforms);
            
            Position = position;
            Rotation = rotation;
            Scale = scale;

            _camera = camera;
            _graphicsDevice = graphicsDevice;

            BuildBoundingSphere();
            GenerateTags();
        }

        #endregion

        #region Initialization

        private void BuildBoundingSphere()
        {
            BoundingSphere sphere = new BoundingSphere(Vector3.Zero, 0);
            foreach (ModelMesh mesh in Model.Meshes)
            {
                _modelTransforms[mesh.ParentBone.Index].Translation = Vector3.Zero;
                BoundingSphere transformed = mesh.BoundingSphere.Transform(_modelTransforms[mesh.ParentBone.Index]);
                sphere = BoundingSphere.CreateMerged(sphere, transformed);
            }
            _boundingSphere = sphere;
        }

        private void GenerateTags()
        {
            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    if (part.Effect is BasicEffect)
                    {
                        BasicEffect effect = part.Effect as BasicEffect;
                        MeshTag tag = new MeshTag(effect.DiffuseColor, effect.Texture, effect.SpecularPower);
                        part.Tag = tag;
                    }
                }
            }
        }

        #endregion

        #region Update

        public virtual void Update(GameTime gameTime)
        {
            _elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public virtual void HandleInput(InputManager input)
        {
        }

        #endregion

        #region Draw

        public virtual void Draw(GameTime gameTime)
        {
            CommonHelper.ResetRenderState(_graphicsDevice);
            Matrix baseWorld = Matrix.CreateScale(Scale) *
                Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z) *
                Matrix.CreateTranslation(Position);

            Model.CopyAbsoluteBoneTransformsTo(_modelTransforms);

            foreach (ModelMesh mesh in Model.Meshes)
            {
                _modelTransforms[mesh.ParentBone.Index].Translation = Vector3.Zero;
                Matrix localWorld = _modelTransforms[mesh.ParentBone.Index] * baseWorld;
                if (AbsoluteWorld != null)
                    localWorld = baseWorld * AbsoluteWorld.Value;

                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    Effect effect = meshPart.Effect;

                    if (effect is BasicEffect)
                    {
                        ((BasicEffect)effect).World = localWorld;
                        ((BasicEffect)effect).View = _camera.View;
                        ((BasicEffect)effect).Projection = _camera.Projection;
                        ((BasicEffect)effect).EnableDefaultLighting();
                    }
                    else
                    {
                        EffectHelper.SetEffectParameter(effect, "World", localWorld);
                        EffectHelper.SetEffectParameter(effect, "View", _camera.View);
                        EffectHelper.SetEffectParameter(effect, "Projection", _camera.Projection);
                        EffectHelper.SetEffectParameter(effect, "CameraPosition", _camera.Position);
                        EffectHelper.SetEffectParameter(effect, "ElapsedTime", _elapsedTime);
                    }
                }
                mesh.Draw();
            }

            //DrawBoundingSphere();
        }

        protected virtual void DrawBoundingSphere()
        {
            BoundingSphereRenderer.Draw(BoundingSphere, _camera.View, _camera.Projection);
        }

        #endregion

        #region Effect Helpers

        public void CacheEffects()
        {
            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    (part.Tag as MeshTag).CachedEffect = part.Effect;
                }
            }
        }

        public void RestoreEffects()
        {
            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    if ((part.Tag as MeshTag).CachedEffect != null)
                    {
                        part.Effect = (part.Tag as MeshTag).CachedEffect;
                    }
                }
            }
        }

        public void SetModelEffectParameter(string paramName, object val)
        {
            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    Effect effect = meshPart.Effect;
                    EffectHelper.SetEffectParameter(effect, paramName, val);
                }
            }
        }

        public void SetModelEffect(Effect effect, bool copyEffect)
        {
            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    Effect toSet = effect;
                    if (copyEffect)
                    {
                        toSet = effect.Clone();
                    }

                    MeshTag tag = part.Tag as MeshTag;
                    if (tag.Texture != null)
                    {
                        EffectHelper.SetEffectParameter(toSet, "BasicTexture", tag.Texture);
                        EffectHelper.SetEffectParameter(toSet, "TextureEnabled", true);
                    }
                    else
                    {
                        EffectHelper.SetEffectParameter(toSet, "TextureEnabled", false);
                    }
                    EffectHelper.SetEffectParameter(toSet, "DiffuseColor", tag.Color);
                    EffectHelper.SetEffectParameter(toSet, "SpecularPower", tag.SpecularPower);

                    part.Effect = toSet;
                }
            }
        }

        #endregion
    }
}
