using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activity : MonoBehaviour {

    public string ActivityName;
    public string ActivityDescription;

    public virtual void LoadActivity()
    {
        print("Loaded activity for " + ActivityName != null ? ActivityName : "null name");
    }

    public virtual void UnloadActivity()
    {
        print("Unloaded activity for " + ActivityName != null ? ActivityName : "null name");
    }
}
