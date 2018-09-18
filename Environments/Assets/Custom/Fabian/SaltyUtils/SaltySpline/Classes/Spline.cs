using SaltyUtils.SaltySpline.Bezier;
using SaltyUtils.SaltySpline.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaltyUtils.SaltySpline.Classes
{
    [ExecuteInEditMode]
    public class Spline : MonoBehaviour
    {
        public List<SplinePoint> splinePoints = new List<SplinePoint>();
        public int resolution;

        public SplinePoint FirstSplinePoint
        {
            get
            {
                return splinePoints[0];
            }
        }
        public SplinePoint LastSplinePoint
        {
            get
            {
                return splinePoints[splinePoints.Count - 1];
            }
        }
    }
}