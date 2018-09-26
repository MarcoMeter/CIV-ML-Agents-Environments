using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerateCurve))]
public class GenerateCurveEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GenerateCurve myScript = (GenerateCurve)target;
        if (GUILayout.Button("Build Mesh"))
        {
            myScript.BuildMesh();
        }
        if(GUILayout.Button("Calculate Random Points"))
        {
            myScript.CalculateRandomPoints();
        }
        if(GUILayout.Button("Draw Random Mesh"))
        {
            myScript.BuildMeshBezier();
        }
    }
}
