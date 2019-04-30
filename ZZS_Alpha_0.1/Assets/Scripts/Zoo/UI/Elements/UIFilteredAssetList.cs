using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zoo.Assets;
using Zoo.UI;
using Zoo.UI.Elements;

public class UIFilteredAssetList : MonoBehaviour, IDynamicUIElement
{

    public GameObject ListPanel;

    public InputField AssetFilterInput;

    public GameObject AssetContainerPrefab;

    private List<string> _currentFilters = new List<string>();

    private List<string> _currentAssetStringIDs;

    private List<GameObject> _activeContainers = new List<GameObject>();

    public void Awake()
    {
        AssetFilterInput.onValueChanged.AddListener(OnInputValueChanged);
        OnInputValueChanged(null);
    }

    public void SetComponentReferences()
    {
        
    }

    public void UpdateElementData()
    {
        UpdateAssetContainers();
    }

    public void OnInputValueChanged(string newText)
    {
        if (string.IsNullOrWhiteSpace(newText))
        {
            // If empty, display everything
            var allTags = AssetManager.AllMetadataTags;
            _currentAssetStringIDs = AssetManager.GetAssetsWithTags(allTags, false).Select(a => a.AssetStringID).ToList();
            _currentAssetStringIDs.Sort();
            UpdateElementData();
            return;
        }
        // Get each tag.
        var separatedInput = newText.Split(',').Select(s => s.Trim().ToLower()).
            Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
        var last = separatedInput.Count > 1 ? separatedInput.Last() : separatedInput[0];
        //var autofillResults = AssetManager.AllMetadataTags.FindAll((str) => str.IndexOf(last) >= 0);
        if (!AssetManager.AllMetadataTags.Contains(last)) separatedInput.Remove(last);
        //separatedInput.AddRange(autofillResults);
        
        var results = new List<string>();
        if (separatedInput.Count > 0) 
        results = AssetManager.GetAssetsWithTags(separatedInput).Select(a => a.AssetStringID).ToList();

        if (results.Count < 1)
        {
            return;
        }
        
        // Break if updating would result in empty menu
        _currentAssetStringIDs = results;
        _currentAssetStringIDs.Sort();
        UpdateElementData();
        
    }

    private void UpdateAssetContainers()
    {
        foreach (var activeContainer in _activeContainers)
        {
            Destroy(activeContainer);
        }

        foreach (var id in _currentAssetStringIDs)
        {
           var containerObject = Instantiate(AssetContainerPrefab, ListPanel.transform);
            var container = containerObject.GetComponent<UIGameAssetContainer>();
            container.Initialize(id);
            _activeContainers.Add(containerObject);
        }
    }
    
}
