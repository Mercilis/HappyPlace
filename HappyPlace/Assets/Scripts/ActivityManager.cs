using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ActivityManager : MonoBehaviour {

    //maybe one day necessary but not right now
    //public AssetBundleCreateRequest ActivityBundleCreateRequest { get; private set; }
    //public AssetBundle ActivityBundle { get; private set; }
    [SerializeField]
    private Activity[] m_activities = null;
    [SerializeField]
    private GameObject m_buttonPrefab = null;
    [SerializeField]
    private GameObject m_buttonStorage = null;
    [SerializeField]
    private GameObject m_closeActivityButton = null;
    [SerializeField]
    private GameObject m_activitiesDisplay = null;

    private GameObject m_currentLoadedActivity = null;

    private void Awake()
    {
        CreateActivityMenuButtons();
    }

    private void OnEnable()
    {
        if(m_currentLoadedActivity != null)
        {
            m_activitiesDisplay.SetActive(false);
            m_closeActivityButton.SetActive(true);
        }
    }

    public void CloseCurrentActivity()
    {
        m_currentLoadedActivity.SetActive(false);
        m_closeActivityButton.SetActive(false);
        m_activitiesDisplay.SetActive(true);
    }

    public void CreateActivityMenuButtons()
    {
        for (int i = 0; i < m_activities.Length; i++)
        {
            Activity act = m_activities[i];
            GameObject button = Instantiate(m_buttonPrefab, m_buttonStorage.transform, false);

            TextMeshProUGUI[] buttonFields = button.GetComponentsInChildren<TextMeshProUGUI>();
            if (act == null) print("activity null");
            print("number of button fields: " + buttonFields.Length);
            buttonFields[0].text = act.ActivityName;
            buttonFields[1].text = act.ActivityDescription;
            button.GetComponent<Button>().onClick.AddListener(delegate { LoadSelectedActivity(act); });
            act.gameObject.SetActive(false);
        }
    }

    public void LoadSelectedActivity(Activity act)
    {
        act.gameObject.SetActive(true);
        m_currentLoadedActivity = act.gameObject;
    }
}
