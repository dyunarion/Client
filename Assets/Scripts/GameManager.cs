using UnityEngine; 
using System.Collections; 
using System.Collections.Generic; 

public class GameManager : MonoBehaviour 
{ 
	// 매니져 클래스를 싱글톤으로 구성. 
	private static GameManager _instance; 
	public static GameManager Instance { get { return _instance; } } 
	
	// 플레이어 프리팹. 
	public GameObject playerPrefab; 
	// 플레이어를 따라다니는 카메라. 
	public CameraRig1 playerCam; 
	// GUI 카메라. 
	public Camera guiCam; 
	// 이동 표시 마커. 
	public Transform marker; 
	// 플레이어 캐릭터 리스트. 
	public List<PlayerMove> players; 
	
	void Awake() 
	{ 
		_instance = this; 
		players = new List<PlayerMove> (); 
	} 
	
	// 전체 플레이어 캐릭터 생성. 
	public void InitializePlayers() 
	{ 
		// 현재 접속 중인 플레이어 생성. 
		foreach (UserInfo user in UserInfoManager.Instance.users) 
		{ 
			SpawnPlayer(user); 
		} 
	} 
	
	// 플레이어 캐릭터 생성할 때 호출. 
	public void SpawnPlayer(UserInfo playerInfo) 
	{ 
		// 플레이어 게임 오브젝트 생성. 
		GameObject newPlayer = (GameObject)Instantiate (playerPrefab, playerInfo.currentPos.ToVector3(), playerPrefab.transform.rotation); 
		
		// PlayerMove 컴포넌트 설정. 
		PlayerMove player = newPlayer.GetComponent<PlayerMove> (); 
		player.playerInfo = playerInfo; 
		player.marker = marker; 
		
		// 사용자 본인 캐릭터인지 확인. 
		bool isMe = playerInfo.id == UserInfoManager.Instance.myInfo.id; 
		if (isMe) 
		{ 
			// 사용자 본인 캐릭터인 경우 CameraRig 설정. 
			playerCam.SetTarget (newPlayer.transform); 
			// AudioListener 컴포넌트 추가. 
			newPlayer.AddComponent<AudioListener>(); 
		} 
		
		// 생성한 플레이어를 리스트에 추가. 
		players.Add (player); 
	} 
	
	// 플레이어 캐릭터 삭제. 
	public void DeletePlayer(UserInfo playerInfo) 
	{ 
		// id 가 일치하는 플레이어를 찾아서 삭제. 
		for (int ix = 0; ix < players.Count; ++ix) 
		{ 
			if (playerInfo.id == players[ix].playerInfo.id) 
			{ 
				Destroy (players[ix].gameObject); 
				players.RemoveAt(ix); 
				
				break; 
			} 
		} 
	} 
	
	public void PlayerMove(MoveInfo moveInfo)
	{
		Debug.Log("PlayerMove");
	
		// id가 일치하는 플레이어를 찾아서 이동 명령.
		foreach(PlayerMove player in players)
		{
			if(player.playerInfo.id == moveInfo.id)
			{
				bool isMe = moveInfo.id == UserInfoManager.Instance.myInfo.id;
				player.SetMove (moveInfo, isMe);
				
				break;
			}
		}
	}
}

