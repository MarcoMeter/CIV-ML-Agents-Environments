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
            GameObject newGameObject = new GameObject("Spline");
            Spline newSpline = newGameObject.AddComponent<Spline>();

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

            return newSplinePoint;
        }

        public static void DeletePoint(SplinePoint splinePoint)
        {
            Destroy(splinePoint.gameObject);
        }

        #endregion

        #region Utility

        private void AutoPlaceNewPoint()
        {

        }

        #endregion
    }
}