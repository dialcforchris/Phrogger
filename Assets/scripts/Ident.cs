using UnityEngine;
using UnityEngine.SceneManagement;

public class Ident : MonoBehaviour
{
 public   AudioClip[] honks;
    public AudioSource honkSound;
    
    private void Awake()
    {
        Cursor.visible = false;
    }

	public void loadLevel(int level)
    {
        SceneManager.LoadScene(level);  
    }
    public void selectRandomHonk()
    {
        Random.seed = System.DateTime.Now.Millisecond;
        honkSound.clip = honks[Random.Range(0, honks.Length)];
    }

    private void Update()
    {
        if (Input.GetButton("Fire0") || Input.GetButton("Fire1"))
        {
            loadLevel(3);
        }
    }
}
