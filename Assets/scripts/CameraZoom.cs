using UnityEngine;
using UnityEditor;
using System.Collections;

public class CameraZoom : MonoBehaviour {

    public static CameraZoom instance;
    Vector2 origin;
    // Use this for initialization
    void Start()
    {
        origin = Camera.main.transform.position;
        instance = this;
    }

    public IEnumerator Zoom(Transform target)
    {
        while (Camera.main.orthographicSize > 2)
        {
            Camera.main.orthographicSize -= Time.deltaTime * 20;
            Camera.main.transform.position = Vector2.Lerp(Camera.main.transform.position, target.position, Time.deltaTime * 10);
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -10);
            yield return new WaitForEndOfFrame();
        }

        float timer = 0;

        while (timer < 2)
        {
            timer += Time.deltaTime;
            Camera.main.transform.position = Vector2.Lerp(Camera.main.transform.position, target.position, Time.deltaTime * 10);
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -10);
            yield return new WaitForEndOfFrame();
        }

        while (Camera.main.orthographicSize < 9)
        {
            Camera.main.orthographicSize += Time.deltaTime * 20;
            Camera.main.transform.position = Vector2.Lerp(Camera.main.transform.position, origin, Time.deltaTime * 10);
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -10);
            yield return new WaitForEndOfFrame();
        }
        Camera.main.transform.position = origin;
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -10);
        Camera.main.orthographicSize = 9;
    }

    public void doAZoom()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(Zoom(player.transform));
    }

	// Update is called once per frame
	void Update () {
	
	}
}


[CustomEditor(typeof(CameraZoom))]
public class CameraZoomer : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (EditorApplication.isPlaying)
        {
            if (GUILayout.Button("Do a zoom"))
            {
                CameraZoom.instance.doAZoom();
            }
        }
    }
}
