using UnityEngine;
using System.Collections;

public class TalkBubble : MonoBehaviour
{
	public PlayerMove player;
	public UILabel talkLabel;
	public UILabel nickNameLabel;
	public UISlicedSprite bgSprite;
	public Camera guiCam;
	public Camera gameCam;
	public float xAdjust = 10f;
	public float yAdjust = 0f;
	
	public bool isFollow = false;
	
	// 설정된 플레이어 Follow.
	void Update ()
	{
		if (!isFollow)
			return;
		
		Vector3 screenPos = gameCam.WorldToScreenPoint(player.transform.position);
		screenPos += Vector3.up * yAdjust + Vector3.right * xAdjust;
		screenPos.z = 0f;
		transform.position = guiCam.ScreenToWorldPoint(screenPos);
	}
	
	// 말풍선 변수 설정.
	public void SetTarget(PlayerMove playerInfo)
	{
		player = playerInfo;
		nickNameLabel.text = playerInfo.playerInfo.nickName;
		guiCam = GameManager.Instance.guiCam;
		gameCam = GameManager.Instance.playerCam.GetComponentInChildren<Camera> ();
		
		isFollow = true;
	}
	
	// 말풍선 정보 갱신.
	public void UpdateTalk(string msg)
	{
		talkLabel.text = msg;
	}
}