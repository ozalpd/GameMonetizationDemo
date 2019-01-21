using Pops.Extensions;
using UnityEngine;

namespace Pops.Controllers.UserInput
{
    public class SimpleTouchControl : MonoBehaviour
    {
        [Tooltip("A reference to the main camera in the scene.")]
        public Transform mainCamera;

        public bool useTouchControl = false;

        [Range(100, 20000)]
        public int touchSensitivity = 500;

        private AbstractPlayerController player;
        private MoveMechanism MoveMechanism { get { return player.moveMechanism; } }

        private Vector3 camForward;
        private Vector3 move;
        private float hInput; //Horizontal Input
        private float vInput; //Vertical Input
        private float speed { get { return player.speed; } }


        private Vector3 aimTo;
        private float xAim;
        private float zAim;

        protected virtual void Awake()
        {
            player = GetComponent<AbstractPlayerController>();
            if (mainCamera == null)
            {
                if (Camera.main != null)
                {
                    mainCamera = Camera.main.transform;
                }
                else
                {
                    Debug.LogWarning("Warning: No main camera found. Player needs a Camera tagged \"MainCamera\", for camera-relative controls.");
                }
            }

#if (UNITY_IOS || UNITY_ANDROID)
            useTouchControl = useTouchControl || !Debug.isDebugBuild;
#endif
        }

        protected virtual void Update()
        {
            if (useTouchControl)
            {
                if (Input.touchCount > 0)
                {
                    Touch touchZero = Input.GetTouch(0);
                    switch (touchZero.phase)
                    {
                        case TouchPhase.Began:
                            touchReleased = false;
                            touchBeginPoint = touchZero.position;
                            break;

                        case TouchPhase.Moved:
                            touchDelta = (touchZero.position - touchBeginPoint) / (float)touchSensitivity;
                            hInput = touchDelta.x;
                            vInput = touchDelta.y;
                            break;

                        case TouchPhase.Ended:
                            touchReleased = true;
                            break;

                        default:
                            break;
                    }
                    Debug.DrawLine(Camera.main.ScreenToWorldPoint(touchBeginPoint), Camera.main.ScreenToWorldPoint(touchZero.position));
                }

                if (touchReleased && touchDelta.magnitude > 0)
                {
                    touchDelta = Vector2.Lerp(touchDelta, Vector2.zero, Time.deltaTime * 5.0f);

                    if (touchDelta.magnitude < 0.05f)
                        touchDelta = Vector2.zero;

                    hInput = touchDelta.x;
                    vInput = touchDelta.y;
                }
            }
            else
            {
                hInput = Input.GetAxis("Horizontal");
                vInput = Input.GetAxis("Vertical");
                player.Attacking = Input.GetKey(KeyCode.LeftShift);
            }

            if (Mathf.Approximately(hInput, 0) && Mathf.Approximately(vInput, 0))
            {
                if (!didWeCallPlayerStopped)
                {
                    GameManager.PlayerStopped(player);
                    didWeCallPlayerStopped = true;
                }
            }
            else
            {
                didWeCallPlayerStopped = false;
            }

            if (mainCamera != null)  // calculate camera relative direction
            {
                camForward = Vector3.Scale(mainCamera.forward, new Vector3(1, 0, 1)).normalized;
                move = ((vInput * camForward + hInput * mainCamera.right) * speed).ClampMagnitude(speed);
            }
            else  // world-relative directions in the case of no camera
            {
                move = ((vInput * Vector3.forward + hInput * Vector3.right) * speed).ClampMagnitude(speed);
            }

            if (MoveMechanism.UsesRigidbody())
                return;
            GameManager.PlayerMoving(player, move);
            player.Move(move);
        }
        private bool didWeCallPlayerStopped;
        private bool touchReleased;
        private Vector2 touchBeginPoint;
        private Vector2 touchDelta = Vector2.zero;

        protected virtual void FixedUpdate()
        {
            if (MoveMechanism.UsesRigidbody())
            {
                GameManager.PlayerMoving(player, move);
                player.Move(move);
            }
        }
    }
}