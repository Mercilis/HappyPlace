using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TestingScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var myLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "realisticforest"));
        if (myLoadedAssetBundle == null)
        {
            Debug.Log("Failed to load AssetBundle!");
            return;
        }
        GameObject prefab = myLoadedAssetBundle.LoadAsset<GameObject>("SM_Tree_Pine_01");
        prefab.AddComponent<BoxCollider>();
        BoxCollider boxCollider= prefab.GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        MeshRenderer renderer = prefab.GetComponentInChildren<MeshRenderer>();
        boxCollider.center = renderer.bounds.center;
        boxCollider.size = renderer.bounds.size;
        prefab.AddComponent<PlaceableObject>();
        Instantiate(prefab);
    }
}
