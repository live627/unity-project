using System;
using System.IO;
using UnityEngine;

namespace Main
{
    public class CameraControls
    {
        private Camera cam;
        private Transform m_Transform; //camera tranform

        #region Movement

        public float keyboardMovementSpeed = 15f; //speed with keyboard movement
        public float screenEdgeMovementSpeed = 20f; //speed with screen edge movement
        public float followingSpeed = 5f; //speed when following a target
        public float rotationSpeed = 3f;
        public float panningSpeed = 10f;
        public float mouseRotationSpeed = 5f;

        #endregion

        #region Height

        public bool autoHeight = true;
        public LayerMask groundMask = -1; //layermask of ground or other objects that affect height

        public float maxHeight = 10f; //maximal height
        public float minHeight = 150f; //minimnal height
        public float heightDampening = 5f;
        public float keyboardZoomingSensitivity = 2f;
        public float scrollWheelZoomingSensitivity = 25f;

        private float zoomPos = 0.8f; //value in range (0, 1) used as t in Matf.Lerp

        #endregion

        #region MapLimits

        public bool limitMap = true;
        public float limitX = 50f; //x limit of map
        public float limitY = 50f; //z limit of map

        #endregion

        #region Targeting

        public Transform targetFollow; //target to follow
        public Vector3 targetOffset;

        /// <summary>
        /// are we following target
        /// </summary>
        public bool FollowingTarget
        {
            get
            {
                return targetFollow != null;
            }
        }

        #endregion

        #region Input

        public bool useScreenEdgeInput = true;
        public float screenEdgeBorder = 25f;

        public bool useKeyboardInput = true;
        public string horizontalAxis = "Horizontal";
        public string verticalAxis = "Vertical";

        public bool usePanning = true;
        public KeyCode panningKey = KeyCode.Mouse2;

        public bool useKeyboardZooming = true;
        public KeyCode zoomInKey = KeyCode.E;
        public KeyCode zoomOutKey = KeyCode.Q;

        public bool useScrollwheelZooming = true;
        public string zoomingAxis = "Mouse ScrollWheel";

        public bool useKeyboardRotation = true;
        public KeyCode rotateRightKey = KeyCode.X;
        public KeyCode rotateLeftKey = KeyCode.Z;

        public bool useMouseRotation = true;
        public KeyCode mouseRotationKey = KeyCode.Mouse1;

        private Vector2 KeyboardInput
        {
            get { return useKeyboardInput ? new Vector2(Input.GetAxis(horizontalAxis), Input.GetAxis(verticalAxis)) : Vector2.zero; }
        }

        private Vector2 MouseInput
        {
            get { return Input.mousePosition; }
        }

        private float ScrollWheel
        {
            get { return Input.GetAxis(zoomingAxis); }
        }

        private Vector2 MouseAxis
        {
            get { return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")); }
        }

        private int ZoomDirection
        {
            get
            {
                bool zoomIn = Input.GetKey(zoomInKey);
                bool zoomOut = Input.GetKey(zoomOutKey);
                if (zoomIn && zoomOut)
                {
                    return 0;
                }
                else if (!zoomIn && zoomOut)
                {
                    return 1;
                }
                else if (zoomIn && !zoomOut)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }

        private int RotationDirection
        {
            get
            {
                bool rotateRight = Input.GetKey(rotateRightKey);
                bool rotateLeft = Input.GetKey(rotateLeftKey);
                if (rotateLeft && rotateRight)
                {
                    return 0;
                }
                else if (rotateLeft && !rotateRight)
                {
                    return -1;
                }
                else if (!rotateLeft && rotateRight)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        #endregion

        public CameraControls(Camera camera)
        {
            cam = camera;
            m_Transform = cam.transform;
            SetRotation();
            ResetZoom();

            // Add callbacks for our custom binary file events.
            DataHandler.WriteBinary += OnWriteBinary;
            DataHandler.ReadBinary += OnReadBinary;

            // Add callback for our custom MonoBehavior.Update() event.
            Main.Updating += OnUpdate;
        }

        private void OnReadBinary(BinaryReader reader)
        {
            reader.ReadSingle(); reader.ReadSingle(); reader.ReadSingle(); reader.ReadSingle(); reader.ReadSingle(); reader.ReadSingle();
            //m_Transform.localPosition = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            //m_Transform.localEulerAngles = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }

        private void OnWriteBinary(BinaryWriter writer)
        {
            writer.Write(m_Transform.localPosition.x);
            writer.Write(m_Transform.localPosition.y);
            writer.Write(m_Transform.localPosition.z);
            writer.Write(m_Transform.localEulerAngles.x);
            writer.Write(m_Transform.localEulerAngles.y);
            writer.Write(m_Transform.localEulerAngles.z);
        }

        /// <summary>
        /// update camera movement and rotation
        /// </summary>
        public void OnUpdate()
        {
            if (FollowingTarget)
            {
                FollowTarget();
            }
            else
            {
                Move();
            }

            HeightCalculation();
            Rotation();
            //LimitPosition();
        }

        /// <summary>
        /// move camera with keyboard or with screen edge
        /// </summary>
        private void Move()
        {
            if (useKeyboardInput)
            {
                Vector3 desiredMove = new Vector3(KeyboardInput.x, 0, KeyboardInput.y);

                desiredMove *= keyboardMovementSpeed;
                desiredMove *= Time.deltaTime;
                desiredMove = Quaternion.Euler(new Vector3(0f, m_Transform.eulerAngles.y, 0f)) * desiredMove;
                desiredMove = m_Transform.InverseTransformDirection(desiredMove);

                m_Transform.Translate(desiredMove, Space.Self);
            }

            if (useScreenEdgeInput)
            {
                Vector3 desiredMove = new Vector3();

                Rect leftRect = new Rect(0, 0, screenEdgeBorder, Screen.height);
                Rect rightRect = new Rect(Screen.width - screenEdgeBorder, 0, screenEdgeBorder, Screen.height);
                Rect upRect = new Rect(0, Screen.height - screenEdgeBorder, Screen.width, screenEdgeBorder);
                Rect downRect = new Rect(0, 0, Screen.width, screenEdgeBorder);

                desiredMove.x = leftRect.Contains(MouseInput) ? -1 : rightRect.Contains(MouseInput) ? 1 : 0;
                desiredMove.z = upRect.Contains(MouseInput) ? 1 : downRect.Contains(MouseInput) ? -1 : 0;

                desiredMove *= screenEdgeMovementSpeed;
                desiredMove *= Time.deltaTime;
                desiredMove = Quaternion.Euler(new Vector3(0f, m_Transform.eulerAngles.y, 0f)) * desiredMove;
                desiredMove = m_Transform.InverseTransformDirection(desiredMove);

                m_Transform.Translate(desiredMove, Space.Self);
            }

            if (usePanning && Input.GetKey(panningKey) && MouseAxis != Vector2.zero)
            {
                Vector3 desiredMove = new Vector3(-MouseAxis.x, 0, -MouseAxis.y);

                desiredMove *= panningSpeed;
                desiredMove *= Time.deltaTime;
                desiredMove = Quaternion.Euler(new Vector3(0f, m_Transform.eulerAngles.y, 0f)) * desiredMove;
                desiredMove = m_Transform.InverseTransformDirection(desiredMove);

                m_Transform.Translate(desiredMove, Space.Self);
            }
        }

        /// <summary>
        /// calcualte height
        /// </summary>
        private void HeightCalculation()
        {
            float distanceToGround = DistanceToGround();
            if (useScrollwheelZooming)
            {
                zoomPos += ScrollWheel * Time.deltaTime * scrollWheelZoomingSensitivity;
            }

            if (useKeyboardZooming)
            {
                zoomPos += ZoomDirection * Time.deltaTime * keyboardZoomingSensitivity;
            }

            zoomPos = Mathf.Clamp01(zoomPos);

            float targetHeight = Mathf.Lerp(minHeight, maxHeight, zoomPos);
            float difference = 0;

            //if (distanceToGround != targetHeight)
            //{
            //    difference = targetHeight - distanceToGround;
            //}

            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetHeight + difference, Time.deltaTime * heightDampening);
            //cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetHeight + difference, Time.deltaTime * heightDampening);
            //float y = (targetHeight + difference) * Mathf.Min(4, 80 / cam.fieldOfView);
            //m_Transform.position = Vector3.Lerp(m_Transform.position,
            //    new Vector3(m_Transform.position.x, y, m_Transform.position.z), Time.deltaTime * heightDampening);
        }

        /// <summary>
        /// Rotation of the Camera based on Mouse Coordinates
        /// </summary>
        private void Rotation()
        {
            if (Input.GetMouseButton(2) && (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0))
            {
                float x = 0.0f;
                float y = 0.0f;
                Vector3 angles = m_Transform.eulerAngles;
                x = angles.y;
                y = angles.x;
                x += Input.GetAxis("Mouse X") * mouseRotationSpeed;
                y -= Input.GetAxis("Mouse Y") * mouseRotationSpeed;

                y = Mathf.Clamp(y, 5f, 350f);
                Quaternion rotation = Quaternion.Euler(y, x, 0);
                m_Transform.rotation = rotation;
            }
        }

        /// <summary>
        /// TODO write this/rename method
        /// </summary>
        private void ResetZoom()
        {
            float targetHeight = Erp(minHeight, maxHeight, zoomPos);
            m_Transform.position = new Vector3(m_Transform.position.x, targetHeight, m_Transform.position.z);
        }

        /// <summary>
        /// interpolates from one value to another, is essentially Mathf.Lerp() but returns the final value straight away.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        float Erp(float a, float b, float t)
        {
            return a * (1 - t) + b * t;
        }

        /// <summary>
        /// Set the camera's viewing angles to defaults.
        /// </summary>
        private void SetRotation()
        {
            m_Transform.localEulerAngles = new Vector3(60, 50, m_Transform.localEulerAngles.z);
        }

        /// <summary>
        /// follow targetif target != null
        /// </summary>
        private void FollowTarget()
        {
            Vector3 targetPos = new Vector3(targetFollow.position.x, m_Transform.position.y, targetFollow.position.z) + targetOffset;
            m_Transform.position = Vector3.MoveTowards(m_Transform.position, targetPos, Time.deltaTime * followingSpeed);
        }

        /// <summary>
        /// limit camera position
        /// </summary>
        private void LimitPosition()
        {
            if (!limitMap)
            {
                return;
            }

            m_Transform.position = new Vector3(Mathf.Clamp(m_Transform.position.x, -limitX, limitX),
                m_Transform.position.y,
                Mathf.Clamp(m_Transform.position.z, -limitY, limitY));
        }

        /// <summary>
        /// set the target
        /// </summary>
        /// <param name="target"></param>
        public void SetTarget(Transform target)
        {
            targetFollow = target;
        }

        /// <summary>
        /// reset the target (target is set to null)
        /// </summary>
        public void ResetTarget()
        {
            targetFollow = null;
        }

        /// <summary>
        /// calculate distance to ground
        /// </summary>
        /// <returns></returns>
        private float DistanceToGround()
        {
            Ray ray = new Ray(m_Transform.position, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundMask.value))
            {
                return (hit.point - m_Transform.position).magnitude;
            }

            return 0f;
        }

        /// <summary>
        /// raycasst to center of screen
        /// </summary>
        /// <returns></returns>
        private bool IsMapInView()
        {
            return Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), Mathf.Infinity, groundMask.value);
        }
    }
}