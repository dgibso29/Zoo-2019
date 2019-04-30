using System;
using UnityEditor;
using UnityEngine;

namespace Zoo.Utilities.GameAssetIconTool
{

#if UNITY_EDITOR
        [CustomEditor(typeof(GameAssetIconTool))]
        public class GameAssetIconToolEditor : Editor
        {

            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();

                var iconTool = (GameAssetIconTool)target;
                if (GUILayout.Button("Generate Icons"))
                {
                    try
                    {
                        iconTool.GenerateGameAssetIcons();
                        Debug.Log($"Icons generated!");
                    }
                    catch (Exception e)
                    {
                        Debug.Log($"Icon generation failed.");
                        Debug.Log(e.Message);
                    }
                }
            }


    }
#endif

}
