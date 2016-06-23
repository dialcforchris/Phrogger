using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;

public class CameraZoom : MonoBehaviour {

    public static CameraZoom instance;
    Vector2 origin;
    public Image overlayTop, overlayBot;

    // Use this for initialization
    void Start()
    {
        origin = Camera.main.transform.position;
        instance = this;
    }

    public IEnumerator Zoom(Transform target,bool IntroZoom)
    {
        if (!IntroZoom)
            yield return new WaitForSeconds(1.5f);
        GameStateManager.instance.ChangeState(GameStates.STATE_DAYOVER);
        if (!IntroZoom)
        {
            while (Camera.main.orthographicSize > 2)
            {
                Camera.main.orthographicSize -= Time.deltaTime * 20;
                Camera.main.transform.position = Vector2.Lerp(Camera.main.transform.position, target.position, Time.deltaTime * 10);
                Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -10);
                yield return new WaitForEndOfFrame();
            }
        }
        float timer = 0;

        if (!IntroZoom)
        {
            while (timer < 2)
            {
                timer += Time.deltaTime;
                Camera.main.transform.position = Vector2.Lerp(Camera.main.transform.position, target.position, Time.deltaTime * 10);
                Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -10);
                yield return new WaitForEndOfFrame();
            }
        }
        else
            yield return new WaitForSeconds(0.5f);

        while (Camera.main.orthographicSize < 8.9f)
        {
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize,9, Time.deltaTime * ((IntroZoom) ? 1 : 20));
            Camera.main.transform.position = Vector2.Lerp(Camera.main.transform.position, origin, Time.deltaTime * ((IntroZoom) ? 1 : 10));
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -10);
            yield return new WaitForEndOfFrame();
        }
        Camera.main.transform.position = origin;
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -10);
        Camera.main.orthographicSize = 9;
        while (overlayTop.rectTransform.anchoredPosition.y < 670)
        {
            overlayTop.rectTransform.anchoredPosition = new Vector2(overlayTop.rectTransform.anchoredPosition.x, overlayTop.rectTransform.anchoredPosition.y + Time.deltaTime * 75);
            overlayBot.rectTransform.anchoredPosition = new Vector2(overlayBot.rectTransform.anchoredPosition.x, overlayBot.rectTransform.anchoredPosition.y - Time.deltaTime * 75);
            yield return new WaitForEndOfFrame();
        }
        GameStateManager.instance.ChangeState(GameStates.STATE_GAMEPLAY);
    }

    public void doAZoom(bool b, Transform t = null)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (t != null)
            StartCoroutine(Zoom(t, b));
        else
            StartCoroutine(Zoom(player.transform, b));
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
                CameraZoom.instance.doAZoom(false);
            }
        }
    }
}
