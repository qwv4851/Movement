using UnityEngine;
using UnityEngine.UI;

public class WorldRotator : MonoBehaviour
{
    private Quaternion _startCameraRot;
    private Vector3 _startCameraPos;
    private Quaternion _startHead;
    private Quaternion _prevHead;
    private GameObject _cameraRig;
    private GameObject _canvas;
    private bool _isNearWall;
    private bool _wasNearWall;
    private GameObject _meter;
    private int _timeScale = 100;
    private float _deltaY;
    private float _deltaYSum;
    private float _deltaYFrame;
    private bool _initialized;
    private bool _turnLeft;

    void Start ()
    {
        _cameraRig = GameObject.Find("[CameraRig]");
        _canvas = GameObject.Find("Canvas");
        _meter = GameObject.Find("Meter");
    }
	
	void Update ()
    {
        UpdateMovement();
        UpdateMeter();
        UpdateCanvas();
	}

    void FixedUpdate()
    {
        // Slow down time and music pitch when near a wall
        if ( _isNearWall && _timeScale > 10)
        {
            _timeScale--;
        }
        else if (!_isNearWall && _timeScale < 100)
        {
            _timeScale++;
        }

        UnityEngine.Time.timeScale = _timeScale;
        GameObject cameraHead = GameObject.Find("Camera (head)");
        cameraHead.GetComponent<AudioSource>().pitch = _timeScale / 200.0f + 0.5f;
    }


    private void UpdateMovement()
    {        
        // Save the player's starting position and rotation
        if (!_wasNearWall && _isNearWall)
        {
            _startCameraPos = _cameraRig.transform.position;
            _startCameraRot = _cameraRig.transform.rotation;
            _startHead = this.transform.localRotation;
        }

        _wasNearWall = _isNearWall;

        if (_isNearWall)
        {
            // Rotate the world to match the rotation of the player's head
            _deltaY = (this.transform.localRotation * Quaternion.Inverse(_startHead)).eulerAngles.y;
            _cameraRig.transform.position = _startCameraPos;
            _cameraRig.transform.rotation = _startCameraRot;
            _cameraRig.transform.RotateAround(this.transform.position, Vector3.up, -_deltaY);
        }

        if (!_initialized && _isNearWall)
        {
            _prevHead = this.transform.rotation;
            _initialized = true;
        }

        if (_initialized)
        {
            // Record the sum of the player's rotation. Use this to determine whether or not they should turn left or right to avoid getting tangled.
            _deltaYFrame = (this.transform.localRotation * Quaternion.Inverse(_prevHead)).eulerAngles.y;
            _prevHead = this.transform.localRotation;
            if (_deltaYFrame > 180.0f)
            {
                _deltaYFrame -= 360.0f;
            }
            _deltaYSum += _deltaYFrame;
        }
    }

    private void UpdateMeter()
    {
        RaycastHit hitInfo;
        Vector3 projection = Vector3.ProjectOnPlane(this.transform.forward, Vector3.up);

        // Raycast onto the grid walls
        if (Physics.Raycast(new Ray(this.transform.position, projection), out hitInfo, float.PositiveInfinity, LayerMask.NameToLayer("GridWall")))
        {
            // Calculate the player's current distance relative to the maximum wall to wall distance
            float percentToWall = hitInfo.distance / 3.0f;
            int deviceIndex = (int)this.GetComponent<SteamVR_TrackedObject>().index;
            float angVel = SteamVR_Controller.Input(deviceIndex).angularVelocity.magnitude;
            float vel = SteamVR_Controller.Input(deviceIndex).velocity.magnitude;
            
            // Activate the overlay if we are near a wall. Deactivate if we are are no longer facing the wall and are no longer turning or standing still.
            if (!_isNearWall && percentToWall < 0.15f)
            {
                _isNearWall = true;
                _turnLeft = _deltaYSum > 0;
            }
            else if (_isNearWall && percentToWall > 0.18f && (angVel < 0.1f || (vel - angVel) > -0.20f))
            {
                _isNearWall = false;
            }
            
            // Update the meter graphics
            if (_meter != null)
            {
                float offset = 100.0f * percentToWall;
                _meter.GetComponent<RectTransform>().sizeDelta = new Vector2(offset, 10.0f);
                _meter.GetComponent<RectTransform>().anchoredPosition = new Vector3(offset / 2.0f, 0, 0);
                _meter.GetComponent<Image>().color = _isNearWall && percentToWall < 0.5f ?  Color.red : Color.cyan;
            }
        }
    }

    private void UpdateCanvas()
    {
        // Position the HUD in front of the camera. There seems to be a bug where making the canvas a child of the camera causes rendering issues.
        _canvas.transform.position = this.transform.position + this.transform.forward * 0.2f + this.transform.up * -0.1f;
        _canvas.transform.rotation = this.transform.rotation;
        _canvas.SetActive(_isNearWall);
       
        // Flip the HUD when we want to turn left
        if (_turnLeft)
        {
            _canvas.transform.Rotate(0, 0, 180, Space.Self);
        }
    }
}
