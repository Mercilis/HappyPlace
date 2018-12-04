using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivityManager : MonoBehaviour {

    public AssetBundleCreateRequest ActivityBundleCreateRequest { get; private set; }
    public AssetBundle ActivityBundle { get; private set; }
    private Activity[] m_activities = null;

    private void Awake()
    {
        LoadActivityAssetBundle();
    }

    public void LoadActivityAssetBundle()
    {

    }
}
