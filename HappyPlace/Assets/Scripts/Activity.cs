using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activity : MonoBehaviour {

	public string ActivityName { get; protected set; }
    public string ActivityDescription { get; protected set; }

    public virtual void LoadActivity()
    {
        print("Loaded activity for " + ActivityName != null ? ActivityName : "null name");
    }
}
