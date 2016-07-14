using UnityEngine;
using UnityEngine.UI;
//using UnityEditor;
using System.Collections;
using DG.Tweening;

public class CameraZoom : MonoBehaviour {

    public static CameraZoom instance;
    Vector2 origin;
    public Image overlayTop, overlayBot;

    // Use this for initialization
    void Start()
    {
        origin = new Vector3(0.0f, 9.5f, -10.0f);
        instance = this;
    }

    public IEnumerator Zoom(Transform target,bool IntroZoom)
    {
        float lerpy = 0;
        if (!IntroZoom)
        {
            //yield return new WaitForSeconds(0.25f); //Wait for the boss to appear
            
            while (lerpy < 1)
            {
                lerpy += Time.deltaTime * 5;
                if (lerpy > 1)
                    lerpy = 1;

                //Black bars yo
                overlayBot.rectTransform.anchoredPosition = Vector2.Lerp(new Vector2(960, -410), new Vector2(960, -540), 1 - lerpy);
                overlayTop.rectTransform.anchoredPosition = Vector2.Lerp(new Vector2(960, 540), new Vector2(960, 670), 1 - lerpy);
                yield return new WaitForEndOfFrame();
            }
        }
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
            while (timer < 1.25f)
            {
                timer += Time.deltaTime;
                Camera.main.transform.position = Vector2.Lerp(Camera.main.transform.position, target.position, Time.deltaTime * 10);
                Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -10);
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            SoundManager.instance.music.Play();
            SoundManager.instance.music.DOFade(1, 5);
            yield return new WaitForSeconds(0.5f);
        }

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

        lerpy = 0;
        while (lerpy < 1)
        {
            lerpy += Time.deltaTime * 5;
            if (lerpy > 1)
                lerpy = 1;

            //Black bars yo
            overlayBot.rectTransform.anchoredPosition = Vector2.Lerp(new Vector2(960, -410), new Vector2(960, -540),  lerpy);
            instance.overlayTop.rectTransform.anchoredPosition = Vector2.Lerp(new Vector2(960, 540), new Vector2(960, 670),lerpy);
            yield return new WaitForEndOfFrame();
        }
        if(dayTimer.instance.time < dayTimer.instance.secondsDay)
        {
            GameStateManager.instance.ChangeState(GameStates.STATE_GAMEPLAY);
        }
        if (!IntroZoom)
            GameStateManager.instance.bossTransitioning = false;
    }

    public void doAZoom(bool b, Transform t = null)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (t != null)
            StartCoroutine(Zoom(t, b));
        else
            StartCoroutine(Zoom(player.transform, b));
    }
}


//[CustomEditor(typeof(CameraZoom))]
//public class CameraZoomer : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        DrawDefaultInspector();
//        if (EditorApplication.isPlaying)
//        {
//            if (GUILayout.Button("Do a zoom"))
//            {
//                CameraZoom.instance.doAZoom(false);
//            }
//        }
//    }
//}
