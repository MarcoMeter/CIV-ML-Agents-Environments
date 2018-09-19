using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SaltyUtils.Misc.Editor
{
    public class TransformNormalizerWindow : EditorWindow
    {
        private bool includeChildren;
        private int roundFactor;

        [MenuItem("Tools/SaltyUtils/Transform Normalizer")]
        static void Init()
        {
            TransformNormalizerWindow window = GetWindow<TransformNormalizerWindow>("Transform Normalizer");
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("Selected Transform", Selection.activeTransform == null ? "None" : Selection.activeTransform.name);
            EditorGUI.EndDisabledGroup();

            includeChildren = EditorGUILayout.Toggle("Include children", includeChildren);
            roundFactor = EditorGUILayout.IntSlider("Rounding factor", (roundFactor / 10), 1, 10) * 10;

            if (GUILayout.Button("Round Transform(s)"))
            {
                if (includeChildren)
                {
                    List<Transform> transforms = new List<Transform>();

                    foreach (var item in Selection.transforms)
                    {
                        transforms.Add(item);
                        Transform[] childs = item.GetComponentsInChildren<Transform>();

                        foreach (var item2 in childs)
                        {
                            transforms.Add(item2);
                        }
                    }

                    foreach (var item in transforms)
                    {
                        item.localPosition = new Vector3(
                            Mathf.Round(item.localPosition.x * roundFactor) / roundFactor,
                            Mathf.Round(item.localPosition.y * roundFactor) / roundFactor,
                            Mathf.Round(item.localPosition.z * roundFactor) / roundFactor);

                        item.localEulerAngles = new Vector3(
                            Mathf.Round(item.localEulerAngles.x * roundFactor) / roundFactor,
                            Mathf.Round(item.localEulerAngles.y * roundFactor) / roundFactor,
                            Mathf.Round(item.localEulerAngles.z * roundFactor) / roundFactor);

                        item.localScale = new Vector3(
                            Mathf.Round(item.localScale.x * roundFactor) / roundFactor,
                            Mathf.Round(item.localScale.y * roundFactor) / roundFactor,
                            Mathf.Round(item.localScale.z * roundFactor) / roundFactor);
                    }

                    transforms.Clear();
                }
                else
                {
                    foreach (var item in Selection.transforms)
                    {
                        item.localPosition = new Vector3(
                            Mathf.Round(item.localPosition.x * roundFactor) / roundFactor,
                            Mathf.Round(item.localPosition.y * roundFactor) / roundFactor,
                            Mathf.Round(item.localPosition.z * roundFactor) / roundFactor);

                        item.localEulerAngles = new Vector3(
                            Mathf.Round(item.localEulerAngles.x * roundFactor) / roundFactor,
                            Mathf.Round(item.localEulerAngles.y * roundFactor) / roundFactor,
                            Mathf.Round(item.localEulerAngles.z * roundFactor) / roundFactor);

                        item.localScale = new Vector3(
                            Mathf.Round(item.localScale.x * roundFactor) / roundFactor,
                            Mathf.Round(item.localScale.y * roundFactor) / roundFactor,
                            Mathf.Round(item.localScale.z * roundFactor) / roundFactor);
                    }
                }
            }
        }

        private void OnSelectionChange()
        {
            Repaint();
        }
    }
}
