using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace CommonLibrary.Graphics
{
    /* We're going to divide terrain into multiple square patches.
     * Then, only draw patches within view of camera. */
    public class Terrain : IVisibleGameEntity
    {
        #region Enumeration

        public enum TerrainType { Mountain, Ground, River, OutOfTheBox}

        #endregion

        #region Fields

        int _width, _height;        // Number of vertices on x and z axes
        float _cellSize;            // Distance between vertices on x and z axes

        int _patchSize;            // Size of patch (by number of vertices)
        int _numPatchesWidth;
        int _numPatchesHeight;
        TerrainPatch[,] _patches;  // Divide terrain into grid of patches

        Texture2D _heightMap;
        float[,] _heights;      // Array of vertex heights
        float _maxHeight;       // Maximum height of terrain
        float _baseHeight;      // Height of ground

        Texture2D _baseTexture;
        Texture2D _rockTexture;
        Texture2D _waterTexture;
        Texture2D _weightMap;
        float _textureTiling;

        /* Common index buffer for all terrain patch 
           since index buffer for all terrain patch is the same */
        int[] _indices;
        int _nIndices;
        IndexBuffer _indexBuffer;

        Effect _effect;
        Camera _camera;
        GraphicsDevice _graphicsDevice;

        #endregion

        #region Properties

        public float BaseHeight { get { return _baseHeight; } }
        public int TotalPatches { get { return _numPatchesWidth * _numPatchesHeight; } }
        public int RenderedPatches { get; set; }
        public BoundingSphere BoundingSphere
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region Construction

        public Terrain(TerrainConfig config)
        {
            _heightMap = config.HeightMap;
            _baseTexture = config.BaseTexture;
            _rockTexture = config.RockTexture;
            _waterTexture = config.WaterTexture;
            _cellSize = config.CellSize;
            _patchSize = config.PatchSize;
            _maxHeight = config.MaxHeight;
            _textureTiling = config.TextureTiling;
            _effect = config.Effect;
            _camera = config.Camera;
            _graphicsDevice = config.Device;

            RenderedPatches = 0;
            _width = _heightMap.Width;
            _height = _heightMap.Height;
            _baseHeight = config.BaseColor.X * _maxHeight;
            _numPatchesWidth = (_width - 1) / (_patchSize - 1);
            _numPatchesHeight = (_height - 1) / (_patchSize - 1);
            _nIndices = (_patchSize - 1) * (_patchSize - 1) * 6;

            WeightMapCreator wmCreator = new WeightMapCreator(_heightMap, 
                config.BaseColor, config.WeightMapEffect, _graphicsDevice);
            _weightMap = wmCreator.WeightMap;

            _indexBuffer = new IndexBuffer(_graphicsDevice, IndexElementSize.ThirtyTwoBits,
                _nIndices, BufferUsage.WriteOnly);

            GetHeights();
            CreateCommonIndices();
            CreatePatches();

            _indexBuffer.SetData<int>(_indices);
        }

        #endregion

        #region Creaation

        private void GetHeights()
        {
            Color[] heightMapData = new Color[_width * _height];
            _heightMap.GetData<Color>(heightMapData);

            _heights = new float[_width, _height];

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    // Get color value (0 - 255)
                    float amt = heightMapData[y * _width + x].R;

                    // Scale to (0, 1)
                    amt /= 255.0f;

                    // Multiply by max height to get final height
                    _heights[x, y] = amt * _maxHeight;
                }
            }
        }

        private void CreateCommonIndices()
        {
            _indices = new int[_nIndices];

            int i = 0;

            // For each cell
            for (int x = 0; x < _patchSize - 1; x++)
                for (int z = 0; z < _patchSize - 1; z++)
                {
                    // Find the indices of the corners
                    int upperLeft = z * _patchSize + x;
                    int upperRight = upperLeft + 1;
                    int lowerLeft = upperLeft + _patchSize;
                    int lowerRight = lowerLeft + 1;

                    // Specify upper triangle
                    _indices[i++] = upperLeft;
                    _indices[i++] = upperRight;
                    _indices[i++] = lowerLeft;

                    // Specify lower triangle
                    _indices[i++] = lowerLeft;
                    _indices[i++] = upperRight;
                    _indices[i++] = lowerRight;
                }
        }

        private void CreatePatches()
        {
            _patches = new TerrainPatch[_numPatchesWidth, _numPatchesHeight];

            for (int y = 0; y < _numPatchesHeight; y++)
            {
                for (int x = 0; x < _numPatchesWidth; x++)
                {
                    _patches[x, y] = CreatePatch(x, y);
                }
            }
        }

        #endregion

        #region Creation Helpers

        private TerrainPatch CreatePatch(int xPatch, int zPatch)
        {
            VertexPositionNormalTexture[] patchVertices;
            int nPatchVertices = _patchSize * _patchSize;

            Vector3 offsetToCenter = -new Vector3(((float)_width / 2.0f) * _cellSize,
                0, ((float)_height / 2.0f) * _cellSize);

            patchVertices = new VertexPositionNormalTexture[nPatchVertices];

            for (int z = 0; z < _patchSize; z++)
            {
                for (int x = 0; x < _patchSize; x++)
                {
                    int xTerrain = (_patchSize - 1) * xPatch + x;
                    int zTerrain = (_patchSize - 1) * zPatch + z;

                    Vector3 position = new Vector3(xTerrain * _cellSize,
                        _heights[xTerrain, zTerrain], zTerrain * _cellSize) + offsetToCenter;

                    Vector2 uv = new Vector2((float)xTerrain / _width, (float)zTerrain / _height);

                    patchVertices[z * _patchSize + x] = new VertexPositionNormalTexture(
                        position, Vector3.Zero, uv);
                }
            }
            CreatePatchNormals(patchVertices, nPatchVertices);

            return new TerrainPatch(patchVertices, nPatchVertices, _graphicsDevice);
        }

        private void CreatePatchNormals(VertexPositionNormalTexture[] patchVertices,
            int nPatchVertices)
        {
            // For each triangle
            for (int i = 0; i < _nIndices; i += 3)
            {
                // Find the position of each corner of the triangle
                Vector3 v1 = patchVertices[_indices[i]].Position;
                Vector3 v2 = patchVertices[_indices[i + 1]].Position;
                Vector3 v3 = patchVertices[_indices[i + 2]].Position;

                // Cross the vectors between the corners to get the normal
                Vector3 normal = Vector3.Cross(v1 - v2, v1 - v3);
                normal.Normalize();

                // Add the influence of the normal to each vertex in the
                // triangle
                patchVertices[_indices[i]].Normal += normal;
                patchVertices[_indices[i + 1]].Normal += normal;
                patchVertices[_indices[i + 2]].Normal += normal;
            }

            // Average the influences of the triangles touching each
            // vertex
            for (int i = 0; i < nPatchVertices; i++)
                patchVertices[i].Normal.Normalize();
        }

        #endregion

        #region Update

        public void Update(GameTime gameTime)
        {
        }

        public void HandleInput(InputManager input)
        {
        }

        #endregion

        #region Draw

        public void Draw(GameTime gameTime)
        {
            CommonHelper.ResetRenderState(_graphicsDevice);

            EffectHelper.SetEffectParameter(_effect, "World", Matrix.Identity);
            EffectHelper.SetEffectParameter(_effect, "View", _camera.View);
            EffectHelper.SetEffectParameter(_effect, "Projection", _camera.Projection);

            EffectHelper.SetEffectParameter(_effect, "BaseTexture", _baseTexture);
            EffectHelper.SetEffectParameter(_effect, "RTexture", _rockTexture);
            EffectHelper.SetEffectParameter(_effect, "BTexture", _waterTexture);
            EffectHelper.SetEffectParameter(_effect, "WeightMap", _weightMap);
            EffectHelper.SetEffectParameter(_effect, "TextureTiling", _textureTiling);

            Point minBound, maxBound;
            FindVisibleTerrainPatches(out minBound, out maxBound);

            RenderedPatches = 0;
            for (int y = minBound.Y; y <= maxBound.Y + 1; y++)
            {
                for (int x = minBound.X; x <= maxBound.X + 1; x++)
                {
                    _graphicsDevice.Indices = _indexBuffer;
                    /*if ((Even(y) && Odd(x)) || (Odd(y) && Even(x)))
                    {
                        EffectHelper.SetEffectParameter(_effect, "OddPatch", true);
                    }
                    else
                    {
                        EffectHelper.SetEffectParameter(_effect, "OddPatch", false);
                    }*/
                    _effect.Techniques[0].Passes[0].Apply();
                    if (x < _numPatchesWidth && y < _numPatchesHeight)
                    {
                        _patches[x, y].Draw(_patchSize * _patchSize, _nIndices, _graphicsDevice, gameTime);
                        RenderedPatches++;
                    }
                }
            }
        }

        #endregion

        #region Helper

        private void FindVisibleTerrainPatches(out Point minBound, out Point maxBound)
        {
            Vector3 minBoundWorld, maxBoundWorld;
            TerrainHelper.FindVisibleTerrainPatches(_camera, _baseHeight,
                out minBoundWorld, out maxBoundWorld);

            Vector3 offsetToCenter = -new Vector3(((float)_width / 2.0f) * _cellSize,
                0, ((float)_height / 2.0f) * _cellSize);

            minBoundWorld -= offsetToCenter;
            maxBoundWorld -= offsetToCenter;

            float patchSize = _cellSize * _patchSize;

            minBound = new Point((int)(minBoundWorld.X / patchSize), (int)(minBoundWorld.Z / patchSize));
            maxBound = new Point((int)(maxBoundWorld.X / patchSize), (int)(maxBoundWorld.Z / patchSize));

            Clamp(ref minBound);
            Clamp(ref maxBound);
        }

        private void Clamp(ref Point index)
        {
            index.X = MyMathHelper.Clamp(index.X, 0, _numPatchesWidth - 1);
            index.Y = MyMathHelper.Clamp(index.Y, 0, _numPatchesHeight - 1);
        }

        private bool Even(int a)
        {
            return a % 2 == 0;
        }

        private bool Odd(int a)
        {
            return !Even(a);
        }

        #endregion

        #region Public Methods

        public TerrainType DetermineTerrainType(float x, float z)
        {
            Vector3 offsetToCenter = -new Vector3(((float)_width / 2.0f) * _cellSize,
                0, ((float)_height / 2.0f) * _cellSize);

            x -= offsetToCenter.X;
            z -= offsetToCenter.Z;

            int xCellMin = (int)Math.Floor(x / _cellSize);
            int zCellMin = (int)Math.Floor(z / _cellSize);
            int xCellMax = xCellMin + 1;
            int zCellMax = zCellMin + 1;

            if (xCellMin < 0 || xCellMax >= _width ||
                zCellMin < 0 || zCellMax >= _height)
                return TerrainType.OutOfTheBox;

            Vector3 p1 = new Vector3(xCellMin * _cellSize,
                        _heights[xCellMin, zCellMax], zCellMax * _cellSize) + offsetToCenter;
            Vector3 p2 = new Vector3(xCellMax * _cellSize,
                        _heights[xCellMax, zCellMin], zCellMin * _cellSize) + offsetToCenter;
            Vector3 p3;

            if ((x / _cellSize - xCellMin) + (z / _cellSize - zCellMin) <= 1)
            {
                p3 = new Vector3(xCellMin * _cellSize,
                        _heights[xCellMin, zCellMin], zCellMin * _cellSize) + offsetToCenter;
            }
            else
            {
                p3 = new Vector3(xCellMax * _cellSize,
                        _heights[xCellMax, zCellMax], zCellMax * _cellSize) + offsetToCenter;
            }

            Plane plane = new Plane(p1, p2, p3);
            Ray ray = new Ray(new Vector3(x, 0, z) + offsetToCenter, Vector3.Up);

            Vector3 intersectedPoint = MyMathHelper.FindIntersection(ray, plane);
            float epsilon = 5.0f;

            if (intersectedPoint.Y > _baseHeight + epsilon)
                return TerrainType.Mountain;
            else if (intersectedPoint.Y > _baseHeight - epsilon)
                return TerrainType.Ground;
            else
                return TerrainType.River;
        }

        public void AddVisibleEntity(float x, float z, IVisibleGameEntity entity)
        {
            Vector3 offsetToCenter = -new Vector3(((float)_width / 2.0f) * _cellSize,
                0, ((float)_height / 2.0f) * _cellSize);

            x -= offsetToCenter.X;
            z -= offsetToCenter.Z;

            float worldPatchSize = (_patchSize - 1) * _cellSize;

            int xPatch = (int)(x / worldPatchSize);
            int zPatch = (int)(z / worldPatchSize);

            if (xPatch < 0 || xPatch >= _numPatchesWidth || 
                zPatch < 0 || zPatch >= _numPatchesHeight)
                return;

            _patches[xPatch, zPatch].AddVisibleEntity(entity);
        }

        public bool CheckCollision(float x, float z, IVisibleGameEntity entity)
        {
            Vector3 offsetToCenter = -new Vector3(((float)_width / 2.0f) * _cellSize,
                0, ((float)_height / 2.0f) * _cellSize);

            x -= offsetToCenter.X;
            z -= offsetToCenter.Z;

            float worldPatchSize = (_patchSize - 1) * _cellSize;

            int xPatch = (int)(x / worldPatchSize);
            int zPatch = (int)(z / worldPatchSize);

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (CheckCollisionPatch(xPatch + i, zPatch + j, entity))
                        return true;
                }
            }
            return false;
        }

        private bool CheckCollisionPatch(int xPatch, int zPatch, IVisibleGameEntity entity)
        {
            if (xPatch < 0 || xPatch >= _numPatchesWidth ||
                zPatch < 0 || zPatch >= _numPatchesHeight)
                return false;

            foreach (IVisibleGameEntity vEntity in _patches[xPatch, zPatch].VisibleEntities)
            {
                CModel cmodel = vEntity as CModel;
                if (cmodel != null)
                {
                    if (cmodel.Name == "OldBridge")
                        continue;
                }
                if (MyMathHelper.IsIntersection(entity.BoundingSphere, vEntity.BoundingSphere, 100))
                    return true;
            }

            return false;
        }

        #endregion
    }
}
