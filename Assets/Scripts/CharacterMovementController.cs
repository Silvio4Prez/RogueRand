using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementController : MonoBehaviour
{

    [SerializeField] CharacterController characterController;
    [SerializeField] Animator animator;
    [SerializeField] Transform groundChecker;
    [SerializeField] LayerMask groundMask;
    [SerializeField] Transform targetTransform;
    [SerializeField] LayerMask mouseAimMask;
    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] float walkingSpeed;
    [SerializeField] float runningSpeed;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float jumpHeight = 3;

    Camera mainCamera;
    float xInput;
    float speed = 3;
    Vector3 velocity;
    bool isGrounded;


    int FacingSide 
    {
        get 
        {
            Vector3 perp = Vector3.Cross(transform.forward, Vector3.forward);
            float dir = Vector3.Dot(perp, transform.up);
            return dir > 0f ? -1 : dir < 0 ? 1 : 0;
        }
    }

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        xInput = Input.GetAxisRaw("Horizontal");

        #region MOUSE AIMING
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mouseAimMask))
        {
            targetTransform.position = hit.point;
        }
        #endregion

        #region GRAVITY & JUMP LOGIC
        isGrounded = Physics.CheckSphere(groundChecker.position, groundDistance, groundMask);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        // Apply gravity
        velocity.y += gravity * Time.fixedDeltaTime;
        characterController.Move(velocity * Time.deltaTime);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2;
        }
        #endregion

        #region FACING DIRECTION
        // Character Facing Direction of Mouse Logic
        Quaternion rotateTo = Quaternion.Euler(new Vector3(0, 110 * Mathf.Sign(targetTransform.position.x - transform.position.x), 0));
        transform.rotation = Quaternion.Lerp(transform.rotation, rotateTo, 10 * Time.deltaTime);

        #endregion

        #region ANIMATIONS
        //Walking Animation Logic
        if (xInput != 0 && !Input.GetButton("Fire3"))
        {
            speed = walkingSpeed;
            animator.SetBool("isWalking", true);
            animator.SetBool("isRunning", false);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        //Running Animation Logic
        if (xInput != 0 && Input.GetButton("Fire3"))
        {
            speed = runningSpeed;
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", true);
        }
        else
        {
            speed = walkingSpeed;
            animator.SetBool("isRunning", false);
        }

        // Jumping Animation Logic
        if (!isGrounded)
        {
            animator.SetBool("isJumping", true);
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
        }
        else 
        {
            animator.SetBool("isJumping", false);
        }
        #endregion
    }

    private void FixedUpdate()
    {
        #region CHARACTER MOVEMENT
        // Character Movement Logic
        Vector3 move = new Vector3(xInput, 0, 0);
        characterController.Move( move * Time.fixedDeltaTime * speed);


        // hack to alwayys evaluate the multiplying speed of the animation to 1
        float animationSpeedMultiplier = 1;

        if (xInput < 0)
            animationSpeedMultiplier = -1;
        else
            animationSpeedMultiplier = 1;

        // Sets the animation parameter speed so it can make the player turn where the mouse is aiming
        animator.SetFloat("speed", animationSpeedMultiplier * FacingSide);
        #endregion
    }

    private void OnAnimatorIK()
    {
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        animator.SetIKPosition(AvatarIKGoal.RightHand, targetTransform.position);
    }
}