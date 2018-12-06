using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlocksActivity : Activity {

    [SerializeField]
    private GameObject m_playTable = null;

    private List<GameObject> m_playBlocks;
    

	// Use this for initialization
	void Start () {
        ActivityName = "Building Blocks";
        ActivityDescription = "Colorful blocks to stack and build with.";
        
        m_playBlocks = new List<GameObject>(GetComponentsInChildren<GameObject>());
        m_playBlocks.Remove(m_playTable);
        //LoadActivity();
	}

    private void OnEnable()
    {
        LoadActivity();
    }

    public override void LoadActivity()
    {
        base.LoadActivity();
        GameObject player = FindObjectOfType<GameManager>().CameraRig;
        transform.position = player.transform.position + player.transform.forward * 2.0f;
    }

    public override void UnloadActivity()
    {
        base.UnloadActivity();
    }

    private void OnTriggerExit(Collider other)
    {
        //don't want to reset the object if the player is just holding it
        if(other.tag == "BuildingBlock")
        {
            other.transform.localPosition = Vector3.zero;
        }
    }
}
