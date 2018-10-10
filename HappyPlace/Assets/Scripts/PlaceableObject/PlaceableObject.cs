using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class PlaceableObject : VRTK_InteractableObject {
    /// <summary>
    /// The distance at which the object with hover above the ground when it is selected.
    /// </summary>
    private const int HOVER_HEIGHT = 10;
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
    private Color RED = new Color(1, 0, 0);
    /// <summary>
    /// Color used for various indicators to indicate the object is valid.
    /// </summary>
    private Color GREEN = new Color(0, 1, 0);
    /// <summary>
    /// Indicates that the objects position is valid for placing down on the ground. Cannot be placed if this is not true.
    /// </summary>
    public bool IsValidPlacement { get; private set; }
    /// <summary>
    /// Indicates that the object is selected by the player and will be able to be moved around, modified, or placed.
    /// </summary>
    public bool IsSelected { get; private set; }
    /// <summary>
    /// Collider will be a trigger to determine if it overlaps with other objects, because if it does then it is not a valid placement.
    /// </summary>
    private Collider m_collider;
    /// <summary>
    /// Will keep track of how many objects are overlapping
    /// </summary>
    private int m_currentCollisions = 0;
    /// <summary>
    /// Need a kinematic rigidbody in order for the object to be movable and still check for collisions.
    /// </summary>
    private Rigidbody m_rigidbody;

    private void Start()
    {
        m_collider = GetComponent<Collider>();
        m_rigidbody = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //If another collider collides with this object's, then IsValidPlacement will be set to false
        IsValidPlacement = false;
        m_currentCollisions++;
        //Will need to change any visual indicators to RED
    }

    private void OnTriggerExit(Collider other)
    {
        //Check if there are any other objects overlapping other than the one that just left, if there isn't then change IsValidPlacement to true
        m_currentCollisions--;
        if(m_currentCollisions <= 0)
        {
            //If IsValidPlacement is changed to true, then will need to change any visual indicators to GREEN
            IsValidPlacement = true;
        }
    }

    public override void StartUsing(VRTK_InteractUse usingObject)
    {
        base.StartUsing(usingObject);
        //spinSpeed = 360f;
    }

    public override void StopUsing(VRTK_InteractUse usingObject)
    {
        base.StopUsing(usingObject);
        //spinSpeed = 0f;
    }

    protected override void Update()
    {
        base.Update();
    }
}
