﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VRTK;

public class GameManager : MonoBehaviour {

    public enum eGameState
    {
        EDIT,
        PLAY,
        MAINMENU,
        PAUSE
    }

    #region AssetBundleStuff
    private AssetBundle m_realistForestAssetBundle;
    public string[] AllRealisticForestNames { get; private set; }
    public string[] AllRealisticForestSimplifiedNames { get; private set; }
    #endregion

    #region PlayerControllers
    [SerializeField]
    public GameObject m_leftController = null;
    [SerializeField]
    public GameObject m_rightController = null;
    PlayerInteractions m_playerInteractions = null;
    #endregion

    public eGameState GameState { get; private set; }

    // Use this for initialization
    void Start()
    {
        GameState = eGameState.EDIT;

        var myLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "realisticforest"));
        if (myLoadedAssetBundle == null)
        {
            Debug.Log("Failed to load AssetBundle realisticforest!");
            return;
        }
        m_realistForestAssetBundle = myLoadedAssetBundle;
        AllRealisticForestNames = m_realistForestAssetBundle.GetAllAssetNames();
        AllRealisticForestSimplifiedNames = new string[AllRealisticForestNames.Length];
        CleanUpRealisticForestNames();

        m_playerInteractions = FindObjectOfType<PlayerInteractions>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            SpawnTreeTest();
        }
    }

    private void SpawnTreeTest()
    {
        
        GameObject prefab = m_realistForestAssetBundle.LoadAsset<GameObject>(AllRealisticForestNames[(int)(Random.value * AllRealisticForestNames.Length)]);
        if(!prefab.GetComponent<Rigidbody>())
        {
            prefab.AddComponent<BoxCollider>();

            BoxCollider boxCollider = prefab.GetComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            MeshRenderer renderer = prefab.GetComponentInChildren<MeshRenderer>();
            boxCollider.center = renderer.bounds.center;
            boxCollider.size = renderer.bounds.size;

            Rigidbody body = prefab.AddComponent<Rigidbody>();
            body.isKinematic = true;
            //body.useGravity = false;
            //body.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

            VRTK_InteractableObject interactableObject = prefab.AddComponent<VRTK_InteractableObject>();
            interactableObject.allowedTouchControllers = VRTK_InteractableObject.AllowedController.Both;
            interactableObject.isGrabbable = true;
            interactableObject.holdButtonToGrab = false;
            interactableObject.stayGrabbedOnTeleport = true;
            interactableObject.holdButtonToUse = false;
            interactableObject.touchHighlightColor = Color.blue;

            VRTK.Highlighters.VRTK_OutlineObjectCopyHighlighter outline = prefab.AddComponent<VRTK.Highlighters.VRTK_OutlineObjectCopyHighlighter>();
            outline.active = true;
            outline.unhighlightOnDisable = true;
            prefab.AddComponent<PlaceableObject>();
        }
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
        if(name.Contains("tree"))
        {
            boxCollider.size.Scale(new Vector3(0.5f, 0.5f, 1.0f));
        }
        prefab.AddComponent<Rigidbody>();
        prefab.GetComponent<Rigidbody>().isKinematic = true;
        //prefab.AddComponent<VRTK.VRTK_InteractUse>();
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
            print(s);
            AllRealisticForestSimplifiedNames[i] = s;
        }
    }
}
