using UnityEngine;

public class Coin : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.name == "Body")
        {
            GetComponent<AudioSource>().Play();
            this.enabled = false;
            this.GetComponent<MeshRenderer>().enabled = false;
            Destroy(this.gameObject, 2);
        }
    }
}
