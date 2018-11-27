using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
//[RequireComponent(typeof(VRTK_InteractableObject))]
public class PlaceableObject : MonoBehaviour {

    public enum eObjectState
    {
        IN_MENU,
        IN_WORLD
    }

    public eObjectState ObjectState { get; set; }

    /// <summary>
    /// The distance at which the object with hover above the ground when it is selected.
    /// </summary>
    private const int HOVER_HEIGHT = 2;
    //delete later
        ////Fetch the Renderer from the GameObject
        //Renderer rend = GetComponent<Renderer>();

        ////Set the main Color of the Material to green
        //rend.material.shader = Shader.Find("_Color");
        //    rend.material.SetColor("_Color", Color.green);

        //    //Find the Specular shader and change its Color to red
        //    rend.material.shader = Shader.Find("Specular");
        //    rend.material.SetColor("_SpecColor", Color.red);
    /// <summary>
    /// Color used for various indicators to indicate the object is invalid.
    /// </summary>
    private Color COLOR_INVALID = Color.red;
    /// <summary>
    /// Color used for various indicators to indicate the object is valid.
    /// </summary>
    private Color COLOR_VALID = Color.green;
    /// <summary>
    /// Color used for various indicators to indicate the object is selected.
    /// </summary>
    private Color COLOR_SELECTED = Color.blue;
    /// <summary>
    /// Indicates that the objects position is valid for placing down on the ground. Cannot be placed if this is not true.
    /// </summary>
    public bool IsValidPlacement { get; private set; }
    /// <summary>
    /// Indicates that the object is selected by the player and will be able to be moved around, modified, or placed.
    /// </summary>
    public bool IsSelected { get; private set; }
    public bool IsGrabbed { get; private set; }
    /// <summary>
    /// Collider will be a trigger to determine if it overlaps with other objects, because if it does then it is not a valid placement.
    /// </summary>
    private Collider m_collider;
    /// <summary>
    /// Will keep track of how many objects are overlapping
    /// </summary>
    [SerializeField]
    private int m_currentCollisions = 0;
    /// <summary>
    /// Need a kinematic rigidbody in order for the object to be movable but not react to physics and still check for collisions.
    /// Rigidbody also necessary for te VRTK_Interactableoject to work.
    /// </summary>
    private Rigidbody m_rigidbody;

    private VRTK_InteractableObject m_interactableObject;
    public string ObjectName { get; set; }
    //I don't know how to organize this so the mod for when an object is in the menu will go here
    public float DistanceMod { get; set; }

    private GameManager m_gameManager = null;

    private void Start()
    {
        m_collider = GetComponent<Collider>();
        m_rigidbody = GetComponent<Rigidbody>();
        m_interactableObject = GetComponent<VRTK_InteractableObject>();
        SetUpInteractableObjectEventListeners();
        m_gameManager = FindObjectOfType<GameManager>();
        //ObjectState = eObjectState.IN_MENU;
    }

    private void Update()
    {
        if(IsGrabbed)
        {
            Vector3 pos = transform.position;
            transform.position = new Vector3(pos.x, HOVER_HEIGHT, pos.z);
        }
    }

    /// <summary>
    /// This method will subscribe all of the PlaceableObjects methods that need to be called when certain events are triggered by the VRTK_InteractableObject
    /// </summary>
    private void SetUpInteractableObjectEventListeners()
    {
        ///// <summary>
        ///// Emitted when another object touches the current object.
        ///// </summary>
        //public event InteractableObjectEventHandler InteractableObjectTouched;
        ///// <summary>
        ///// Emitted when the other object stops touching the current object.
        ///// </summary>
        //public event InteractableObjectEventHandler InteractableObjectUntouched;
        ///// <summary>
        ///// Emitted when another object grabs the current object (e.g. a controller).
        ///// </summary>
        //public event InteractableObjectEventHandler InteractableObjectGrabbed;
        ///// <summary>
        ///// Emitted when the other object stops grabbing the current object.
        ///// </summary>
        //public event InteractableObjectEventHandler InteractableObjectUngrabbed;
        if(m_interactableObject == null)
        {
            m_interactableObject = GetComponent<VRTK_InteractableObject>();
        }
        m_interactableObject.InteractableObjectTouched += InteractableObjectTouchedListener;
        m_interactableObject.InteractableObjectUntouched += InteractableObjectUntouchedListener;
        m_interactableObject.InteractableObjectGrabbed += InteractableObjectGrabbedListener;
        m_interactableObject.InteractableObjectUngrabbed += InteractableObjectUngrabbedListener;
    }

    private void InteractableObjectGrabbedListener(object sender, InteractableObjectEventArgs e)
    {
        if (ObjectState == eObjectState.IN_MENU)
        {
            ObjectState = eObjectState.IN_WORLD;
        }

        IsGrabbed = true;
        //print("grab listened!");
        //m_interactableObject.touchHighlightColor = COLOR_VALID;
        //m_interactableObject.ToggleHighlight(true);
        //print("placeable object listener turns on highlight");
    }

    private void InteractableObjectUngrabbedListener(object sender, InteractableObjectEventArgs e)
    {
        if(ObjectState == eObjectState.IN_WORLD)
        {
            if (transform.parent != m_gameManager.PlacedObjectStorage.transform) transform.SetParent(m_gameManager.PlacedObjectStorage.transform, true);
            IsGrabbed = false;
            //m_interactableObject.touchHighlightColor = COLOR_SELECTED;
            //m_interactableObject.ToggleHighlight(false);
            Vector3 pos = transform.position;
            transform.position = new Vector3(pos.x, m_gameManager.GLOBAL_FLOOR_HEIGHT, pos.z);
        }
    }

    private void InteractableObjectTouchedListener(object sender, InteractableObjectEventArgs e)
    {
        //m_interactableObject.touchHighlightColor = COLOR_SELECTED;
    }

    private void InteractableObjectUntouchedListener(object sender, InteractableObjectEventArgs e)
    {

    }

    private void SetIsSelectedTrue()
    {
        IsSelected = true;
    }

    private void SetIsSelectedFalse()
    {
        IsSelected = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        //If another collider collides with this object's, then IsValidPlacement will be set to false
        //if(ObjectState == eObjectState.IN_WORLD && m_interactableObject != null && IsGrabbed && other.GetComponent<PlaceableObject>() != null)
        //{
        //    IsValidPlacement = false;
        //    m_currentCollisions++;
        //    m_interactableObject.touchHighlightColor = COLOR_INVALID;
        //    m_interactableObject.validDrop = VRTK_InteractableObject.ValidDropTypes.NoDrop;
        //}
        //Will need to change any visual indicators to RED
    }

    private void OnTriggerExit(Collider other)
    {
        //Check if there are any other objects overlapping other than the one that just left, if there isn't then change IsValidPlacement to true
        //if(ObjectState == eObjectState.IN_WORLD && other.GetComponent<PlaceableObject>() != null && m_currentCollisions <= 0)
        //{
        //    m_currentCollisions--;
        //    //If IsValidPlacement is changed to true, then will need to change any visual indicators to GREEN
        //    IsValidPlacement = true;
        //    if(m_interactableObject != null && IsGrabbed)
        //    {
        //        m_interactableObject.touchHighlightColor = COLOR_VALID;
        //        m_interactableObject.validDrop = VRTK_InteractableObject.ValidDropTypes.DropAnywhere;
        //    }
        //}
    }
    
}
