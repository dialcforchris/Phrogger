using UnityEngine;
using UnityEngine.UI;

public class flash : MonoBehaviour {

    public Text r;
    bool flashOn;
    int i=0;
    public int flashSpeed;
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
            if (i > flashSpeed)
            {
                flashOn = true;
                i = 0;
            }
        }
	}
}
