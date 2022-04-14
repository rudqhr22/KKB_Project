using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CAT
{
    public class PlayerLocomotion : MonoBehaviour
    {
        PlayerManager playerManager;
        Transform cameraObject;
        InputHandler inputHandler;
        public Vector3 moveDirection;
        PlayerStats playerStats;

        [HideInInspector]
        public Transform myTransform;
        [HideInInspector]
        public AnimatorHandler animatorHandler;

        public new Rigidbody rigidbody;
        public GameObject normalCamera;

        [Header("Ground & Air Deteation Stats")]
        [SerializeField]
        float groundDetectionRayStaryPoint = 0.5f;
        [SerializeField]
        float minimumDistanceNeededToBeginFall = 1f;
        [SerializeField]
        float groundDetectionRayDistance = 0.8f;
        LayerMask igonoreForGroundCheck;
        public float inAirTimer;

        [Header("Movement Stats")]
        [SerializeField]
        float movementSpeed = 2;
        [SerializeField]
        float walkingSpeed = 1;
        [SerializeField]
        float sprintSpeed = 4;
        [SerializeField]
        float rotationSpeed = 10;
        [SerializeField]
        float fallingSpeed = 45;

        [Header("Stamina Costs")]
        [SerializeField]
        float rollStaminaCost = 15f;
        float backstepStaminaCost = 15f;
        float sprintStaminaCost = 0.1f;

        private void Awake()
        {
            playerManager = GetComponentInParent<PlayerManager>();
            playerStats = GetComponent<PlayerStats>();
            rigidbody = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();            
        }

        void Start()
        {
            
            cameraObject = Camera.main.transform;
            myTransform = transform;
            animatorHandler.Initialize();

            playerManager.isGrounded = true;
            igonoreForGroundCheck = ~(1 << 8 | 1 << 11);
        }

 #region Movement
        Vector3 normalVector = Vector3.up;
        Vector3 targetPosition;

        void HandleRotaion(float delta)
        {
            Vector3 targetDIr = Vector3.zero;
            float moveOverride = inputHandler.moveAmount;

            targetDIr = cameraObject.forward * inputHandler.vertical;
            targetDIr += cameraObject.right * inputHandler.horizontal;

            targetDIr.Normalize();
            targetDIr.y = 0;

            if (targetDIr == Vector3.zero)
                targetDIr = myTransform.forward;

            float rs = rotationSpeed;

            Quaternion tr = Quaternion.LookRotation(targetDIr);
            Quaternion targetRotation = Quaternion.Slerp(myTransform.rotation, tr, rs * delta);

            myTransform.rotation = targetRotation;
        }

        public void HandleMovement(float delta)
        {
            if (inputHandler.rollFlag)
                return;
            
            moveDirection = cameraObject.forward * inputHandler.vertical;            
            moveDirection += cameraObject.right * inputHandler.horizontal;
            moveDirection.Normalize();
            moveDirection.y = 0;
            
            float speed = movementSpeed;

            if(inputHandler.sprintFlag && inputHandler.moveAmount > 0.5)
            {   
                speed = sprintSpeed;
                playerManager.isSprinting = true;
                moveDirection *= speed;
                playerStats.TakeStaminaDamage(sprintStaminaCost);
            }
            else
            {
                if(inputHandler.moveAmount < 0.5)
                {
                    moveDirection *= walkingSpeed;
                    playerManager.isSprinting = false;
                }
                else
                {
                    moveDirection *= speed;
                    playerManager.isSprinting = false;
                }                
            }            

            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            rigidbody.velocity = projectedVelocity;
            

            animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0, playerManager.isSprinting);


            if (playerManager.isInteracting) return;

            if (animatorHandler.canRotate)
            {
                HandleRotaion(delta);
            }
        }

        public void HandleRollingAndSprinting(float delta)
        {

            if (animatorHandler.anim.GetBool("isInteracting"))
                return;

            if (playerStats.currentStamina <= 0)
                return;

            if (inputHandler.rollFlag)
            {
                moveDirection = cameraObject.forward * inputHandler.vertical;
                moveDirection += cameraObject.right * inputHandler.horizontal;

                if(inputHandler.moveAmount > 0)
                {
                    animatorHandler.PlayTagetAnimation("Roll", true);
                    moveDirection.y = 0;
                    Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                    myTransform.rotation = rollRotation;
                    playerStats.TakeStaminaDamage(rollStaminaCost);
                }
                else
                {
                    animatorHandler.PlayTagetAnimation("Backstep", true);
                    playerStats.TakeStaminaDamage(backstepStaminaCost);
                }
            }

        }

        public void HandleFalling(float delta, Vector3 moveDirection)
        {
            playerManager.isGrounded = false;
            
            RaycastHit hit;
            Vector3 origin = myTransform.position;
            origin.y += groundDetectionRayStaryPoint;

            if (Physics.Raycast(origin, myTransform.forward, out hit, 0.4f))
            {
                moveDirection = Vector3.zero;
            }

            if (playerManager.isInAir)
            {
                rigidbody.AddForce(-Vector3.up * fallingSpeed);
                rigidbody.AddForce(moveDirection * fallingSpeed / 10f);
            }

            Vector3 dir = moveDirection;
            dir.Normalize();
            origin = origin + dir * groundDetectionRayDistance;

            targetPosition = myTransform.position;

            Debug.DrawRay(origin, -Vector3.up * minimumDistanceNeededToBeginFall, Color.red, 0.1f, false);

            if (Physics.Raycast(origin, -Vector3.up, out hit, minimumDistanceNeededToBeginFall, igonoreForGroundCheck))
            {
                normalVector = hit.normal;
                Vector3 tp = hit.point;
                playerManager.isGrounded = true;
                targetPosition.y = tp.y;

                if (playerManager.isInAir)
                {
                    if (inAirTimer > 0.5f)
                    {   
                        animatorHandler.PlayTagetAnimation("Land", true);
                        inAirTimer = 0;
                    }
                    else
                    {
                        animatorHandler.PlayTagetAnimation("Locomotion", false);
                        inAirTimer = 0;
                    }
                    playerManager.isInAir = false;
                }
            }
            else
            {
                if (playerManager.isGrounded)
                {
                    playerManager.isGrounded = false;
                }

                if (playerManager.isInAir == false)
                {
                    if (playerManager.isInteracting == false)
                    {
                        animatorHandler.PlayTagetAnimation("Falling", true);
                    }
                    Vector3 vel = rigidbody.velocity;
                    vel.Normalize();
                    rigidbody.velocity = vel * (movementSpeed / 2);
                    playerManager.isInAir = true;
                }
            }

            //if (playerManager.isGrounded)
            //{
                if (playerManager.isInteracting || inputHandler.moveAmount > 0)
                {
                    myTransform.position = Vector3.Lerp(myTransform.position, targetPosition, Time.deltaTime / 0.1f);
                }
                else
                {
                    myTransform.position = targetPosition;
                }
            //}

            #endregion
        }
        
    }
}