using FC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 마우스 오른쪽 버튼 조준 , 다른 동작을 대체해서 동작하게 됩니다.
/// 마우스 휠버튼으로 좌우 카메라 변경
/// 벽의 모서리에서 조준할때 상체를 살짝 기울여주는 기능.
/// </summary>
public class AimBehaviour : GenericBehaviour
{

    public Texture2D crossHair; // 십자선 이미지
    public float anumTurnSmoothing = 0.15f; // 카메라를 향하도록 조준할때 회전속도
    public Vector3 aimPivotoffset = new Vector3(0.5f, 1.2f, 0.0f);
    public Vector3 anumCamoffset = new Vector3(0.0f, 0.4f, -0.7f);

    private int aimBool; // 애니메이터 페러메터 조준관련
    private bool isaim;
    private int conerBool; // 애니메이터 관련 코너 
    private bool peekCorner; // 플레이어가 코너 모서리에있는가
    private Vector3 initialRootRotation; // 루트 본 로컬 회전값
    private Vector3 initialHipRotation;
    private Vector3 initialSpineRotation;

    private void Start()
    {
        aimBool = Animator.StringToHash(AnimatorKey.Aim);
        conerBool = Animator.StringToHash(AnimatorKey.Corner);

        Transform hips = behaviourController.GetAnimator.GetBoneTransform(HumanBodyBones.Hips);

        
        
        
        





        
    }









}
