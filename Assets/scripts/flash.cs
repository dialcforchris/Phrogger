using UnityEngine;
using System.Collections;

public class flash : MonoBehaviour {

    public Renderer r;
    bool flashOn;
    int i=0;
    void FixedUpdate()
    {
        if (flashOn)
        {
            r.enabled = !r.enabled;
            flashOn = false;
        }
        else
        {
            i++;
            if (i > 2)
            {
                flashOn = true;
                i = 0;
            }
        }
	}
}
