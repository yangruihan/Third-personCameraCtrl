using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // 单例
    public static CameraManager Singleton;

    #region Value

    public float MouseSpeedX = 3;
    public float MouseSpeedY = 3.5f;
    public float FollowSpeed = 9;
    public float TurnSmoothing = 0.1f;
    public float MinAngle = -35f;
    public float MaxAngle = 35f;

    #endregion

    public Transform TargetTf;
    
    private Transform _pivotTf;
    private Transform _camTf;

    private float _smoothX;
    private float _smoothXvelocity;
    private float _smoothY;
    private float _smoothYvelocity;
    private float _lookAngle;
    private float _tiltAngle;

    void Awake()
    {
        if (Singleton != null)
        {
            GameObject.Destroy(Singleton.gameObject);
        }

        Singleton = this;
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        Tick(Time.deltaTime);
    }

    public void Init()
    {
        _camTf = Camera.main.transform;
        _pivotTf = _camTf.parent;
    }

    public void Tick(float deltaTime)
    {
        float h = Input.GetAxis("Mouse X");
        float v = Input.GetAxis("Mouse Y");

        FollowTarget(deltaTime);
        HandleRotations(deltaTime, v, h, MouseSpeedX, MouseSpeedY);
    }

    private void FollowTarget(float deltaTime)
    {
        Vector3 newPos = Vector3.Lerp(transform.position, TargetTf.position, deltaTime * FollowSpeed);
        transform.position = newPos;
    }

    private void HandleRotations(float deltaTime, float vertical, float horizontal, float rotationSpeedX, float rotationSpeedY)
    {
        if (TurnSmoothing > 0f)
        {
            _smoothX = Mathf.SmoothDamp(_smoothX, horizontal, ref _smoothXvelocity, TurnSmoothing);
            _smoothY = Mathf.SmoothDamp(_smoothY, vertical, ref _smoothYvelocity, TurnSmoothing);
        }
        else
        {
            _smoothX = horizontal;
            _smoothY = vertical;
        }

        _lookAngle += _smoothX * rotationSpeedX;
        transform.localRotation = Quaternion.Euler(0, _lookAngle, 0);

        _tiltAngle -= _smoothY * rotationSpeedY;
        _tiltAngle = Mathf.Clamp(_tiltAngle, MinAngle, MaxAngle);
        _pivotTf.localRotation = Quaternion.Euler(_tiltAngle, 0, 0);
    }
}
