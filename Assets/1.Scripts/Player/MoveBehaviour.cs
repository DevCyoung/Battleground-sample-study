using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 이동과 점프 동작을 담당하는 컴포넌트
/// 충돌처리에 대한 기능도 포함
/// 기본 동작으로써 작동
/// </summary>
public class MoveBehaviour : GenericBehaviour
{
    public float walkSpeed = 0.15f;
    public float runSpeed = 1.0f;
    public float sprintSpeed = 2.0f;
    public float speedDameTime = 0.1f;
    public float jumpHeight = 1.5f;
    public float jumpInertialForce = 10f; // 점프 관성
    public float speed, speedSeeker;

    private int jumpBool; // 애니메이터 해시용
    private int groundedBool;

    private bool jump;
    private bool isColliding; // 충돌중이냐

    private CapsuleCollider capsuleCollider;
    private Transform myTransform;

    private void Start()
    {
        myTransform = transform;
        speedSeeker = runSpeed;

        capsuleCollider = GetComponent<CapsuleCollider>();

        jumpBool     = Animator.StringToHash(FC.AnimatorKey.Jump);
        groundedBool = Animator.StringToHash(FC.AnimatorKey.Grounded);

        behaviourController.SubScribeBehaviour(this);
        behaviourController.GetAnimator.SetBool(groundedBool, true);
        behaviourController.RegisterDefalutBehaviour(this.behaviourCode);
        
    }

    private void Update()
    {

        if(!jump && Input.GetButtonDown(ButtonName.Jump)
            &&behaviourController.IsCurrentBehaviour(this.behaviourCode)
            && !behaviourController.IsOverriding())
        {
            jump = true;
        }

    }

    private void OnCollisionStay(Collision collision)
    {
        isColliding = true;
        if(behaviourController.IsCurrentBehaviour(GetBehaviourCode)
            && collision.GetContact(0).normal.y <= 0.1f)
        {

            float vel = behaviourController.GetAnimator.velocity.magnitude;
            Vector3 targetMove = Vector3.ProjectOnPlane(myTransform.forward, collision.GetContact(0).normal).normalized * vel;
            behaviourController.GetRigidbody.AddForce(targetMove, ForceMode.VelocityChange);

        }
        
      
        
    }
    private void OnCollisionExit(Collision collision)
    {
        isColliding = false;
    }



    private void MovementManagement(float horizontal , float vertical)
    {
        if (behaviourController.IsGrounded())
        {
            behaviourController.GetRigidbody.useGravity = true;

        }
        else if( !behaviourController.GetAnimator.GetBool(jumpBool) 
            && behaviourController.GetRigidbody.velocity.y > 0) 
        {
            RemoveVerticalVelocity();
        }

        Rotating(horizontal, vertical);
        Vector2 dir  = new Vector2(horizontal, vertical);
        speed        = Vector2.ClampMagnitude(dir, 1f).magnitude;
        speedSeeker += Input.GetAxis("Mouse ScrollWheel");
        speedSeeker  = Mathf.Clamp(speedSeeker, walkSpeed, runSpeed);

        if (behaviourController.IsSprinting())
        {
            speed = sprintSpeed;
        }
        behaviourController.GetAnimator.SetFloat(speedFloat, speed, speedDameTime, Time.deltaTime);

    }
    private Vector3 Rotating(float horizontal , float vertical) 
    {

        Vector3 forward = behaviourController.playerCamera.TransformDirection(Vector3.forward);
        forward.y = 0.0f;
        forward = forward.normalized;

        //forward 에 직교하는 벡터

        Vector3 right = new Vector3(forward.z, 0.0f, -forward.x);
        Vector3 targetDirection = Vector3.zero;
        targetDirection = forward * vertical + right * horizontal;

        if(behaviourController.IsMoving() && targetDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            Quaternion newRotation = Quaternion.Slerp(behaviourController.GetRigidbody.rotation,
              targetRotation, behaviourController.turnSmoothing);
            behaviourController.GetRigidbody.MoveRotation(newRotation);
            behaviourController.SetLastDirection(targetDirection);

        }
        if(!(Mathf.Abs(horizontal) > 0.9f || Mathf.Abs(vertical) > 0.9f))
        {
            behaviourController.Repositioning();


        }
        return targetDirection;
        
        
    }
    private void RemoveVerticalVelocity()
    {
        Vector3 horizontalVelocity = behaviourController.GetRigidbody.velocity;
        horizontalVelocity.y = 0.0f;
        behaviourController.GetRigidbody.velocity = horizontalVelocity;

    }

    public override void LocalFixedUpdate()
    {
        MovementManagement(behaviourController.GetH, behaviourController.GetV);
        JumpManagement();

    }
    private void JumpManagement()
    {
        if ( jump && !behaviourController.GetAnimator.GetBool(jumpBool) && behaviourController.IsGrounded())
        {
            behaviourController.LockTempBehaviour(behaviourCode);
            behaviourController.GetAnimator.SetBool(jumpBool, true);

            if(   behaviourController.GetAnimator.GetFloat(speedFloat) > 0.1f )
            {
                capsuleCollider.material.dynamicFriction = 0f;
                capsuleCollider.material.staticFriction = 0f;

                RemoveVerticalVelocity();

                float velocity = 2f * Mathf.Abs(Physics.gravity.y) * jumpHeight;
                velocity = Mathf.Sqrt(velocity);

                behaviourController.GetRigidbody.AddForce(Vector3.up * velocity, ForceMode.VelocityChange);

            }
        }
        else if ( behaviourController.GetAnimator.GetBool(jumpBool)  )
        {
            
            if ( !behaviourController.IsGrounded() && !isColliding && !behaviourController.GetTempLockStatus() )
            {
                behaviourController.GetRigidbody.AddForce( myTransform.forward * jumpInertialForce * Physics.gravity.magnitude * sprintSpeed, ForceMode.Acceleration);

                Debug.Log(myTransform.forward * jumpInertialForce * Physics.gravity.magnitude * sprintSpeed);

            }

            if(behaviourController.GetRigidbody.velocity.y < 0f && behaviourController.IsGrounded())
            {
                behaviourController.GetAnimator.SetBool(groundedBool, true);
                capsuleCollider.material.dynamicFriction = 0.6f;
                capsuleCollider.material.staticFriction = 0.6f;
                jump = false;
                behaviourController.GetAnimator.SetBool(jumpBool, false);
                behaviourController.UnLockTempBehaviour(this.behaviourCode);

                    

                
            }

        }
    }

















}
