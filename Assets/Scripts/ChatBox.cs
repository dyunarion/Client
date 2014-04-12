using UnityEngine; 
using System.Collections; 

public class ChatBox : MonoBehaviour 
{ 
	// 전체 채팅 메시지를 보여중 채팅 창.  
	public UITextList textList; 
	// 사용자의 채팅 메시지를 입력 받을 UIInput.  
	public UIInput mInput; 
	
	bool mIgnoreNextEnter = false; 
	// 보낼 메시지.  
	string msgToSend = ""; 
	
	// mInput에 메시지가 입력되면 호출.  
	void OnSubmit() 
	{ 
		// mInput의 채팅 메시지 문자열을 msgToSend에 저장.  
		msgToSend = mInput.text; 
		// mInput 초기화.  
		mInput.text = ""; 
		
		// 서버에 채팅 메시지 전달 요청.  
		if (msgToSend != "") 
			SocketManager.Instance.RequestMessage(msgToSend); 
		
		// mInput 포커스 설정.  
		mInput.selected = true; 
		mIgnoreNextEnter = true; 
	} 
	
	// 서버에서 채팅 메시지 갱신 응답이 있는 경우 호출.  
	public void UpdateTextList(MessageInfo msgInfo) 
	{ 
		// 전달 받은 msgInfo에서 id 와 메시지를 조합.  
		string newMsg = msgInfo.id + " : " + msgInfo.msg; 
		
		// id와 메시지를 조합한 문자열을 채팅 창에 추가.  
		if (textList != null) 
		{ 
			if (!string.IsNullOrEmpty(newMsg)) 
			{ 
				textList.Add(newMsg); 
				msgToSend = ""; 
			} 
		} 
		
		// mInput 포커스 설정.  
		mInput.selected = true; 
		mIgnoreNextEnter = true; 
	} 
} 
