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
    private Canvas m_categoryCanvas = null;
    [SerializeField]
    private Canvas m_backCanvas = null;

    private List<Button> m_menuButtons = new List<Button>();
    private List<GameObject>[] m_objects;
    private List<GameObject> m_activeMenuObjects = new List<GameObject>();

    private int m_currentCategoryNum = 0;
    private float m_baseMenuObjectScale = 0.3f;

    private BoxCollider m_displayBounds = null;
    private GameManager m_gameManager = null;

    private VRTK_Lever m_lever = null;
    private int m_valueThresholdForMovement = 5;
    private float m_displayDisplacement = 0.0f;
    private float m_maxDistanceForDisplay = 10.0f;
    private bool m_objectsBeingDisplayed = false;
    private float m_displayMovementSpeedBase = 0.5f;

    private void Start()
    {
        m_gameManager = FindObjectOfType<GameManager>();
        m_gameManager.PlacedObjectStorage = GameObject.FindGameObjectWithTag("PlacedObjectStorage");
        m_mainObjectContainer = m_gameManager.m_MainObjectStorage;
        m_displayBounds = GetComponent<BoxCollider>();
        m_lever = GetComponentInChildren<VRTK_Lever>();
        m_categoryCanvas.gameObject.SetActive(true);
        m_backCanvas.gameObject.SetActive(false);

        if(m_categoryCanvas != null && m_buttonTemplate != null)
        {
            m_objects = m_gameManager.ObjectsSortedByCategory;
            print("category canvas and button template not null");
            for (int i = 0; i < m_objects.Length; i++)
            {
                GameObject obj = Instantiate(m_buttonTemplate, m_categoryCanvas.transform);
                Button newButton = obj.GetComponent<Button>();
                obj.GetComponentInChildren<TextMeshProUGUI>().text = m_gameManager.CurrentEnvironmentCategories[i];
                print(m_gameManager.CurrentEnvironmentCategories[i] + " button created");
                newButton.onClick.AddListener(delegate { OnCategoryClicked(newButton); });
                m_menuButtons.Add(newButton);
            }
        }
    }

    private void Update()
    {
        Vector3 pos = m_worldObjectDisplay.transform.position;
        float movement = (m_displayMovementSpeedBase * m_lever.GetValue()) * Time.deltaTime;
        if(Mathf.Abs(m_lever.GetValue()) > m_valueThresholdForMovement && Mathf.Abs(m_displayDisplacement + movement) <= m_maxDistanceForDisplay && m_objectsBeingDisplayed)
        {
            //Move the menu
            m_displayDisplacement += movement;
            m_worldObjectDisplay.transform.position = new Vector3(pos.x + movement, pos.y, pos.z);
            for (int i = 0; i < m_activeMenuObjects.Count; i++)
            {
                CalculateObjectsVisibility(m_activeMenuObjects[i]);
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
        //float distanceFromBoxCenter = 0.0f;
        //distanceFromBoxCenter = Mathf.Abs(m_displayBounds.center.x - obj.transform.position.x) / (m_displayBounds.size.x / 2);
        //print(distanceFromBoxCenter);
        //distanceFromBoxCenter = Mathf.Abs(1 - distanceFromBoxCenter);
        ////print(distanceFromBoxCenter);

        //if(distanceFromBoxCenter > 1)
        //{
        //    float scaleMod = Mathf.Lerp(0.2f, 1, distanceFromBoxCenter);
        //    scaleMod = (m_baseMenuObjectScale * scaleMod);
        //    //print("scalemod: " + scaleMod);
        //    obj.transform.localScale = new Vector3(scaleMod, scaleMod, scaleMod);
        //}

        //print("display size x: " + m_displayBounds.size.x);
        if(obj.GetComponent<PlaceableObject>().ObjectState == PlaceableObject.eObjectState.IN_MENU)
        {
            if(!m_displayBounds.bounds.Contains(obj.transform.position))
            {
                obj.layer = 2;
                List<MeshRenderer> renderers = new List<MeshRenderer>(obj.GetComponentsInChildren<MeshRenderer>());
                renderers.Add(obj.GetComponent<MeshRenderer>());
                for (int i = 0; i < renderers.Count; i++)
                {
                    if(renderers[i] != null) renderers[i].enabled = false;
                }
            }
            else
            {
                obj.layer = 0;
                List<MeshRenderer> renderers = new List<MeshRenderer>(obj.GetComponentsInChildren<MeshRenderer>());
                renderers.Add(obj.GetComponent<MeshRenderer>());
                for (int i = 0; i < renderers.Count; i++)
                {
                    if (renderers[i] != null) renderers[i].enabled = true;
                }
            }
        }
    }

    private void OnCategoryClicked(Button button)
    {
        print(button.GetComponentInChildren<TextMeshProUGUI>().text + " category clicked");
        m_categoryCanvas.gameObject.SetActive(false);
        m_backCanvas.gameObject.SetActive(true);
        m_objectsBeingDisplayed = true;
        int categoryNum = m_menuButtons.IndexOf(button);
        m_currentCategoryNum = categoryNum;
        float distanceMod = 0.0f;

        if ( m_objects != null)
        {

            for (int i = 0; i < m_objects[categoryNum].Count; i++)
            {
                GameObject obj = m_objects[categoryNum][i];
                float xDistanceMod = i % 2 == 0 ? distanceMod : distanceMod * -1 - 0.625f;
                Transform objTrans = obj.transform;
                objTrans.SetParent(m_worldObjectDisplay.transform, true);
                objTrans.gameObject.SetActive(true);
                objTrans.position = new Vector3(xDistanceMod, objTrans.position.y + 1f, objTrans.position.z);
                objTrans.localScale = new Vector3(m_baseMenuObjectScale, m_baseMenuObjectScale, m_baseMenuObjectScale);
                objTrans.GetComponent<VRTK_InteractableObject>().InteractableObjectGrabbed += CreateAndReplaceObject;
                obj.GetComponent<PlaceableObject>().DistanceMod = xDistanceMod;
                obj.GetComponent<PlaceableObject>().ObjectState = PlaceableObject.eObjectState.IN_MENU;
                //print("when category clicked, object name is: " + obj.GetComponent<PlaceableObject>().ObjectName);
                if(i == m_objects[categoryNum].Count - 1)
                {
                    m_maxDistanceForDisplay = Mathf.Abs(distanceMod) - (m_displayBounds.size.x/2.0f);
                }
                distanceMod += 0.5f;
            }
            m_worldObjectDisplay.transform.SetParent(transform, false);
            for (int i = 0; i < m_objects[categoryNum].Count; i++)
            {
                CalculateObjectsVisibility(m_objects[categoryNum][i]);
            }
        }
    }
    
    private void MakeAMenuObject(GameObject obj, float distanceMod)
    {
        //print("make a menu object called for " + obj.name);
        obj.transform.SetParent(m_gameManager.PlacedObjectStorage.transform, true);
        GameObject newObj = Instantiate(obj);
        Transform objTrans = newObj.transform;
        objTrans.SetParent(m_worldObjectDisplay.transform, false);
        newObj.gameObject.SetActive(true);
        objTrans.position = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y, obj.transform.localPosition.z);
        //objTrans.SetParent(m_worldObjectDisplay.transform, false);
        objTrans.localScale = new Vector3(m_baseMenuObjectScale, m_baseMenuObjectScale, m_baseMenuObjectScale);
        newObj.GetComponent<PlaceableObject>().ObjectState = PlaceableObject.eObjectState.IN_MENU;
        string oldObjName = obj.GetComponent<PlaceableObject>().ObjectName;
        newObj.GetComponent<PlaceableObject>().ObjectName = oldObjName;
        //print("old obj name: " + oldObjName);
        newObj.GetComponent<VRTK_InteractableObject>().InteractableObjectGrabbed += CreateAndReplaceObject;
        obj.GetComponent<VRTK_InteractableObject>().InteractableObjectGrabbed -= CreateAndReplaceObject;
        int indexOfObject = m_objects[m_currentCategoryNum].IndexOf(obj);
        m_objects[m_currentCategoryNum][indexOfObject] = newObj;
        CalculateObjectsVisibility(newObj);
    }

    private void CreateAndReplaceObject(object sender, InteractableObjectEventArgs e)
    {
        GameObject grabbedObject = e.interactingObject.GetComponent<VRTK_InteractGrab>().GetGrabbedObject();

        grabbedObject.transform.SetParent(null, true);
        grabbedObject.transform.localScale = Vector3.one;
        MakeAMenuObject(grabbedObject, grabbedObject.GetComponent<PlaceableObject>().DistanceMod);
        //print("create and replace called");
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
        m_objectsBeingDisplayed = false;
        m_backCanvas.gameObject.SetActive(false);
        m_categoryCanvas.gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlaceableObject obj = other.GetComponent<PlaceableObject>();
        if (obj != null && obj.ObjectState == PlaceableObject.eObjectState.IN_MENU)
        {
            CalculateObjectsVisibility(other.gameObject);
            //add to a list of gameobjects to update
            m_activeMenuObjects.Add(other.gameObject);
            print("object enter");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlaceableObject obj = other.GetComponent<PlaceableObject>();
        if (obj != null && obj.ObjectState == PlaceableObject.eObjectState.IN_MENU)
        {
            if(m_activeMenuObjects.Contains(other.gameObject))
            {
                m_activeMenuObjects.Remove(other.gameObject);
            }
            CalculateObjectsVisibility(other.gameObject);
            print("object exit");
        }
    }
}