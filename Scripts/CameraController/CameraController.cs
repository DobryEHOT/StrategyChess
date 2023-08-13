using Assets.Scripts.SelectorSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CameraController
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float speedCamera = 10f;
        [SerializeField] private float intensityMove = 1f;
        [SerializeField] private SelectorItems selector;

        private Vector3 nextPosition;
        private Quaternion nextRotation;
        private Vector2 oldTouchPosition;

        [SerializeField]
        private float distnce = 0;
        private float startDistnce = 0;

        private float min = 1;
        private float max = 8;

        private float minDown = 4;
        private float maxDown = 10;

        private float minActive = 0;
        private float maxActive = 0;

        private float angleRotateDown = 40f;
        private float OffsetUpWithDown = 2f;

        private bool isNotDown = false;
        private float speedCameraPull = 0.5f;

        void Start()
        {
            nextPosition = transform.position;
            startDistnce = nextPosition.z;
            nextRotation = transform.rotation;
            Input.simulateMouseWithTouches = true;
            minActive = min;
            maxActive = max;
        }

        void Update()
        {
            TouchCalculateCamera();
            CalculateCamera();
        }

        private void TouchCalculateCamera()
        {
            if (Input.GetMouseButtonDown(0))
                oldTouchPosition = Input.mousePosition;

            if (selector.ActiveVizor && Input.GetMouseButton(0))
            {
                var newTouchPosition = (Vector2)Input.mousePosition;
                var diff = oldTouchPosition - newTouchPosition;
                diff.x = 0;

                if (diff.y > 0)
                    distnce += intensityMove * Time.deltaTime * diff.magnitude * speedCameraPull;
                if (diff.y < 0)
                    distnce -= intensityMove * Time.deltaTime * diff.magnitude * speedCameraPull;

                oldTouchPosition = newTouchPosition;
            }
        }

        private void CalculateCamera()
        {
            distnce += Input.mouseScrollDelta.y * intensityMove;
            distnce = Mathf.Clamp(distnce, minActive, maxActive);
            nextPosition = new Vector3(nextPosition.x, nextPosition.y, startDistnce + distnce);
            transform.position = Vector3.Lerp(transform.position, nextPosition, Time.deltaTime * speedCamera);
            transform.rotation = Quaternion.Lerp(transform.rotation, nextRotation, Time.deltaTime * speedCamera);
        }

        public void SwithVision()
        {
            int i = +1;

            if (isNotDown)
            {
                minActive = min;
                maxActive = max;
                i = -1;
            }
            else
            {
                minActive = minDown;
                maxActive = maxDown;
            }

            var angle = transform.rotation.eulerAngles;
            nextRotation = Quaternion.Euler(angle.x + (angleRotateDown * i), angle.y, angle.z);
            nextPosition += new Vector3(0, OffsetUpWithDown * i, 0);
            isNotDown = isNotDown ? false : true;
        }

        public void LookDown()
        {
            if (isNotDown)
                return;

            isNotDown = true;
            transform.Rotate(new Vector3(angleRotateDown, 0, 0));
            nextPosition += new Vector3(0, OffsetUpWithDown, 0);
        }

        public void LookForward()
        {
            if (!isNotDown)
                return;

            isNotDown = false;
            transform.Rotate(new Vector3(-angleRotateDown, 0, 0));
            nextPosition += new Vector3(0, -OffsetUpWithDown, 0);
        }
    }
}