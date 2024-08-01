using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPOVExtension : CinemachineExtension
{

    private InputManager iManager;
    private Vector3 startingRotation;// kameran�n ilk rotasyonu


    //rotasyon h�zlar�
    private float horizontalSpeed = 10f;
    private float verticalSpeed = 10f;
    [SerializeField]
    private float clampAngle = 80f;//�st alt rotasyon kilidi
    protected override void Awake()
    {
        iManager = InputManager.Instance;
        base.Awake();
        if (startingRotation == null) startingRotation = transform.localRotation.eulerAngles;
    }
    public void SensivityChanger(float sensX, float sensY)
    {
        horizontalSpeed = sensX;
        verticalSpeed = sensY;
    }
    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (vcam.Follow)
        {
            if (stage == CinemachineCore.Stage.Aim)
            {
                Vector2 deltaInput;
                try
                {
                    deltaInput = iManager.getCameraMovement();
                }
                catch
                {
                    return;
                }
                startingRotation.x += deltaInput.x * Time.deltaTime * verticalSpeed;
                startingRotation.y += deltaInput.y * Time.deltaTime * horizontalSpeed;
                startingRotation.y = Mathf.Clamp(startingRotation.y, -clampAngle, clampAngle);//kamera rotasyonunu dikey bi�imde k�s�tlama
                state.RawOrientation = Quaternion.Euler(-startingRotation.y, startingRotation.x, 0f);//camera rotasyonunu ayarlama

            }
        }
    }
}
