using UnityEngine;

namespace NewGame.SmokeTest
{
    /// <summary>On-screen FPS counter drawn with IMGUI (no UI package or assets required).</summary>
    public class FpsLabel : MonoBehaviour
    {
        private const float RefreshSeconds = 0.5f;

        private float _accumTime;
        private int _accumFrames;
        private float _fps;
        private string _text = "FPS: --";
        private GUIStyle _style;

        private void Update()
        {
            _accumTime += Time.unscaledDeltaTime;
            _accumFrames++;

            if (_accumTime >= RefreshSeconds)
            {
                _fps = _accumFrames / _accumTime;
                _text = "FPS: " + _fps.ToString("0.0");
                _accumTime = 0f;
                _accumFrames = 0;
            }
        }

        private void OnGUI()
        {
            if (_style == null)
            {
                _style = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.UpperLeft,
                };
                _style.normal.textColor = Color.white;
            }

            _style.fontSize = Mathf.Max(14, Mathf.RoundToInt(Screen.height * 0.035f));

            var safe = Screen.safeArea;
            var margin = _style.fontSize * 0.5f;
            var topInset = Screen.height - (safe.y + safe.height);
            var rect = new Rect(safe.x + margin, topInset + margin, Screen.width, _style.fontSize * 3f);

            GUI.Label(rect, _text + "\nDrag to rotate", _style);
        }
    }
}
