using SaltyUtils.SaltySpline.Editor;
using SaltyUtils.SaltySpline.Enums;
using System;
using UnityEngine;

namespace SaltyUtils.SaltySpline.Classes
{
    [ExecuteInEditMode]
    public class SplinePoint : MonoBehaviour
    {
        public Spline parentSpline;
        public BezierType bezierType;
        public Vector3 controlPoint1, controlPoint2;
        public Vector3[] bezierPoints;

        private void OnDestroy()
        {
            SplineBuilder.DeletePoint(this);
        }
    }
}

