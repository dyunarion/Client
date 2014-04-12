using UnityEngine;
using System.Collections;

public class AnimRecycler : MonoBehaviour {

	Animation _a;

	void Awake()
	{
		_a = animation;
		_a["point_1"].wrapMode = WrapMode.Loop;
		_a["point_1"].speed = 0.5f;
	}

	void OnEnable()
	{
		_a.Play();
	}

}
