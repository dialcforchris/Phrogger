using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class gameOverScreen : MonoBehaviour
{

    public static gameOverScreen instance;
    public Image screen;
    public AudioClip sadTromboneSound;
    
    void Start()
    {
        instance = this;
    }

    public IEnumerator TriggerGameOver()
    {
        //Fade in image
        while (screen.color.a < 1)
        {
            Color col = screen.color;
            col.a += Time.deltaTime;
            screen.color = col;
            yield return new WaitForEndOfFrame();
        }
        SoundManager.instance.playSound(sadTromboneSound);

        while (!Input.GetButtonDown("Fire" + multiplayerManager.instance.currentActivePlayer.ToString()))
            yield return null;

        StartCoroutine(MainMenu.instance.wholeScreenFade(true));
    }
}
