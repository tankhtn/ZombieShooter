using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using SkinnedModelData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace CommonLibrary.Graphics
{
    /// <summary>
    /// An encloser for an XNA model that we will use that includes support for
    /// bones, animation, and some manipulations.
    /// </summary>
    public class AnimatedModel : IVisibleGameEntity
    {
        #region Fields

        /// <summary>
        /// The actual underlying XNA model
        /// </summary>
        protected Model _model = null;

        protected BoundingSphere _boundingSphere;

        /// <summary>
        /// Extra data associated with the XNA model
        /// </summary>
        protected ModelExtra _modelExtra = null;

        /// <summary>
        /// The model bones
        /// </summary>
        protected List<Bone> _bones = new List<Bone>();

        /// <summary>
        /// The model asset name
        /// </summary>
        protected string _assetName = "";

        /// <summary>
        /// An associated animation clip player
        /// </summary>
        protected AnimationPlayer _player = null;

        protected Camera _camera;
        protected GraphicsDevice _device;

        #endregion

        #region Properties

        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public virtual Matrix BaseWorld
        {
            get
            {
                return Matrix.CreateScale(Scale) *
                        Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z) *
                        Matrix.CreateTranslation(Position);
            }
        }

        public bool PlayingAnimation { get; set; }

        public Model Model
        {
            get { return _model; }
        }

        public List<Bone> Bones { get { return _bones; } }

        /// <summary>
        /// The model animation clips
        /// </summary>
        public List<AnimationClip> Clips { get { return _modelExtra.Clips; } }

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

        #endregion

        #region Construction and Loading

        public AnimatedModel(string assetName, Vector3 position,
            Vector3 rotation, Vector3 scale, Camera camera, GraphicsDevice device)
        {
            _assetName = assetName;
            _camera = camera;
            _device = device;

            Position = position;
            Rotation = rotation;
            Scale = scale;

            PlayingAnimation = false;
        }

        public void LoadContent(ContentManager content)
        {
            this._model = content.Load<Model>(_assetName);
            _modelExtra = _model.Tag as ModelExtra;
            System.Diagnostics.Debug.Assert(_modelExtra != null);

            ObtainBones();
            BuildBoundingSphere();
        }

        #endregion

        #region Bones Management

        /// <summary>
        /// Get the bones from the model and create a bone class object for
        /// each bone. We use our bone class to do the real animated bone work.
        /// </summary>
        private void ObtainBones()
        {
            _bones.Clear();
            foreach (ModelBone bone in _model.Bones)
            {
                // Create the bone object and add to the heirarchy
                Bone newBone = new Bone(bone.Name, bone.Transform, bone.Parent != null ? _bones[bone.Parent.Index] : null);

                // Add to the bones for this model
                _bones.Add(newBone);
            }
        }

        /// <summary>
        /// Find a bone in this model by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Bone FindBone(string name)
        {
            foreach (Bone bone in Bones)
            {
                if (bone.Name == name)
                    return bone;
            }

            return null;
        }

        #endregion

        #region Animation Management

        /// <summary>
        /// Play an animation clip
        /// </summary>
        /// <param name="clip">The clip to play</param>
        /// <returns>The player that will play this clip</returns>
        public AnimationPlayer PlayClip(AnimationClip clip)
        {
            PlayingAnimation = true;
            // Create a clip player and assign it to this model
            _player = new AnimationPlayer(clip, this);

            return _player;
        }

        public void StopClip()
        {
            foreach (Bone bone in Bones)
            {
                bone.SetCompleteTransform(bone.BindTransform);
            }

            _player = null;
            PlayingAnimation = false;
        }

        #endregion

        #region Updating

        public virtual void Update(GameTime gameTime)
        {
            if (_player != null)
            {
                _player.Update(gameTime);
            }
        }

        public virtual void HandleInput(InputManager input)
        {
        }

        #endregion

        #region Drawing

        public virtual void Draw(GameTime gameTime)
        {
            CommonHelper.ResetRenderState(_device);
            if (_model == null)
                return;

            //
            // Compute all of the bone absolute transforms
            //

            Matrix[] boneTransforms = new Matrix[_bones.Count];

            for (int i = 0; i < _bones.Count; i++)
            {
                Bone bone = _bones[i];
                bone.ComputeAbsoluteTransform();

                boneTransforms[i] = bone.AbsoluteTransform;
            }

            //
            // Determine the skin transforms from the skeleton
            //

            Matrix[] skeleton = new Matrix[_modelExtra.Skeleton.Count];
            for (int s = 0; s < _modelExtra.Skeleton.Count; s++)
            {
                Bone bone = _bones[_modelExtra.Skeleton[s]];
                skeleton[s] = bone.SkinTransform * bone.AbsoluteTransform;
            }

            foreach (ModelMesh modelMesh in _model.Meshes)
            {
                foreach (Effect effect in modelMesh.Effects)
                {
                    if (effect is BasicEffect)
                    {
                        BasicEffect beffect = effect as BasicEffect;
                        beffect.World = boneTransforms[modelMesh.ParentBone.Index] * BaseWorld;
                        beffect.View = _camera.View;
                        beffect.Projection = _camera.Projection;
                        beffect.EnableDefaultLighting();
                        beffect.PreferPerPixelLighting = true;
                    }

                    if (effect is SkinnedEffect)
                    {
                        SkinnedEffect seffect = effect as SkinnedEffect;
                        seffect.World = boneTransforms[modelMesh.ParentBone.Index] * BaseWorld;
                        seffect.View = _camera.View;
                        seffect.Projection = _camera.Projection;
                        seffect.EnableDefaultLighting();
                        seffect.PreferPerPixelLighting = true;
                        seffect.SetBoneTransforms(skeleton);
                    }
                }

                modelMesh.Draw();
            }
        }

        #endregion

        #region Helper Methods

        public static void SetAnim(AnimatedModel model, int animIndex)
        {
            AnimationClip clip = model.Clips[animIndex];
            AnimationPlayer player = model.PlayClip(clip);

            player.Looping = true;
        }

        private void BuildBoundingSphere()
        {
            BoundingSphere sphere = new BoundingSphere(Vector3.Zero, 0);
            Matrix[] modelTransforms;

            modelTransforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(modelTransforms);

            foreach (ModelMesh mesh in Model.Meshes)
            {
                BoundingSphere transformed = mesh.BoundingSphere.Transform(modelTransforms[mesh.ParentBone.Index]);
                sphere = BoundingSphere.CreateMerged(sphere, transformed);
            }
            _boundingSphere = sphere;
        }

        #endregion
    }
}
