using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Zoo.UI.Localization
{
    public class LocalizedText : MonoBehaviour
    {
        public string key;

        void Start()
        {
            Text text = GetComponent<Text>();
            text.text = LocalizationManager.Instance.GetLocalizedValue(key);
        }
    }
}