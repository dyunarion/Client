using UnityEngine; 
using System.Collections; 

public class PlayerMove : MonoBehaviour 
{ 
	public Transform marker; 
	public float moveSpeed = 10f; 
	public float turnSpeed = 540f; 
	public int blockLayer = 9; 
	public int moveLayer = 8; 
	
	// 캐릭터 정보. 
	public UserInfo playerInfo; 
	
	CharacterController _cc; 
	Animation _a; 
	Vector3 _dir; 
	float _est; 
	float _elapsedTime; 
	int _layerMask; 
	Vector3 targetPos; 
	
	void Awake() 
	{ 
		_cc = GetComponent<CharacterController>(); 
		_a = GetComponentInChildren<Animation>(); 
		_layerMask = (1 << blockLayer) + (1 << moveLayer); 
		_a.CrossFade("KK_Idle"); 
	} 
	
	void Update() 
	{ 
		// 사용자가 이동할 포지션 지정. 
		if (Input.GetMouseButtonDown(0)) 
		{ 
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
			RaycastHit hitInfo; 
			if (Physics.Raycast(ray, out hitInfo, 1000f, _layerMask)) 
			{ 
				// 마우스를 클릭한 유저가 본인인지 확인. 
				bool isMe = playerInfo.id == UserInfoManager.Instance.myInfo.id;
				if (hitInfo.transform.gameObject.layer == moveLayer && isMe) 
				{ 
					// 마우스가 클릭된 위치를 이용해서 MoveInfo 객체 생성. 
					MoveInfo moveInfo = new MoveInfo( 
					                                 UserInfoManager.Instance.myInfo.id, 
					                                 new NodeVector(hitInfo.point) 
					                                 ); 
					
					// 서버에 RequestMove 이벤트를 발생시켜서 이동 요청. 
					SocketManager.Instance.RequestMove(moveInfo); 
				} 
			} 
		} 
		
		// 캐릭터 이동. 
		if (_dir != Vector3.zero) 
		{ 
			_elapsedTime += Time.deltaTime; 
			
			// 계산한 이동시간이 경과했을 때 캐릭터 멈춤. 
			if (_elapsedTime > _est) 
			{ 
				// 변수 초기화. 
				_a.CrossFade("KK_Idle"); 
				_dir = Vector3.zero; 
				targetPos = Vector3.zero; 
				marker.gameObject.SetActive(false); 
				
				return; 
			} 
			
			// 이동 방향 계산. 
			_dir = (targetPos - transform.position).normalized; 
			// 한 프레임에 이동해야 하는 거리 계산. 
			float movePerFrame = moveSpeed * Time.deltaTime; 
			// 남은 거리 계산. 
			Vector3 diff = targetPos - transform.position; 
			diff.y = 0f; 
			float distance = diff.magnitude; 
			// 남은 거리가 한 프레임에 이동해야하는 거리보다 적은 경우 멈춤. 
			if (distance < movePerFrame) 
			{ 
				// 한 프레임 이동. 
				_cc.Move(_dir * movePerFrame + Vector3.up * Physics.gravity.y * Time.deltaTime);
				// 변수 초기화하고 멈춤. 
				_dir = Vector3.zero; 
				targetPos = Vector3.zero; 
				marker.gameObject.SetActive(false); 
				_a.CrossFade("KK_Idle"); 
			} 
			
			// 이동. 
			else 
			{ 
				_cc.Move(_dir * movePerFrame + Vector3.up * Physics.gravity.y * Time.deltaTime);
				RotateToDir(); 
			} 
		} 
	} 
	
	// 캐릭터 이동 설정. 
	public void SetMove(MoveInfo moveInfo, bool isMe) 
	{ 
		_a.CrossFade ("KK_Run_No"); 
		
		// 이동시켜야 할 캐릭터가 사용자 본인 캐릭터인 경우 marker 표시. 
		if (isMe) 
		{ 
			// marker 활성화. 
			marker.gameObject.SetActive(true); 
			// marker 위치 설정. 
			marker.position = moveInfo.targetPos.ToVector3(); 
		} 
		
		// 캐릭터가 이동해야할 위치 설정. 
		targetPos = moveInfo.targetPos.ToVector3(); 
		// 이동 방향 계산. 
		_dir = (targetPos - transform.position).normalized; 
		// 전체 이동 시간 계산. 
		_est = Vector3.Distance (targetPos, transform.position) / moveSpeed * 1.5f;
		// 이동 경과 시간을 계산하는 변수 초기화. 
		_elapsedTime = 0f; 
	} 
	
	// 이동해야하는 방향으로 캐릭터 회전. 
	void RotateToDir() 
	{ 
		Quaternion targetRot = Quaternion.LookRotation(_dir); 
		targetRot = Quaternion.Euler(targetRot.eulerAngles.y * Vector3.up); 
		transform.rotation = Quaternion.RotateTowards( 
		                                              transform.rotation, 
		                                              targetRot, 
		                                              turnSpeed * Time.deltaTime 
		                                              ); 
	} 
}
