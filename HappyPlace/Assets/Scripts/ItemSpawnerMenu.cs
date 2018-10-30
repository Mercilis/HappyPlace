using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using VRTK;
using Unity.Linq;

public class ItemSpawnerMenu : MonoBehaviour {

    [SerializeField]
    private GameObject m_buttonTemplate = null;
    [SerializeField]
    private GameObject m_worldObjectDisplay = null;
    [SerializeField]
    private GameObject m_mainObjectContainer = null;
    [SerializeField]
    private Canvas m_categoryCanvas;
    [SerializeField]
    private Canvas m_backCanvas;

    private List<Button> m_menuButtons = new List<Button>();
    private GameManager m_gameManager = null;
    private List<GameObject>[] m_objects;
    private int m_currentCategoryNum = 0;

    private BoxCollider m_displayBounds = null;
    private float m_baseMenuObjectScale = 0.3f;
    private float m_scaleMod = 0.0f;

    private void Start()
    {
        m_gameManager = FindObjectOfType<GameManager>();
        m_displayBounds = GetComponent<BoxCollider>();
        m_categoryCanvas.gameObject.SetActive(true);
        m_backCanvas.gameObject.SetActive(false);

        if(m_categoryCanvas != null && m_buttonTemplate != null)
        {
            m_objects = m_gameManager.ObjectsSortedByCategory;
            //for (int i = 0; i < m_gameManager.CurrentEnvironmentCategories.Length; i++)
            //{
            //    print(m_gameManager.CurrentEnvironmentCategories[i]);
            //}
            for (int i = 0; i < m_objects.Length; i++)
            {
                GameObject obj = Instantiate(m_buttonTemplate, m_categoryCanvas.transform);
                Button newButton = obj.GetComponent<Button>();
                obj.GetComponentInChildren<TextMeshProUGUI>().text = m_gameManager.CurrentEnvironmentCategories[i];
                newButton.onClick.AddListener(delegate { OnCategoryClicked(newButton); });
                m_menuButtons.Add(newButton);
            }
        }

        
    }

    public void OnLeverValueChanged(object sender, Control3DEventArgs args)
    {
        if(!m_categoryCanvas.gameObject.activeInHierarchy)
        {
            List<GameObject> menuObjects = m_objects[m_currentCategoryNum];
            for (int i = 0; i < menuObjects.Count; i++)
            {
                CalculateObjectsVisibility(menuObjects[i]);
            }
        }
    }

    private void CalculateObjectsVisibility(GameObject obj)
    {
        float distanceFromBoxCenter = 0.0f;
        distanceFromBoxCenter = Mathf.Abs(m_displayBounds.center.x - obj.transform.position.x);
        print(distanceFromBoxCenter);

        distanceFromBoxCenter = Mathf.Abs(1 - distanceFromBoxCenter);
        print(distanceFromBoxCenter);

        float scaleMod = Mathf.Clamp(distanceFromBoxCenter, 0.2f, 1);
        scaleMod = (m_baseMenuObjectScale * scaleMod);
        print("scalemod: " + scaleMod);
        obj.transform.localScale = new Vector3(scaleMod, scaleMod, scaleMod);

        //float alphaMod = Mathf.Clamp(distanceFromBoxCenter, 0, 1);
        float alphaOff = m_displayBounds.size.x - Mathf.Abs(m_displayBounds.center.x - obj.transform.position.x);
        if(alphaOff < 0)
        {
            List<MeshRenderer> renderers = new List<MeshRenderer>(obj.GetComponentsInChildren<MeshRenderer>());
            renderers.Add(obj.GetComponent<MeshRenderer>());
            for (int i = 0; i < renderers.Count; i++)
            {
                if(renderers[i] != null) renderers[i].enabled = false;
            }
        }
    }

    private void OnCategoryClicked(Button button)
    {
        m_categoryCanvas.gameObject.SetActive(false);
        m_backCanvas.gameObject.SetActive(true);
        int categoryNum = m_menuButtons.IndexOf(button);
        m_currentCategoryNum = categoryNum;
        print("category clicked");
        float distanceMod = 0.0f;

        if ( m_objects != null)
        {
            for (int i = 0; i < m_objects[categoryNum].Count; i++)
            {
                GameObject obj = m_objects[categoryNum][i];
                float xMod = (i % 2 == 0 ? distanceMod : (-1 * distanceMod));
                Transform objTrans = obj.transform;
                objTrans.SetParent(m_worldObjectDisplay.transform, true);
                objTrans.gameObject.SetActive(true);
                objTrans.position = new Vector3((i % 2 == 0 ? distanceMod : (-1 * distanceMod)), objTrans.position.y + 0.5f, objTrans.position.z);
                objTrans.localScale = new Vector3(m_baseMenuObjectScale, m_baseMenuObjectScale, m_baseMenuObjectScale);
                objTrans.GetComponent<VRTK_InteractableObject>().InteractableObjectGrabbed += CreateAndReplaceObject;
                obj.GetComponent<PlaceableObject>().DistanceMod = xMod;
                CalculateObjectsVisibility(obj);
                distanceMod += 0.5f;
            }
            m_worldObjectDisplay.transform.SetParent(transform, false);
        }
    }
    
    private void MakeAMenuObject(GameObject obj, float distanceMod)
    {
        GameObject newObj = Instantiate(obj, m_worldObjectDisplay.transform, false);
        Transform objTrans = newObj.transform;
        objTrans.gameObject.SetActive(true);
        objTrans.SetParent(m_worldObjectDisplay.transform, false);
        objTrans.position = new Vector3(distanceMod, obj.transform.position.y, obj.transform.position.z);
        objTrans.localScale = new Vector3(m_baseMenuObjectScale, m_baseMenuObjectScale, m_baseMenuObjectScale);
        //objTrans.GetComponent<VRTK_InteractableObject>().InteractableObjectGrabbed += CreateAndReplaceObject;
    }

    private void CreateAndReplaceObject(object sender, InteractableObjectEventArgs e)
    {
        GameObject grabbedObject = e.interactingObject.GetComponent<VRTK_InteractGrab>().GetGrabbedObject();

        grabbedObject.transform.SetParent(null, true);
        grabbedObject.transform.localScale = Vector3.one;
        MakeAMenuObject(grabbedObject, grabbedObject.GetComponent<PlaceableObject>().DistanceMod);
        print("create and replace called");
    }

    public void OnBackButtonClicked()
    {
        if (m_objects != null)
        {
            for (int i = 0; i < m_objects[m_currentCategoryNum].Count; i++)
            {
                Transform obj = m_objects[m_currentCategoryNum][i].transform;
                obj.gameObject.SetActive(false);
                obj.SetParent(m_mainObjectContainer.transform, false);
                obj.position = Vector3.zero;
            }
            m_worldObjectDisplay.transform.SetParent(null, true);
        }

        m_backCanvas.gameObject.SetActive(false);
        m_categoryCanvas.gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        CalculateObjectsVisibility(other.gameObject);
    }
}