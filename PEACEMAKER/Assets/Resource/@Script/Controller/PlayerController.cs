using UnityEngine;
using MManager =  Resource.Script.Managers.Managers;

namespace Resource.Script.Controller
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [Tooltip("How quickly the player accelerates to the target movement speed.")]
        public float acceleration = 0.1f;

        [Tooltip("Default walking speed.")]
        public float walkSpeed = 5;

        [Tooltip("Movement speed while crouching.")]
        public float crouchSpeed = 3;

        [Tooltip("Movement speed while sprinting.")]
        public float sprintSpeed = 10;

        [Tooltip("Movement speed during tactical sprinting (faster than normal sprint).")]
        public float tacticalSprintSpeed = 11;

        [Tooltip("How high the player can jump.")]
        public float jumpHeight = 6;

        [Tooltip("Player's height when crouched.")]
        public float crouchHeight = 1.5f;

        [Tooltip("Distance between footstep sounds (lower = more frequent).")]
        public float stepInterval = 7;

        [Tooltip("Automatically detects and follows moving platforms.")]
        public bool autoDetectMovingPlatforms = true;

        [Tooltip("If true, maintains horizontal momentum when jumping or falling.")]
        public bool preserveMomentum = true;

        [Range(0f, 1f)]
        [Tooltip("Fraction of momentum preserved when jumping or falling. For example, 0.2 means 20% is lost and 80% is carried over.")]
        public float momentumLoss = 0.2f;
        
        [Header("Slopes")]
        [Tooltip("If true, the player will slide down steep slopes automatically.")]
        public bool slideDownSlopes = true;

        [Tooltip("Speed at which the player slides down slopes.")]
        public float slopeSlideSpeed = 1;

        [Space]
        [Tooltip("Strength of gravity applied to the player.")]
        public float gravity = 1;

        [Tooltip("Maximum speed the player can fall.")]
        public float maxFallSpeed = 350;

        [Tooltip("Extra downward force applied to keep the player grounded on slopes or uneven terrain.")]
        public float stickToGroundForce = 0.5f;
    
        private Vector3 _finalVelocity;
        
        
        /// <summary>
        ///  Controller for Player
        /// </summary>
        public CharacterController CController { get; set; }
        
        void Awake()
        {
            
        }
    
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            CController = GetComponent<CharacterController>();
        }

        // Update is called once per frame
        void Update()
        {
            if (CController.isGrounded)
            {
                _finalVelocity.x = MManager.Input.Move.x * walkSpeed;
                _finalVelocity.z = MManager.Input.Move.y * walkSpeed;
            }
            else if (CController.velocity.magnitude * 3.5< maxFallSpeed)
            {
                _finalVelocity.y += Physics.gravity.y * gravity * Time.deltaTime;
            }
            CController.Move(_finalVelocity * Time.deltaTime);
            
            //Vector2 move = MManager.Input.Move;
            
            if (MManager.Input.JumpPressed)
                Jump();

            if (MManager.Input.FirePressed)
                Fire();
            
            //if (move != Vector2.zero)
                //Move(move);

        }

        private void Jump()
        {
            Debug.Log("Jump");
        }

        private void Fire()
        {
            Debug.Log("Fire");
        }

        private void Move(Vector2 vec)
        {
            Debug.Log(vec);
        }
    }

}