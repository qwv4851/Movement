using UnityEngine;
using System.Collections;

public class HallwayController : MonoBehaviour {
    public float movementSpeed = 6.0f;
    public float distanceThreshold = 1.0f;

    private float _startAngle;
    private Transform _headTransform;
    private Quaternion _startRotation;
    private Vector3 _prevPos;

	void Start () {
        _headTransform = this.transform.FindChild("Camera (head)");
        _startAngle = CalculateWorldAngle(_headTransform.localPosition, this.transform.position);
        _startRotation = this.transform.rotation;
        _prevPos = _headTransform.position;
    }
	
	void Update  () {
        // Calculate the distance traveled since the previous frame, ignoring vertical distance
        Vector3 mask = new Vector3(1, 0, 1);    
        float distance = Vector3.Distance(Vector3.Scale(_headTransform.localPosition, mask), Vector3.Scale(_prevPos, mask));

        // Use a distance threshold in order to ignore the movement jump during tracking initialization as well as other HMD noise
        if (distance < distanceThreshold)
        {
            this.transform.position += distance * Vector3.forward * movementSpeed;
            _prevPos = _headTransform.localPosition;
        }

        // Rotate the world according to the player's position
        float angle = CalculateWorldAngle(_headTransform.localPosition, this.transform.position);
        this.transform.rotation = Quaternion.AngleAxis(angle - _startAngle, this.transform.up) * Quaternion.Inverse(_startRotation);
	}

    // Calculates a clockwise angle of the player relative to the center of the play area bounds
    float CalculateWorldAngle(Vector3 position, Vector3 cameraRigPos)
    {
        return Mathf.Atan2(-position.x, position.z) * Mathf.Rad2Deg - 90.0f;
    }
}
