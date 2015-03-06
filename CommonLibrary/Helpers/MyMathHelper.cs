using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CommonLibrary.Graphics;

namespace CommonLibrary
{
    public static class MyMathHelper
    {
        private static Random _random = new Random();

        /// <summary>
        /// return float value within 0.0 and 1.0
        /// </summary>
        /// <returns></returns>
        public static float GetRandomFloat()
        {
            return (float)_random.NextDouble();
        }

        public static bool IsIntersection(Ray ray, BoundingSphere sphere)
        {
            Vector3 cross = Vector3.Cross(ray.Direction, sphere.Center - ray.Position);
            float d = cross.Length() / ray.Direction.Length();
            if (d < sphere.Radius)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool RayIsIntersection(Ray ray, BoundingSphere sphere)
        {
            Vector3 cross = Vector3.Cross(ray.Direction, sphere.Center - ray.Position);
            float d = cross.Length() / ray.Direction.Length();
            float dot = Vector3.Dot(ray.Direction, sphere.Center - ray.Position);
            if ((d < sphere.Radius) && (dot >= 0))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsIntersection(BoundingSphere sphere1, BoundingSphere sphere2, float threshold)
        {
            Vector3 o1 = sphere1.Center;
            Vector3 o2 = sphere2.Center;
            float r1 = sphere1.Radius;
            float r2 = sphere2.Radius;
            float d = (o1 - o2).Length();

            if (r1 + r2 < d + threshold)
                return false;
            else
                return true;
        }

        public static Vector3 FindIntersection(Ray ray, Plane plane)
        {
            float denominator = Vector3.Dot(plane.Normal, ray.Direction);
            float numerator = Vector3.Dot(plane.Normal, ray.Position) + plane.D;
            float t = -(numerator / denominator);

            return ray.Position + ray.Direction * t;
        }

        public static Vector3 ScreenCoord2WorldCoord(Vector2 screenCoord, float height,
            Camera camera, GraphicsDevice device)
        {
            Ray ray = CreateRayCasting(screenCoord, camera, device);
            Plane plane = new Plane(0f, -1f, 0f, height);

            return FindIntersection(ray, plane);
        }

        public static Ray CreateRayCasting(Vector2 screenCoord,
            Camera camera, GraphicsDevice device)
        {
            Vector3 minPointSource = device.Viewport.Unproject(
                new Vector3(screenCoord.X, screenCoord.Y, 0),
                camera.Projection, camera.View,
                Matrix.Identity);

            Vector3 maxPointSource = device.Viewport.Unproject(
                new Vector3(screenCoord.X, screenCoord.Y, 10f),
                camera.Projection, camera.View,
                Matrix.Identity);

            Vector3 direction = maxPointSource - minPointSource;
            direction.Normalize();

            return new Ray(minPointSource, direction);
        }

        public static int Clamp(int val, int min, int max)
        {
            if (val < min)
                return min;
            else if (val > max)
                return max;
            else
                return val;
        }
    }
}
