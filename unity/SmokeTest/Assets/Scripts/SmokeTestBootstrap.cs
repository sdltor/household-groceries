using UnityEngine;
using UnityEngine.Rendering;

namespace NewGame.SmokeTest
{
    /// <summary>
    /// Builds the whole smoke-test scene in code so it works from any (even empty) scene,
    /// in the Editor and in player builds, without relying on serialized scene content.
    /// </summary>
    public static class SmokeTestBootstrap
    {
        public const string RootName = "SmokeTestRoot";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Build()
        {
            if (GameObject.Find(RootName) != null)
            {
                return;
            }

            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;

            var root = new GameObject(RootName);

            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.45f, 0.5f, 0.55f);

            CreateCamera(root.transform);
            CreateLight(root.transform);
            CreateGround(root.transform);
            CreateSubject(root.transform);

            root.AddComponent<FpsLabel>();
        }

        private static void CreateCamera(Transform parent)
        {
            var go = new GameObject("Main Camera");
            go.tag = "MainCamera";
            go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(0f, 2.2f, -5f);
            go.transform.LookAt(new Vector3(0f, 0.8f, 0f));

            var cam = go.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.12f, 0.16f, 0.22f);
            cam.fieldOfView = 60f;
            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = 100f;

            go.AddComponent<AudioListener>();
        }

        private static void CreateLight(Transform parent)
        {
            var go = new GameObject("Directional Light");
            go.transform.SetParent(parent, false);
            go.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            var light = go.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = Color.white;
            light.intensity = 1.1f;
            light.shadows = LightShadows.Soft;
        }

        private static void CreateGround(Transform parent)
        {
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.SetParent(parent, false);
            ground.transform.localScale = new Vector3(2f, 1f, 2f);
            Tint(ground, new Color(0.35f, 0.4f, 0.35f));
            RemoveCollider(ground);
        }

        private static void CreateSubject(Transform parent)
        {
            var pivot = new GameObject("Subject");
            pivot.transform.SetParent(parent, false);
            pivot.transform.position = new Vector3(0f, 1f, 0f);
            pivot.AddComponent<DragRotate>();

            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "Cube";
            cube.transform.SetParent(pivot.transform, false);
            Tint(cube, new Color(0.9f, 0.35f, 0.25f));
            RemoveCollider(cube);

            var capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            capsule.name = "Capsule";
            capsule.transform.SetParent(pivot.transform, false);
            capsule.transform.localPosition = new Vector3(1.4f, -0.1f, 0f);
            capsule.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            Tint(capsule, new Color(0.25f, 0.6f, 0.95f));
            RemoveCollider(capsule);
        }

        private static void Tint(GameObject go, Color color)
        {
            var rend = go.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.color = color;
            }
        }

        private static void RemoveCollider(GameObject go)
        {
            var col = go.GetComponent<Collider>();
            if (col != null)
            {
                Object.Destroy(col);
            }
        }
    }
}
