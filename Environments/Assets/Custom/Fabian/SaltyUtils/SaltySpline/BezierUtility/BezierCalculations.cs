using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaltyUtils.SaltySpline.Bezier
{
    public class BezierCalculations
    {
        public static Vector3 CalcLinearBezierPoint(float t, Vector3 p1, Vector3 p2)
        {
            return p1 + t * (p2 - p1);
        }

        public static Vector3 CalcQuadraticBezierPoint(float t, Vector3 p1, Vector3 p2, Vector3 c1)
        {
            return Mathf.Pow((t - 1), 2) * p1 + 2 * (1 - t) * t * c1 + Mathf.Pow(t, 2) * p2;
        }

        public static Vector3 CalcCubicBezierPoint(float t, Vector3 p1, Vector3 p2, Vector3 c1, Vector3 c2)
        {
            return
                Mathf.Pow(1 - t, 3) * p1 +
                3 * Mathf.Pow(1 - t, 2) * t * c1 +
                3 * (1 - t) * Mathf.Pow(t, 2) * c2 +
                Mathf.Pow(t, 3) * p2;
        }

        public static Vector3[] BulkQuadraticBezierPoint(Vector3 startPoint, Vector3 endPoint, Vector3 controlPoint, float resolution)
        {
            List<Vector3> result = new List<Vector3>();

            result.Add(startPoint);

            for (float i = 1; i < resolution; i++)
            {
                float step = i / resolution;
                Vector3 point = CalcQuadraticBezierPoint(step, startPoint, endPoint, controlPoint);
                result.Add(point);
            }

            result.Add(endPoint);

            return result.ToArray();
        }
    }
}
