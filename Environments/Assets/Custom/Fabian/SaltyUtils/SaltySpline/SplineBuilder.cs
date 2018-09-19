using UnityEngine;
using SaltyUtils.SaltySpline.Classes;
using SaltyUtils.SaltySpline.Enums;
using System;

namespace SaltyUtils.SaltySpline.Editor
{
    public class SplineBuilder : MonoBehaviour
    {
        #region General

        public static Spline CreateNewSpline(BezierType bezierType)
        {
            // Create GameObject and add Component
            GameObject newGameObject = new GameObject("Spline");
            Spline newSpline = newGameObject.AddComponent<Spline>();

            // Generate two new points
            SplinePoint startPoint = AddPoint(newSpline, bezierType);
            SplinePoint endPoint = AddPoint(newSpline, bezierType);

            return newSpline;
        }

        #endregion

        #region Point Logic

        public static SplinePoint AddPoint(Spline spline, BezierType bezierType)
        {
            // Create new point and attach SplinePointComponent
            GameObject newGameObject = new GameObject("SplinePoint" + spline.splinePoints.Count);
            SplinePoint newSplinePoint = newGameObject.AddComponent<SplinePoint>();

            newGameObject.transform.parent = spline.transform;
            newSplinePoint.parentSpline = spline;

            newSplinePoint.bezierType = bezierType;

            // TODO Add ControlPoints

            spline.splinePoints.Add(newSplinePoint);
            AutoPlaceNewPoint(newSplinePoint);

            return newSplinePoint;
        }

        public static void DeletePoint(SplinePoint splinePoint)
        {
            if (Application.isEditor)
                DestroyImmediate(splinePoint.gameObject);
            else
                Destroy(splinePoint.gameObject);
        }

        #endregion

        #region Utility

        private static void AutoPlaceNewPoint(SplinePoint splinePoint)
        {
            Spline parentSpline = splinePoint.parentSpline;

            splinePoint.transform.position = splinePoint.parentSpline.LastSplinePoint.transform.position + Vector3.one;
        }

        #endregion
    }
}