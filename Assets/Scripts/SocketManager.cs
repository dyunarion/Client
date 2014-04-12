using UnityEngine; 
using System.Collections; 

[RequireComponent(typeof(SocketIOClient))] 
public class SocketManager : MonoBehaviour 
{ 
	// 매니져 클래스 구성.  
	private static SocketManager _instance; 
	public static SocketManager Instance { get { return _instance; } } 
	
	public ChatBox chatBox; 
	
	// SocketIOClient 객체.  
	SocketIOClient client; 
	// 소켓 연결이 완료되었는지 확인하는 데 사용.  
	bool isReady = false; 
	
	void Awake() 
	{ 
		_instance = this; 
		
		// client 객체 초기화.  
		client = GetComponent<SocketIOClient>(); 
		// client 객체에 이벤트 설정.  
		client.On("EnterConfirm", EnterConfirm); 
		client.On("NewUser", NewUser); 
		client.On("NewMessage", NewMessage); 
		client.On("UserLeft", UserLeft); 
		client.On("Move", Move);
		// ChatBox 컴포넌트 찾아서 초기화.
		if (chatBox == null)
			chatBox = FindObjectOfType(typeof(ChatBox)) as ChatBox;
	} 
	
	void Update() 
	{ 
		if (isReady) 
			return; 
		
		// 소켓 연결이 완료되면 "Enter" 이벤트를 발생시켜서 입장 알림.  
		if (client.Ready) 
		{ 
			isReady = true; 
			Enter(); 
		} 
	} 
	
	// 사용자가 접속해서 서버에 Enter 이벤트를 발생시킬 때 사용.  
	void Enter() 
	{ 
		// 서버에 본인 정보 전송.  
		client.Emit("Enter", UserInfoManager.Instance.myInfo); 
	} 
	
	// 사용자가 접속을 해제하고 서버에 Leave 이벤트를 발생시킬 때 사용.  
	public void Leave() 
	{ 
		// 서버에 본인 정보 전송.  
		client.Emit("Leave", UserInfoManager.Instance.myInfo); 
	} 
	
	// 채팅 메시지를 입력할 때 사용.   
	public void RequestMessage(string msg) 
	{ 
		// 채팅 메시지를 입력한 사용자의 id와 채팅 메시지를 이용해서 MessageInfo 객체 생성.  
		MessageInfo msgInfo = new MessageInfo(UserInfoManager.Instance.myInfo.id, msg);
		
		// 서버에 메시지 정보 전송.  
		client.Emit("RequestMessage", msgInfo); 
	} 
	
	void Move(SocketIOClient socket, IMessage msg)
	{
		Debug.Log("Move");
		
	
		MoveInfoData moveInfoData = JsonFx.Json.JsonReader.Deserialize<MoveInfoData>(msg.data);
		MoveInfo moveInfo = new MoveInfo( moveInfoData.args[0].id, moveInfoData.args[0].targetPos);
		
		GameManager.Instance.PlayerMove(moveInfo);
	}
	
	// 서버에 캐릭터 이동 요청. 
	public void RequestMove(MoveInfo moveInfo) 
	{ 
		// 서버에 이동 데이터 전송. 
		client.Emit ("RequestMove", moveInfo); 
	}
	
	// 서버에서 접속 승인 메시지 응답이 있을 때 호출.  
	void EnterConfirm(SocketIOClient socket, IMessage msg) 
	{ 
		Debug.Log("Enter Confirm");
		GameManager.Instance.InitializePlayers();
	} 
	
	// 새 사용자가 접속했을 때 호출.  
	void NewUser(SocketIOClient socket, IMessage msg) 
	{ 
		// 서버에서 받은 JSON 데이터를 UserInfoData 객체로 파싱.  
		UserInfoData userInfo = JsonFx.Json.JsonReader.Deserialize<UserInfoData>(msg.data); 
		// 사용자 추가.  
		UserInfoManager.Instance.AddUser(userInfo.args[0]); 
	} 
	
	// 서버에서 NewMessage 이벤트를 발생시켰을 때 호출.  
	void NewMessage(SocketIOClient socket, IMessage msg) 
	{ 
		// 서버에서 받은 JSON 데이터를 MessageInfoData 객체로 파싱.  
		MessageInfoData msgInfoData = JsonFx.Json.JsonReader.Deserialize<MessageInfoData>(msg.data); 
		// MessageInfoData 객체를 이용해서 MessageInfo 객체 생성.  
		MessageInfo msgInfo = new MessageInfo(msgInfoData.args[0].id, msgInfoData.args[0].msg);
		
		// 서버에서 받은 채팅 메시지를 ChatBox에 전달.  
		chatBox.UpdateTextList(msgInfo); 
	} 
	
	// 서버에서 UserLeft 이벤트를 발생시켰을 때 호출.  
	void UserLeft(SocketIOClient socket, IMessage msg) 
	{ 
		// 서버에서 받은 JSON 데이터를 UserInfoData 객체로 파싱.  
		UserInfoData userInfo = JsonFx.Json.JsonReader.Deserialize<UserInfoData>(msg.data); 
		// 사용자 삭제.  
		UserInfoManager.Instance.DeleteUser(userInfo.args[0]); 
	} 
}