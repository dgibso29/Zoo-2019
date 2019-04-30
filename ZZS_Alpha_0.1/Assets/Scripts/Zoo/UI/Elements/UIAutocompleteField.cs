using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zoo.Assets;
using Zoo.Systems;

namespace Zoo.UI.Elements
{
    class UIAutocompleteField : MonoBehaviour, IDynamicUIElement
    {

        public InputField inputField;
        public RectTransform resultsParent;
        public RectTransform prefab;

        private void Awake()
        {
            inputField.onValueChanged.AddListener(OnInputValueChanged);
        }

        private void OnInputValueChanged(string newText)
        {
            ClearResults();
            FillResults(GetResults(newText));
        }

        private void ClearResults()
        {
            if (inputField.text.Length == 0)
            {
                return;
            }
            // Reverse loop since you destroy children
            for (int childIndex = resultsParent.childCount - 1; childIndex >= 0; --childIndex)
            {
                Transform child = resultsParent.GetChild(childIndex);
                child.SetParent(null);
                Destroy(child.gameObject);
            }
        }
        
        private void FillResults(List<string> results)
        {
            if (results.Count == 0)
            {
                return;
            }
            foreach (var t in results)
            {
                if (string.IsNullOrWhiteSpace((t)))
                {
                    continue;
                }
                RectTransform child = Instantiate(prefab, resultsParent.transform);
                child.GetComponentInChildren<Text>().text = t;
                //child.SetParent(resultsParent);
            }
        }


        private List<string> GetResults(string input)
        {
            if (input == null)
            {
                return new List<string>();
            }
            var newestTag = input.Split(',').Last().Trim();
            if(string.IsNullOrWhiteSpace(newestTag))
                return new List<string>();

            return AssetManager.AllMetadataTags.FindAll((str) => str.IndexOf(newestTag) >= 0);
        }

        public void SetComponentReferences()
        {
            throw new NotImplementedException();
        }

        public void UpdateElementData()
        {
            throw new NotImplementedException();
        }
    }
}