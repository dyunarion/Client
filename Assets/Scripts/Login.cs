using UnityEngine; 
using System.Collections; 

public class Login : MonoBehaviour 
{ 
	// 서버 url.  
	public string url; 
	// 서버 port.  
	//public string port; 
	// 아이디를 입력받을 NGUI UIInput.  
	public UIInput idInput; 
	// 암호를 입력받을 NGUI UIInput.  
	public UIInput pwInput; 
	// 오류메시지를 출력하는 label.  
	public UILabel errorLabel; 

	void Start()
	{
		if (UserInfoManager.Instance == null) { }
	}
	
	// 로그인 요청 함수.  
	IEnumerator RequestLogin() 
	{ 
		// id 입력 UIInput에 입력된 문자열 추출.  
		string id = idInput.text; 
		// pw 입력 UIInput에 입력된 문자열 추출.  
		string pw = pwInput.text; 
		
		// 서버에 전달할 데이터 형태인 LoginInfo 객체 생성.  
		LoginInfo login = new LoginInfo(id, pw); 
		// 생성한 login 객체를 JSON 문자열로 변경.  
		string loginInfo = JsonFx.Json.JsonWriter.Serialize(login); 
		
		// POST 요청을 할 Form 클래스 생성.  
		WWWForm form = new WWWForm(); 
		// "LoginInfo"라는 키 값에 위에서 변경한 registerInfo json 문자열 추가.  
		form.AddField("LoginInfo", loginInfo); 
		
		// 서버에 로그인 요청.  
		WWW www = new WWW(url, form); 
		yield return www; 
		
		// 서버에서 받은 결과 메시지 파싱.  
		PostMessage result = JsonFx.Json.JsonReader.Deserialize<PostMessage>(www.text); 
		
		// 결과 메시지가 "Login Complete"이면 로그인 완료.  
		if (result.msg == "Login Complete") 
		{ 
			Debug.Log(result.msg); 
			errorLabel.text = result.msg; 

			// UserInfoManager의 users 리스트에 정보 추가. 
			UserInfoManager.Instance.IntializeUsers(result.data, login.id); 
			// 메인씬 로드. 
			Application.LoadLevel("Main"); 
		} 
		
		// 결과 메시지가 "Login Complete"가 아닌경우 오류 메시지 출력.  
		else 
		{ 
			errorLabel.text = result.msg; 
		} 
	} 
	
	// Register 버튼에 연결될 함수.  
	void RegisterButtonListener() 
	{ 
		Application.LoadLevel("Register"); 
	} 
} 
