using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
	void Update ()
    {
        UpdateController(GameObject.Find("Controller (left)"));
        UpdateController(GameObject.Find("Controller (right)"));
    }

    private void UpdateController(GameObject controller)
    {
        if (controller == null)
        {
            return;
        }

        int index = (int)controller.GetComponent<SteamVR_TrackedObject>().index;
        SteamVR_Controller.Device device = SteamVR_Controller.Input(index);
        if (device.GetPress(Valve.VR.EVRButtonId.k_EButton_ApplicationMenu))
        {
            SceneManager.LoadScene("HubWorld");
        }
    }
}
