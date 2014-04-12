using UnityEngine;
using System.Collections;

public class CameraRig : MonoBehaviour {

	Transform cam;
	public Transform target;
	public float lerpSpeed = 2f;

	float _yDistance;

	void Start () {
		if(cam == null)
			cam = Camera.main.transform;
		_yDistance = transform.position.y - target.position.y;
	}
	
	void Update () {

		transform.position = Vector3.Lerp (
			transform.position,
			target.position + Vector3.up * _yDistance,
			Time.deltaTime * lerpSpeed);
	
	}

	void OnDrawGizmos() {

		if(cam != null)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine (transform.position, cam.position);
		}
	}
}
