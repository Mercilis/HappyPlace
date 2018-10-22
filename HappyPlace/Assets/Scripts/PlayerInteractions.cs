using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class PlayerInteractions : MonoBehaviour {

    [SerializeField]
    private GameObject m_itemSpawnerPrefab = null;

    private GameObject m_itemSpawner = null;

    #region Controllers
    [SerializeField]
    private GameObject m_leftController = null;
    [SerializeField]
    private GameObject m_rightController = null;

    private VRTK_ControllerEvents m_leftControllerEvents = null;
    private VRTK_ControllerEvents m_rightControllerEvents = null;
    #endregion

    private GameManager m_gameManager = null;

    private void Start()
    {
        //This garbage sets up the controllers and event subscriptions
        m_gameManager = FindObjectOfType<GameManager>();
        if(m_gameManager != null)
        {
            m_rightController = m_gameManager.m_rightController;
            m_leftController = m_gameManager.m_leftController;
            if(m_rightController != null && m_leftController != null)
            {
                m_leftControllerEvents = m_leftController.GetComponent<VRTK_ControllerEvents>();
                m_rightControllerEvents = m_rightController.GetComponent<VRTK_ControllerEvents>();
                if(m_leftControllerEvents != null)
                {
                    m_leftControllerEvents.GripClicked += OpenObjectSpawningMenu;
                    m_leftControllerEvents.GripUnclicked += CloseObjectSpawningMenu;
                }
            }
        }
    }

    // Update is called once per frame
    void Update () {
		
	}

    private void OpenObjectSpawningMenu(object sender, ControllerInteractionEventArgs e)
    {
        if (m_itemSpawner != null && !m_itemSpawner.activeInHierarchy && m_gameManager.GameState == GameManager.eGameState.EDIT)
        {
            m_itemSpawner.SetActive(true);
        }
    }

    private void CloseObjectSpawningMenu(object sender, ControllerInteractionEventArgs e)
    {
        if (m_itemSpawner != null && m_itemSpawner.activeInHierarchy && m_gameManager.GameState == GameManager.eGameState.EDIT)
        {
            m_itemSpawner.SetActive(false);
        }
    }
}
