using UnityEngine; 
using System.Collections; 

public class Register : MonoBehaviour 
{ 
	// 서버 url.  
	public string url; 
	// 서버 port.  
	//public string port; 
	// 아이디를 입력받을 NGUI UIInput.   
	public UIInput idInput; 
	// 암호를 입력받을 NGUI UIInput.  
	public UIInput pwInput; 
	// 닉네임을 입력받을 UIInput.  
	public UIInput nickNameInput; 
	// 오류메시지를 출력하는 label.  
	public UILabel errorLabel; 
	
	// 등록 요청 함수.  
	IEnumerator RequestRegister() 
	{ 
		// id 입력 UIInput에 입력된 문자열 추출.  
		string id = idInput.text; 
		// pw 입력 UIInput에 입력된 문자열 추출.  
		string pw = pwInput.text; 
		// 닉네임 입력 UIInput에 입력된 문자열 추출.  
		string nickName = nickNameInput.text; 
		
		// 서버에 전달할 데이터 형태인 RegisterInfo 객체 생성.  
		RegisterInfo register = new RegisterInfo(id, pw, nickName); 
		// 생성한 register 객체를 JSON 문자열로 변경.  
		string registerInfo = JsonFx.Json.JsonWriter.Serialize(register); 
		
		// POST 요청을 할 Form 클래스 생성.  
		WWWForm form = new WWWForm(); 
		// "RegisterInfo"라는 키 값에 위에서 변경한 registerInfo json 문자열 추가.  
		form.AddField("RegisterInfo", registerInfo); 
		
		// 서버에 등록 요청.  
		WWW www = new WWW(url, form); 
		yield return www; 
		
		// 서버에서 받은 결과 메시지.  
		string result = www.text; 
		
		// 결과 메시지가 "Register Complete"이면 등록 완료. Login 씬으로 이동.  
		if (result == "Register Complete") 
		{ 
			Application.LoadLevel("Login"); 
		} 
		
		// 결과 메시지가 "Register Complter"가 아닌경우 오류 메시지 출력.  
		else 
		{ 
			errorLabel.text = result; 
		} 
	} 
	
	// Cancel버튼에 연결 될 함수  
	void CancelButtonListener() 
	{ 
		Application.LoadLevel("Login"); 
	} 
} 
