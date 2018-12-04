using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivityManager : MonoBehaviour {

    private Activity[] m_activities = null;

    private void Awake()
    {
        LoadActivityAssetBundle();
    }

    public void LoadActivityAssetBundle()
    {

    }
}
