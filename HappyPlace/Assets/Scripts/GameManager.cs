using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour {

    #region AssetBundleStuff
    private AssetBundle m_realistForestAssetBundle;
    public string[] AllRealisticForestNames { get; private set; }
    #endregion

    #region PlayerControllers
    [SerializeField]
    GameObject m_leftController = null;
    [SerializeField]
    GameObject m_rightController = null;
    #endregion

    // Use this for initialization
    void Start()
    {
        var myLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "realisticforest"));
        if (myLoadedAssetBundle == null)
        {
            Debug.Log("Failed to load AssetBundle!");
            return;
        }
        m_realistForestAssetBundle = myLoadedAssetBundle;
        AllRealisticForestNames = m_realistForestAssetBundle.GetAllAssetNames();
        CleanUpRealisticForestNames();
    }

    private void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            SpawnTreeTest();
        }
    }

    private void SpawnTreeTest()
    {
        GameObject prefab = m_realistForestAssetBundle.LoadAsset<GameObject>("SM_Tree_Pine_01");
        prefab.AddComponent<BoxCollider>();
        BoxCollider boxCollider = prefab.GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        MeshRenderer renderer = prefab.GetComponentInChildren<MeshRenderer>();
        boxCollider.center = renderer.bounds.center;
        boxCollider.size = renderer.bounds.size;
        prefab.AddComponent<Rigidbody>();
        prefab.GetComponent<Rigidbody>().isKinematic = true;
        prefab.AddComponent<PlaceableObject>();
        Instantiate(prefab);
    }

    private void SpawnItemByName(string name)
    {
        GameObject prefab = m_realistForestAssetBundle.LoadAsset<GameObject>(name);
        prefab.AddComponent<BoxCollider>();
        BoxCollider boxCollider = prefab.GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        MeshRenderer renderer = prefab.GetComponentInChildren<MeshRenderer>();
        boxCollider.center = renderer.bounds.center;
        boxCollider.size = renderer.bounds.size;
        prefab.AddComponent<Rigidbody>();
        prefab.GetComponent<Rigidbody>().isKinematic = true;
        prefab.AddComponent<PlaceableObject>();
        Instantiate(prefab);
    }

    private void CleanUpRealisticForestNames()
    {
        for (int i = 0; i < AllRealisticForestNames.Length; i++)
        {
            string s = AllRealisticForestNames[i];
            //comes out like this
            //wanna trim it for user readability
            if (s.Contains("plants"))
            {
                //assets/polygonnature/prefabs/plants/sm_plant_hedge_bush_02.prefab
                s = s.Replace("assets/polygonnature/prefabs/plants/sm_plant_", string.Empty);
                s = s.Replace('_', ' ');
                s = s.Replace(".prefab", string.Empty);

            }
            else if (s.Contains("props"))
            {
                //assets/polygonnature/prefabs/props/sm_prop_campfire_01
                s = s.Replace("assets/polygonnature/prefabs/props/sm_prop_", string.Empty);
                s = s.Replace('_', ' ');
                s = s.Replace(".prefab", string.Empty);
            }
            else if (s.Contains("trees"))
            {
                //assets/polygonnature/prefabs/trees/sm_tree_generic_giant_01
                s = s.Replace("assets/polygonnature/prefabs/trees/sm_tree_", string.Empty);
                s = s.Replace('_', ' ');
                s = s.Replace(".prefab", string.Empty);
            }
            else if (s.Contains("rocks"))
            {
                //assets/polygonnature/prefabs/rocks/sm_rock_cluster_large_01
                s = s.Replace("assets/polygonnature/prefabs/rocks/sm_rock_", string.Empty);
                s = s.Replace('_', ' ');
                s = s.Replace(".prefab", string.Empty);
            }
            //print(s);
            AllRealisticForestNames[i] = s;
        }
    }
}
