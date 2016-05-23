using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public string levelName;

    void OnTriggerEnter(Collider other)
    {
        if (other.name.StartsWith("Controller"))
        {
            this.GetComponent<Animation>().Play();
            this.GetComponent<AudioSource>().Play();
        }
    }

    void OnPushed()
    {
        SceneManager.LoadScene(levelName);
    }
}
