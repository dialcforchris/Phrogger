using UnityEngine;
using UnityEngine.SceneManagement;

public class Ident : MonoBehaviour
{
	public void loadLevel(int level)
    {
        SceneManager.LoadScene(level);
    }
}
