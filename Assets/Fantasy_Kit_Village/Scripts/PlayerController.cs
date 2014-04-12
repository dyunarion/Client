using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public Transform marker;
    public float moveSpeed = 10f;
    public float turnSpeed = 540f;
    public int blockLayer = 9;
    public int moveLayer = 8;

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

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, 1000f, _layerMask))
            {
                if (hitInfo.transform.gameObject.layer == moveLayer)
                {
                    _a.CrossFade("KK_Run_No");

                    marker.gameObject.SetActive(true);
					targetPos = marker.position = hitInfo.point;
                    _dir = (hitInfo.point - transform.position).normalized;
					_est = Vector3.Distance(targetPos, transform.position) / moveSpeed * 1.5f;
                    _elapsedTime = 0f;
                }
            }
        }

        if (_dir != Vector3.zero)
        {
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime > _est)
            {
                _a.CrossFade("KK_Idle");
                _dir = Vector3.zero;
				targetPos = Vector3.zero;
                marker.gameObject.SetActive(false);

                return;
            }

            _dir = (targetPos - transform.position).normalized;
            float movePerFrame = moveSpeed * Time.deltaTime;
			Vector3 diff = targetPos - transform.position;
			diff.y = 0f;
			float distance = diff.magnitude;

            if (distance < movePerFrame)
            {
				_cc.Move(_dir * movePerFrame + Vector3.up * Physics.gravity.y * Time.deltaTime);
                _dir = Vector3.zero;
				targetPos = Vector3.zero;
                marker.gameObject.SetActive(false);
                _a.CrossFade("KK_Idle");
            }
            else
            {
                _cc.Move(_dir * movePerFrame + Vector3.up * Physics.gravity.y * Time.deltaTime);
                RotateToDir();
            }

        }

    }

    void RotateToDir()
    {
        Quaternion targetRot = Quaternion.LookRotation(_dir);
        targetRot = Quaternion.Euler(targetRot.eulerAngles.y * Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
    }
}