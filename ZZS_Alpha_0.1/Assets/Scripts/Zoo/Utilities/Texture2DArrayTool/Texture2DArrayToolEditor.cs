using System;
using UnityEngine;
using UnityEditor;

namespace Zoo.Utilities.Texture2DArrayTool
{
#if UNITY_EDITOR
    [CustomEditor(typeof(Texture2DArrayTool))]
    public class Texture2DArrayToolEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Texture2DArrayTool arrayTool = (Texture2DArrayTool) target;
            if (GUILayout.Button("Create Texture2DArray"))
            {
                try
                {
                    arrayTool.CreateTexture2DArray();
                    Debug.Log($"Texture 2D Array {arrayTool.AssetName} created successfully.");
                }
                catch(Exception e)
                {
                    Debug.Log($"Texture 2D Array {arrayTool.AssetName} creation failed.");
                    Debug.Log(e.Message);
                }
            }
        }

    }
#endif
}
