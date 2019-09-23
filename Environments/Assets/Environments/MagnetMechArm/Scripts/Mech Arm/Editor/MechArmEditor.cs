using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

[CustomEditor(typeof(ArmController))]
public class MechArmEditor : Editor
{
    bool bulkAdd = false;
    private void OnEnable()
    {

    }

    public void SetLock(bool val)
    {
        var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow");
        var window = EditorWindow.GetWindow(type);
        PropertyInfo info = type.GetProperty("isLocked", BindingFlags.Public | BindingFlags.Instance);
        info.SetValue(window, val);
    }

    public static bool InspectorIsLocked()
    {
        var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow");
        var window = EditorWindow.GetWindow(type);
        PropertyInfo info = type.GetProperty("isLocked", BindingFlags.Public | BindingFlags.Instance);
        return (bool)info.GetValue(window, null);
    }
    public override void OnInspectorGUI()
    {
        ArmController t = (target as ArmController);
        DrawDefaultInspector();

        if(bulkAdd)
        {
            if (InspectorIsLocked())
            {
                EditorGUILayout.BeginVertical();
                foreach (GameObject selected in Selection.gameObjects)
                {
                    GUILayout.Label("+ " + selected.name);
                }

                if (Selection.gameObjects.Length > 0)
                {
                    if (GUILayout.Button("Add Selected Objects (" + Selection.gameObjects.Length + ")"))
                    {
                        Undo.RecordObject(t, "Bulk Add");
                        MechJoint[] joints = t.joints;
                        t.joints = new MechJoint[joints.Length + Selection.gameObjects.Length];

                        for (int i = 0; i < joints.Length; i++)
                        {
                            t.joints[i] = joints[i];
                        }

                        for (int i = 0; i < Selection.gameObjects.Length; i++)
                        {
                            MechJoint joint = new MechJoint();
                            joint.joint = Selection.gameObjects[i].transform;
                            t.joints[joints.Length + i] = joint;
                        }

                        bulkAdd = false;
                        SetLock(false);
                        Selection.activeGameObject = t.gameObject;
                    }
                }

                EditorGUILayout.EndVertical();
            }

            if(GUILayout.Button("Stop Bulk Add"))
            {
                bulkAdd = false;
                SetLock(false);
            }
        }
        else if(GUILayout.Button("Start Bulk Add"))
        {
            bulkAdd = true;
            SetLock(true);
        }
        

        if(t.joints.Length > 0)
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Positionen Aktualisieren"))
            {
                Object[] objects = new Object[t.joints.Length];
                int i = 0;

                foreach (MechJoint joint in t.joints)
                {
                    objects[i++] = joint.joint;
                }

                Undo.RecordObjects(objects, "Refresh Mech Positions");

                t.ResetPositions();
            }

            if (GUILayout.Button("Positionen speichern"))
            {
                Undo.RecordObject(t, "Save Initial Positions");
                foreach (MechJoint joint in t.joints)
                {
                    joint.initialRotation = joint.joint.localEulerAngles;
                }
            }

            GUILayout.EndHorizontal();
        }
    }
    public void OnSceneGUI()
    {
        ArmController t = (target as ArmController);

        foreach(MechJoint joint in t.joints)
        {
            EditorGUI.BeginChangeCheck();

            joint.joint.rotation = Handles.Disc(joint.joint.rotation, joint.joint.position, joint.joint.TransformDirection(joint.axis), 1f, false, 45f / 4f);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(joint.joint, "Disc Rotate");
            }
        }
    }
}
