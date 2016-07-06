using UnityEngine;
using UnityEngine.SceneManagement;

public class Ident : MonoBehaviour
{
    public string level;

	public void loadLevel()
    {
        SceneManager.LoadScene(level);
    }
}
