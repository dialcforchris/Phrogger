using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Manages teh PW splash screen.
/// </summary>
public class PlayWestSplash : MonoBehaviour {

    public string levelToLoad;
    public float splashDuration;
    [SerializeField] private GameObject blackScreen = null;
    private bool skip = false;

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

    private void Update()
    {
        if (skip)
        {
            SceneManager.LoadScene(3);
        }
        if (Input.GetButton("Fire0") || Input.GetButton("Fire1"))
        {
            blackScreen.gameObject.SetActive(true);
            skip = true;
        }
    }
}
