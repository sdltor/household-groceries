using UnityEngine;

namespace NewGame.SmokeTest
{
    /// <summary>
    /// Drag with one finger (or the left mouse button) to rotate. Spins slowly when idle,
    /// which also keeps the app visibly alive if the legacy Input Manager is disabled.
    /// </summary>
    public class DragRotate : MonoBehaviour
    {
        [SerializeField] private float degreesPerPixel = 0.3f;
        [SerializeField] private float idleSpinDegreesPerSecond = 20f;

#if ENABLE_LEGACY_INPUT_MANAGER
        private bool _mouseDragging;
        private Vector3 _lastMouse;
#endif

        private void Update()
        {
            var delta = Vector2.zero;
            var dragging = false;

#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                dragging = touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled;
                if (touch.phase == TouchPhase.Moved)
                {
                    delta = touch.deltaPosition;
                }
                _mouseDragging = false;
            }
            else if (Input.GetMouseButton(0))
            {
                dragging = true;
                if (_mouseDragging)
                {
                    delta = Input.mousePosition - _lastMouse;
                }
                _lastMouse = Input.mousePosition;
                _mouseDragging = true;
            }
            else
            {
                _mouseDragging = false;
            }
#endif

            if (dragging)
            {
                transform.Rotate(Vector3.up, -delta.x * degreesPerPixel, Space.World);
                transform.Rotate(Vector3.right, delta.y * degreesPerPixel, Space.World);
            }
            else
            {
                transform.Rotate(Vector3.up, idleSpinDegreesPerSecond * Time.deltaTime, Space.World);
            }
        }
    }
}
