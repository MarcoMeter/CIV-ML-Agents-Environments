using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(GenerateObject))]
public class ObjectBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GenerateObject myScript = (GenerateObject)target;
        if (GUILayout.Button("Build Object"))
        {
            myScript.GenerateFullObject();
        }

        if(GUILayout.Button("Randomize"))
        {
            Undo.RecordObject(target, "Random Values");
            myScript.Randomize();
        }
    }
}