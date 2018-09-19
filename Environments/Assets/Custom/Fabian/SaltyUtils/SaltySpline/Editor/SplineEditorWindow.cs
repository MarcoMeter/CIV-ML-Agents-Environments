using SaltyUtils.Editor;
using SaltyUtils.SaltySpline.Bezier;
using SaltyUtils.SaltySpline.Classes;
using SaltyUtils.SaltySpline.Enums;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SaltyUtils.SaltySpline.Editor
{
    public class SplineBuilderWindow : EditorWindow
    {
        public static SplineBuilderWindow window;

        private Spline selectedSpline;
        private SplinePoint selectedSplinePoint;
        private BezierType bezierType, bezierType2;

        [MenuItem("Tools/SaltyTools/SaltySpline")]
        static void Init()
        {
            // Open editor window on menu click
            SplineBuilderWindow window = GetWindow<SplineBuilderWindow>("SaltySpline");
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("SaltySpline v0.1.0", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            #region Spline

            EditorGUILayout.Space();
            GUILayout.Label("Splines");

            bezierType = (BezierType)EditorGUILayout.EnumPopup(bezierType);

            if (GUILayout.Button("Create new Spline"))
            {
                SplineBuilder.CreateNewSpline(bezierType);
            }

            #endregion

            #region SplinePoints

            EditorGUILayout.Space();
            GUILayout.Label("Spline Points");

            bezierType2 = (BezierType)EditorGUILayout.EnumPopup(bezierType2);

            if (GUILayout.Button("Add Point"))
            {
                // Check if spline selected
                if (Selection.activeGameObject == null || selectedSpline == null)
                {
                    EditorApplication.Beep();
                    EditorUtility.DisplayDialog("No Spline selected", "Please select a Spline GameObject first.", "OK");
                    return;
                }

                SplineBuilder.AddPoint(selectedSpline, bezierType);
            }

            if (GUILayout.Button("Delete Point"))
            {
                SplineBuilder.DeletePoint(selectedSplinePoint);
            }

            #endregion

            #region Display Info

            if (selectedSpline != null)
            {
                EditorGUILayout.Space();
                GUILayout.Label("Current Spline Data");

                EditorGUILayout.Space();
                GUILayout.Label("Number of SplinePoints: " + selectedSpline.splinePoints.Count);

                for (int i = 0; i < selectedSpline.splinePoints.Count; i++)
                {
                    GUILayout.Label(i + ": " + selectedSpline.splinePoints[i].transform.position);
                }
            }
            else if (selectedSplinePoint != null)
            {
                EditorGUILayout.Space();
                GUILayout.Label("Current Spline Data");

                List<SplinePoint> splinePoints = selectedSplinePoint.parentSpline.splinePoints;

                EditorGUILayout.Space();
                GUILayout.Label("Number of SplinePoints: " + splinePoints.Count);

                for (int i = 0; i < splinePoints.Count; i++)
                {
                    GUILayout.Label(i + ": " + splinePoints[i].transform.position);
                }
            }

            #endregion
        }

        private void OnSelectionChange()
        {
            selectedSpline = Utility.GetComponentSelectionNull<Spline>();
            selectedSplinePoint = Utility.GetComponentSelectionNull<SplinePoint>();

            // Auto select spline of selected SplinePoint
            if (selectedSpline == null && selectedSplinePoint != null) selectedSpline = selectedSplinePoint.parentSpline;

            Repaint();
        }

        void OnFocus()
        {
            // Remove delegate listener if it has previously
            // been assigned.
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
            // Add (or re-add) the delegate.
            SceneView.onSceneGUIDelegate += OnSceneGUI;
        }

        void OnDestroy()
        {
            // When the window is destroyed, remove the delegate
            // so that it will no longer do any drawing.
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
        }

        void OnSceneGUI(SceneView sceneView)
        {
            // Do your drawing here using Handles.
            if (selectedSpline != null)
            {
                // Draw points
                foreach (var item in selectedSpline.splinePoints)
                {
                    item.transform.position = Handles.PositionHandle(item.transform.position, Quaternion.identity);
                    Handles.color = Color.green;
                    Handles.SphereHandleCap(0, item.transform.position, Quaternion.identity, 0.1f, EventType.Repaint);

                    GUIStyle labelStyle = new GUIStyle();
                    labelStyle.normal.textColor = Color.red;

                    Handles.Label(item.transform.position, item.name, labelStyle);

                    // Draw ControlPoints Handles
                    if (item == selectedSpline.splinePoints[selectedSpline.splinePoints.Count - 1])
                        break;

                    switch (item.bezierType)
                    {
                        case BezierType.Linear:
                            // No controlPoints needed
                            break;

                        case BezierType.Quadratic:
                            item.controlPoint1 = Handles.PositionHandle(item.controlPoint1, Quaternion.identity);
                            Handles.color = Color.red;
                            Handles.SphereHandleCap(0, item.controlPoint1, Quaternion.identity, 0.1f, EventType.Repaint);
                            break;

                        case BezierType.Cubic:
                            //TODO Cubic bezier control points
                            break;
                    }
                }

                // Only draw when spline contains points
                if (selectedSpline.splinePoints.Count <= 1)
                    return;

                for (int i = 0; i < selectedSpline.splinePoints.Count - 1; i++)
                {
                    switch (selectedSpline.splinePoints[i].bezierType)
                    {
                        case BezierType.Linear:
                            Handles.color = Color.green;
                            Handles.DrawLine(
                                selectedSpline.splinePoints[i].transform.position,
                                selectedSpline.splinePoints[i + 1].transform.position);
                            break;

                        case BezierType.Quadratic:
                            Vector3[] bezierPoints = BezierCalculations.BulkQuadraticBezierPoint(
                                selectedSpline.splinePoints[i].transform.position,
                                selectedSpline.splinePoints[i + 1].transform.position,
                                selectedSpline.splinePoints[i].controlPoint1,
                                selectedSpline.resolution);

                            Handles.color = Color.green;
                            Handles.DrawPolyLine(bezierPoints);

                            Handles.color = Color.yellow;
                            Handles.DrawDottedLine(
                                selectedSpline.splinePoints[i].transform.position,
                                selectedSpline.splinePoints[i].controlPoint1, 5f);

                            Handles.DrawDottedLine(
                                selectedSpline.splinePoints[i].controlPoint1,
                                selectedSpline.splinePoints[i + 1].transform.position, 5f);
                            break;
                        case BezierType.Cubic:
                            break;

                        default:
                            break;
                    }
                }
            }

            Handles.BeginGUI();
            // Do your drawing here using GUI.
            Handles.EndGUI();
        }
    }
}