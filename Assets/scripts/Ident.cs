using UnityEngine;
using UnityEngine.SceneManagement;

public class Ident : MonoBehaviour
{
 public   AudioClip[] honks;
    public AudioSource honkSound;
    
	public void loadLevel(int level)
    {
        SceneManager.LoadScene(level);
    }
    public void selectRandomHonk()
    {
        Random.seed = System.DateTime.Now.Millisecond;
        honkSound.clip = honks[Random.Range(0, honks.Length)];
    }
}
