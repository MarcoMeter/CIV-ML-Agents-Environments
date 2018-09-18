using System;
using UnityEditor;
using UnityEngine;

namespace SaltyUtils.Editor
{
    public class Utility
    {
        public static MonoBehaviour GetComponentSelectionNull<MonoBehaviour>()
        {
            GameObject selection = Selection.activeGameObject;

            if (selection == null)
                return default(MonoBehaviour);

            try
            {
                return Selection.activeGameObject.GetComponent<MonoBehaviour>();
            }
            catch (Exception)
            {
                Debug.LogError("Component not found.");
                return default(MonoBehaviour);
            }
        }
    }
}