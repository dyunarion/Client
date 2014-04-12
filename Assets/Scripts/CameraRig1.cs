using UnityEngine; 
using System.Collections; 

public class CameraRig1 : MonoBehaviour 
{ 
	Transform cam; 
	public Transform target; 
	public float lerpSpeed = 2f; 
	
	float _yDistance; 
	bool isStarted = false; 
	
	void Start () 
	{ 
		if(cam == null) 
			cam = Camera.main.transform; 
	} 
	
	// 매 프레임마다 캐릭터의 위치를 확인하고 따라 다닙니다. 
	void Update () 
	{ 
		if (!isStarted) 
			return; 
		
		transform.position = Vector3.Lerp ( 
		                                   transform.position, 
		                                   target.position + Vector3.up * _yDistance, 
		                                   Time.deltaTime * lerpSpeed 
		                                   ); 
	} 
	
	// 따라다닐 캐릭터를 설정합니다. 
	public void SetTarget(Transform target) 
	{ 
		this.target = target; 
		_yDistance = transform.position.y - target.position.y; 
		isStarted = true; 
	} 
	
	void OnDrawGizmos() 
	{ 
		if(cam != null) 
		{ 
			Gizmos.color = Color.yellow; 
			Gizmos.DrawLine (transform.position, cam.position); 
		} 
	} 
}
