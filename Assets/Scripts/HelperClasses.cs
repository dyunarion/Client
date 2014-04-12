using UnityEngine;

// 로그인 정보를 전달할 때 사용합니다.  
[System.Serializable] 
public class LoginInfo 
{ 
	public string id; 
	public string pw; 
	
	public LoginInfo() { } 
	public LoginInfo(string id, string pw) 
	{ 
		this.id = id; 
		this.pw = pw; 
	} 
} 

// 등록 정보를 전달할 때 사용합니다.  
[System.Serializable] 
public class RegisterInfo 
{ 
	public string id; 
	public string pw; 
	public string nickName; 
	
	public RegisterInfo() { } 
	public RegisterInfo(string id, string pw, string nickName) 
	{ 
		this.id = id; 
		this.pw = pw; 
		this.nickName = nickName; 
	} 
} 

// 사용자 정보를 전달하고 받을 때 사용합니다.  
[System.Serializable] 
public class UserInfo 
{ 
	public string id; 
	public string nickName; 
	public NodeVector currentPos;
	
	public UserInfo() { } 
	public UserInfo(string id, string nickName, NodeVector currentPos = null) 
	{ 
		this.id = id; 
		this.nickName = nickName; 
		if(currentPos == null){
		 currentPos = new NodeVector(Vector3.zero);
		}
		else{
		this.currentPos = currentPos;
		}
	} 
} 

// 서버와 Vector 를 전송할 때 사용합니다. 
[System.Serializable] 
public class NodeVector 
{ 
	public float x; 
	public float y; 
	public float z; 
	
	public NodeVector() { } 
	public NodeVector(Vector3 vector) 
	{ 
		this.x = vector.x; 
		this.y = vector.y; 
		this.z = vector.z; 
	} 
	
	public Vector3 ToVector3() 
	{ 
		return new Vector3 (x, y, z); 
	} 
}



// POST 응답 메시지를 파싱할 때 사용합니다.  
[System.Serializable] 
public class PostMessage 
{ 
	public string msg; 
	public UserInfo[] data; 
} 

// 사용자 정보를 수신할 때 사용합니다.  
[System.Serializable] 
public class UserInfoData 
{ 
	public string name; 
	public UserInfo[] args; 
} 

// 채팅 메시지를 전송할 때 사용합니다.  
[System.Serializable] 
public class MessageInfo 
{ 
	public string id; 
	public string msg; 
	
	public MessageInfo() { } 
	public MessageInfo(string id, string msg) 
	{ 
		this.id = id; 
		this.msg = msg; 
	} 
} 

// 채팅 메시지를 수신할 때 사용합니다.  
[System.Serializable] 
public class MessageInfoData 
{ 
	public string name; 
	public MessageInfo[] args; 
}

// 이동에 관한 데이터를 주고 받을 때 사용합니다. 
[System.Serializable] 
public class MoveInfo 
{ 
	public string id; 
	public NodeVector targetPos; 
	
	public MoveInfo() { } 
	public MoveInfo(string id, NodeVector target) 
	{ 
		this.id = id; 
		this.targetPos = target; 
	} 
} 

// 서버에서 보낸 MoveInfo를 파싱할 때 사용합니다. 
[System.Serializable] 
public class MoveInfoData 
{ 
	public string name; 
	public MoveInfo[] args; 
}
