using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Zoo.Assets;

namespace Zoo.Utilities.GameAssetIconTool
{
#if UNITY_EDITOR

    public class GameAssetIconTool : MonoBehaviour
    {
        public GameObject CameraTarget;

        public Camera IconCamera;

        [Tooltip("Tool will generate icons for all assets in this folder & its subfolders.")]
        public string FolderToCheck = "Asset Bundles";

        public string IconSaveFolder = "GeneratedAssetIcons";

        [Tooltip("When checked, tool will regenerate existing icons.")]
        public bool RegenerateIcons = false;

        public float CameraAspect = 1.0f;

        public int IconWidth = 1024;

        public int IconHeight = 1024;

        internal void GenerateGameAssetIcons()
        {
            // Make sure folder is valid
            if (!AssetDatabase.IsValidFolder($"Assets/{FolderToCheck}"))
            {
                Debug.Log("Icon generation failed -- Invalid folder!");
            }

            // Check for icon save folders
            if (!AssetDatabase.IsValidFolder($"Assets/Art"))
            {
                AssetDatabase.CreateFolder($"Assets", "Art");
            }
            if (!AssetDatabase.IsValidFolder($"Assets/Art/{IconSaveFolder}"))
            {
                AssetDatabase.CreateFolder($"Assets/Art", IconSaveFolder);
            }

            IconCamera.GetComponent<Camera>().aspect = CameraAspect;
            var renderTex = new RenderTexture(IconWidth, IconHeight, 32);
            IconCamera.GetComponent<Camera>().targetTexture = renderTex;
            RenderTexture.active = renderTex;
            var assetsToModify = AssetDatabase.FindAssets("t:GameAsset", new [] {$"Assets/{FolderToCheck}"});
            foreach (var guid in assetsToModify)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<GameAsset>(assetPath);

                // Skip this asset if icon generation is disabled.
                if (!asset.AllowIconGeneration) continue;

                EditorUtility.SetDirty(asset);
                if (asset.AssetUITexture == null || RegenerateIcons)
                {
                    var newIcon = RenderIcon(asset.AssetPrefab);
                    var newFolder = asset.GetType().Name;
                    if (!AssetDatabase.IsValidFolder($"Assets/Art/{IconSaveFolder}/{newFolder}"))
                    {
                        AssetDatabase.CreateFolder($"Assets/Art/{IconSaveFolder}", newFolder);
                    }
                    var newPath = $"Assets/Art/{IconSaveFolder}/{newFolder}/{asset.AssetStringID}.asset";
                    AssetDatabase.CreateAsset(newIcon, newPath);
                    var newTex = AssetDatabase.LoadAssetAtPath<Texture2D>(newPath);
                    asset.AssetUITexture = newTex;
                    
                    // Overwrite the asset with the updated data
                    // Hopefully this will actually save our changes
                    AssetDatabase.SaveAssets();
                }


            }



            RenderTexture.active = null;
            IconCamera.GetComponent<Camera>().targetTexture = null;
            
        }

        private Texture2D RenderIcon(GameObject gameObjectToRender)
        {

            var target = Instantiate(gameObjectToRender, CameraTarget.transform);
            IconCamera.GetComponent<Camera>().Render();
            var texture = new Texture2D(IconWidth, IconHeight, TextureFormat.ARGB32, false);
            texture.ReadPixels(new Rect(0,0, IconWidth, IconHeight),0, 0);
            texture.Apply();
            DestroyImmediate(target);
            return texture;
        }
    }

#endif
}
