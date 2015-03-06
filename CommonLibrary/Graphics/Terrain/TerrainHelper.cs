using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CommonLibrary.Graphics
{
    public static class TerrainHelper
    {
        public static void FindVisibleTerrainPatches(Camera camera, float baseHeight,
            out Vector3 upperLeftBound, out Vector3 lowerRightBound)
        {
            Vector3[] cameraCorners = camera.Frustum.GetCorners();
            Ray[] rays = CreateRays(cameraCorners);
            Vector3[] intersectedPoints = FindIntersection(rays, new Plane(0f, -1f, 0f, baseHeight));
            FindRectangularBounding(intersectedPoints, out upperLeftBound, out lowerRightBound);
        }

        private static void FindRectangularBounding(Vector3[] points,
            out Vector3 upperLeft, out Vector3 lowerRight)
        {
            upperLeft = points[0];
            lowerRight = points[0];

            for (int i = 0; i < points.Length; i++)
            {
                if (upperLeft.X > points[i].X)
                    upperLeft.X = points[i].X;
                if (upperLeft.Z > points[i].Z)
                    upperLeft.Z = points[i].Z;

                if (lowerRight.X < points[i].X)
                    lowerRight.X = points[i].X;
                if (lowerRight.Z < points[i].Z)
                    lowerRight.Z = points[i].Z;
            }
        }

        private static Vector3[] FindIntersection(Ray[] rays, Plane plane)
        {
            Vector3[] points = new Vector3[rays.Length];

            for (int i = 0; i < rays.Length; i++)
            {
                points[i] = MyMathHelper.FindIntersection(rays[i], plane);
            }

            return points;
        }

        private static Ray[] CreateRays(Vector3[] points)
        {
            Ray[] rays = new Ray[4]
            {
                new Ray(points[0], points[4] - points[0]),
                new Ray(points[1], points[5] - points[1]),
                new Ray(points[2], points[6] - points[2]),
                new Ray(points[3], points[7] - points[3])
            };

            return rays;
        }
    }
}
