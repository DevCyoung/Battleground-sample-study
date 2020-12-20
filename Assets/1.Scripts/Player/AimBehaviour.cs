using FC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>

/// 마우스 오른쪽 버튼 조준 , 다른 동작을 대체해서 동작하게 됩니다. 
/// 다른동작보다 먼저행동합니다.

/// 마우스 오른쪽 버튼 조준 , 다른 동작을 대체해서 동작하게 됩니다.

/// 마우스 휠버튼으로 좌우 카메라 변경
/// 벽의 모서리에서 조준할때 상체를 살짝 기울여주는 기능.
/// </summary>
public class AimBehaviour : GenericBehaviour
{

    public Texture2D crossHair; // 십자선 이미지
    private Transform myTransform;

    private int aimBool; // 애니메이터 페러메터 조준관련
    private int cornerBool; // 애니메이터 관련 코너 
    
    public float aimTurnSmoothing = 0.15f; // 카메라를 향하도록 조준할때 회전속도
    public float anumTurnSmoothing = 0.15f; // 카메라를 향하도록 조준할때 회전속도

    private bool isAim;
    private bool peekCorner; // 플레이어가 코너 모서리에있는가

    public Vector3 aimPivotoffset = new Vector3(0.5f, 1.2f, 0.0f);
    public Vector3 aimCamoffset = new Vector3(0.0f, 0.4f, -0.7f);

    private Vector3 initialRootRotation; // 루트 본 로컬 회전값
    private Vector3 initialHipRotation;
    private Vector3 initialSpineRotation;

    private void Start()
    {
        //setup
        aimBool = Animator.StringToHash(AnimatorKey.Aim);
        cornerBool = Animator.StringToHash(AnimatorKey.Corner);
        myTransform = transform;


        Transform hips = behaviourController.GetAnimator.GetBoneTransform(HumanBodyBones.Hips);
        initialRootRotation = (hips.parent == myTransform) ? Vector3.zero : hips.parent.localEulerAngles;
        initialHipRotation = hips.localEulerAngles;
        initialSpineRotation = behaviourController.GetAnimator.GetBoneTransform(HumanBodyBones.Spine).localEulerAngles;

    }

    //카메라에 따라 플레이어를 올바른 방향으로 회전합니다.

    private void Rotating()
    {

        Vector3 forward = behaviourController.playerCamera.TransformDirection(Vector3.forward);
        forward.y = 0.0f;
        forward = forward.normalized;

        Quaternion targetRotation = Quaternion.Euler(0f, behaviourController.GetCameraScript.GetH, 0.0f);

        float minSpeed = Quaternion.Angle(myTransform.rotation, targetRotation) * aimTurnSmoothing;

        if (peekCorner)
        {
            //조준 중일때 플레이어 상체만 살짝 기울입니다.
            myTransform.rotation = Quaternion.LookRotation(-behaviourController.GetLastDirection());
            targetRotation *= Quaternion.Euler(initialRootRotation);
            targetRotation *= Quaternion.Euler(initialHipRotation);
            targetRotation *= Quaternion.Euler(initialSpineRotation);
            Transform spine = behaviourController.GetAnimator.GetBoneTransform(HumanBodyBones.Spine);
            spine.rotation = targetRotation;
        }
        else
        {
            behaviourController.SetLastDirection(forward);
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, targetRotation, minSpeed * Time.deltaTime);

        }

    }

    //조준중일때를 관리하는 함수.
    private void AimManagerment()
    {
        Rotating();
    }
    private IEnumerator ToggleAimOn() // UI 트리거에선 Toggle이란 말을 많이씁니다.
    {
        yield return new WaitForSeconds(0.05f);
        //조준이 불가능한 상태일때에 대한 예외처리
        if(behaviourController.GetTempLockStatus(this.behaviourCode) || behaviourController.
            IsOverriding(this))
        {
            isAim = true;
            int signal = 1;
            if (peekCorner)
            {
                signal = (int)Mathf.Sign(behaviourController.GetH);
             
            }
            aimCamoffset.x = Mathf.Abs(aimCamoffset.x ) * signal;
            aimPivotoffset.x = Mathf.Abs(aimPivotoffset.x) * signal;
            yield return new WaitForSeconds(0.1f);
            behaviourController.GetAnimator.SetFloat(speedFloat, 0.0f);
            behaviourController.OverrideWithBehaviour(this);
        }


    }

    private IEnumerator ToggleAimOff()
    {
        isAim = false;
        yield return new WaitForSeconds(0.3f);
        behaviourController.GetCameraScript.ResetTargetOffsets();
        behaviourController.GetCameraScript.ResetMaxVerticalAngle();
        yield return new WaitForSeconds(0.1f);
        behaviourController.RevokeOverridingBehaviour(this);


    }

    public override void LocalFixedUpdate()
    {
        if(isAim)
        {
            behaviourController.GetCameraScript.SetTargetOffset(aimPivotoffset, aimCamoffset);

        }
    }

    public override void LocalLateUpdate()
    {
        AimManagerment();
    }

    private void Update()
    {
        peekCorner = behaviourController.GetAnimator.GetBool(cornerBool);
        if(Input.GetAxisRaw(ButtonName.Aim) != 0 && !isAim)
        {
            StartCoroutine(ToggleAimOn());


        }
        else if( isAim && Input.GetAxisRaw(ButtonName.Aim) == 0)
        {
            StartCoroutine(ToggleAimOff());
        }
        //조준중일때는 달리기를 하지 않습니다.
        canSprint = !isAim;
        if(isAim && Input.GetButtonDown(ButtonName.Shoulder))
        {
            aimCamoffset.x = aimCamoffset.x * -1;
            aimPivotoffset.x = aimPivotoffset.x * -1;


        }
        behaviourController.GetAnimator.SetBool(aimBool, isAim);


    }

    private void OnGUI()
    {
        if(crossHair != null)
        {
            float length = behaviourController.GetCameraScript.GetCurrentPivotMagnitude(aimPivotoffset);
            if(length < 0.05f)
            {
                GUI.DrawTexture(new Rect(Screen.width * 0.5f - (crossHair.width * 0.5f), Screen.height * 0.5f - (crossHair.height * 0.5f), crossHair.width, crossHair.height), crossHair);


            }

        }
        
    }





}
