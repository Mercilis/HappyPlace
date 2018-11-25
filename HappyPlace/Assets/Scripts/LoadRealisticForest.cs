using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadRealisticForest : MonoBehaviour {

    public AssetBundle RealistForestAssetBundle { get; private set; }
    public string[] AllRealisticForestNames { get; private set; }

    private GameManager m_gameManager = null;
    private AssetBundleCreateRequest m_loadedAssetBundle;

    private bool m_done = false;

    private void Awake()
    {
        if(FindObjectsOfType<LoadRealisticForest>().Length > 1)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        m_gameManager = FindObjectOfType<GameManager>();

        if(m_gameManager.RealistForestAssetBundle == null)
        {
            m_loadedAssetBundle = AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, "realisticforest"));
            print("loading realistic forest asset bundle");
        }
    }

    private void Update()
    {
        if(m_loadedAssetBundle.isDone && !m_done && m_gameManager.RealistForestAssetBundle == null)
        {
            m_done = true;
            RealistForestAssetBundle = m_loadedAssetBundle.assetBundle;
            AllRealisticForestNames = RealistForestAssetBundle.GetAllAssetNames();

            m_gameManager.FinalizeLoadingRealisticForest();
        }
    }
}
