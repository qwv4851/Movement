using UnityEngine;

public class Skier : MonoBehaviour
{
    public float speed = 25f;
    public float moveHeight = 0.90f;
    public float outOfBoundsHeight = -80.0f;

    private Vector3 _startPos;
    [SerializeField]
    private GameObject _cameraHead;

    void Start ()
    {
        _startPos = this.transform.position;
    }
	
	void FixedUpdate ()
    {
        // Respawn when the HMD is intialized and if we've falled off the stage
        if ((SteamVR.active && !_cameraHead.GetComponent<SteamVR_TrackedObject>().isValid) || transform.position.y < outOfBoundsHeight)
        {
            Respawn();
        }

        // Update each ski pole
        UpdatePole(GameObject.Find("Controller (left)"), true);
        UpdatePole(GameObject.Find("Controller (right)"), false);
    }

    private void Respawn()
    {
        this.transform.position = _startPos;
        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    private void UpdatePole(GameObject controller, bool isLeft)
    {
        if (controller == null)
        {
            return;
        }

        int index = (int)controller.GetComponent<SteamVR_TrackedObject>().index;
        SteamVR_Controller.Device device = SteamVR_Controller.Input(index);

        // Get the controller velocity, ignoring the vertical component
        Vector3 velocity = Vector3.Scale(device.velocity, new Vector3(1, 0, 1)) * -speed;

        // If the controller is below moveHeight, use the velocity to push the player in the opposite direction
        if (controller.transform.localPosition.y < moveHeight)
        {
            if (_cameraHead != null)
            {
                this.GetComponent<Rigidbody>().AddForce(_cameraHead.transform.forward * velocity.magnitude);
                this.transform.Rotate(this.transform.up, device.velocity.magnitude * (isLeft ? 1 : -1));
            }
        }
    }
}
