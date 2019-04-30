using UnityEditor;
using UnityEngine;

namespace Zoo.Utilities.Texture2DArrayTool
{
#if UNITY_EDITOR
    // Used to create Texture2DArray assets in the editor.
    public class Texture2DArrayTool : MonoBehaviour
    {
        [Tooltip("Textures to add to the array, arranged in the intended order.")]
        public Texture2D[] Textures;

        [Tooltip("Folder in which to save created arrays. Will be created if it does not exist.")]
        public string AssetFolder = "Texture2DArrays";

        [Tooltip("Name with which the asset will be saved.")]
        public string AssetName;

        public TextureFormat Format = TextureFormat.RGBA32;

        public bool MipMap = true;

        public bool Linear = false;

        public FilterMode FilterMode = FilterMode.Bilinear;

        public TextureWrapMode TextureWrapMode = TextureWrapMode.Repeat;

        /// <summary>
        /// Creates & saves a Texture2DArray using the provided parameters.
        /// </summary>
        internal void CreateTexture2DArray()
        {
            // Declare the array
            var array = new Texture2DArray(Textures[0].width, Textures[0].height, Textures.Length,
                Format, MipMap, Linear);

                // Apply additional settings
            array.filterMode = FilterMode;
            array.wrapMode = TextureWrapMode;

            // Copy pixels of Textures to the array
            for (var i = 0; i < Textures.Length; i++)
            {
                array.SetPixels(Textures[i].GetPixels(0), i, 0);
            }

            // Apply changes to array
            array.Apply();

            // Make sure Texture2DArrays dir exists
            if (!AssetDatabase.IsValidFolder($"Assets/{AssetFolder}"))
            {
                // Create it if not
                AssetDatabase.CreateFolder("Assets", AssetFolder);
            }

            // Save array as to Assets/Texture2DArrays using the specific asset name.
            AssetDatabase.CreateAsset(array, $"Assets/{AssetFolder}/" + AssetName + ".asset");

        }
    }
#endif
}
