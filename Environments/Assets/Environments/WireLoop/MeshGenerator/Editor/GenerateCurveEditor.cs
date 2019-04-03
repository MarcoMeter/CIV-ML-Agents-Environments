using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerateCurve))]
public class GenerateCurveEditor : Editor {

    public override void OnInspectorGUI()
    {
        GenerateCurve myScript = (GenerateCurve)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Calculate Random Points"))
        {
            myScript.CalculateRandomPoints();
        }
        if (myScript.hasRandomPoints())
        {
            if (GUILayout.Button("Draw Random Mesh"))
            {
                myScript.BuildMeshBezier();
            }
        }
    }

    public virtual void OnSceneGUI()
    {
        GenerateCurve gc = (GenerateCurve)target;

        gc.endPosition = gc.transform.InverseTransformPoint(Handles.PositionHandle(gc.transform.TransformPoint(gc.endPosition), Quaternion.identity));
        gc.endPosition = new Vector3(gc.endPosition.x, 0f, gc.endPosition.z);
    }
}
