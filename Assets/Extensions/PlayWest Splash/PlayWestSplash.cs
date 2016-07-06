using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Manages teh PW splash screen.
/// </summary>
public class PlayWestSplash : MonoBehaviour {

    public string levelToLoad;
    public float splashDuration;

	// Use this for initialization
	void Start () {

        StartCoroutine(WaitForLevelLoad());
	}

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForLevelLoad() {

        yield return new WaitForSeconds(splashDuration);

        SceneManager.LoadScene(levelToLoad);
    }
}
