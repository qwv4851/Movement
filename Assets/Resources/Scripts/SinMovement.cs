using UnityEngine;

public class SinMovement : MonoBehaviour
{
    public Vector3 direction;
    public float speed;

    private Vector3 _startPos;

	void Start ()
    {
        _startPos = transform.position;
	}
	
	void FixedUpdate ()
    {
        transform.position = _startPos + Mathf.Sin(Time.timeSinceLevelLoad * speed) * direction;
	}
}
