using UnityEngine; 
using System.Collections; 
using System.Collections.Generic; 

public class UserInfoManager : MonoBehaviour 
{ 
	// 클래스를 싱글톤으로 구성. 
	private static UserInfoManager _instance; 
	public static UserInfoManager Instance 
	{ 
		get 
		{ 
			if (_instance == null) 
			{ 
				GameObject userInfoManager = new GameObject("UserInfoManager");
				_instance = userInfoManager.AddComponent<UserInfoManager>(); 
			} 
			
			return _instance; 
		} 
	} 
	
	// 접속 중인 사용자 정보를 관리하는 리스트. 
	public List<UserInfo> users; 
	// 사용자 본인의 정보. 
	public UserInfo myInfo; 
	
	void OnEnable() 
	{ 
		// 사용자 리스트 초기화. 
		users = new List<UserInfo> (); 
		// 씬이 변경되도 게임 오브젝트가 유지될 수 있도록 DontDestroyOnLoad를 호출. 
		DontDestroyOnLoad (this.gameObject); 
	} 
	
	// 서버에서 받은 사용자 정보 배열로 users 리스트 갱신. 
	public void IntializeUsers(UserInfo[] userInfos, string myID) 
	{ 
		for (int ix = 0; ix < userInfos.Length; ++ix) 
		{ 
			// 사용자 정보 추가. 
			users.Add(userInfos[ix]); 
			
			// 내 정보인 경우 myInfo 정보 갱신. 
			if (userInfos[ix].id == myID) 
			{ 
				myInfo = new UserInfo(userInfos[ix].id, userInfos[ix].nickName);
			} 
		} 
	} 
	
	// 새로운 사용자가 접속했을 때 해당 사용자 정보를 users 리스트에 추가. 
	public void AddUser(UserInfo newUser) 
	{ 
		users.Add (newUser); 
		
		GameManager.Instance.SpawnPlayer(newUser);
	} 
	
	// 사용자가 접속해제를 했을 때 해당 사용자 정보를 users 리스트에서 삭제. 
	public void DeleteUser(UserInfo outUser) 
	{ 
		for (int ix = 0; ix < users.Count; ++ix) 
		{ 
			if (users[ix].id == outUser.id) 
			{ 
				users.RemoveAt(ix); 
				break; 
			} 
		} 
		
		GameManager.Instance.DeletePlayer(outUser);
	} 
}
