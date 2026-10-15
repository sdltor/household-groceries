using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace NewGame.SmokeTest.EditorTools
{
    public static class SmokeTestBuilder
    {
        public const string ApplicationId = "com.newgame.smoketest";
        public const string ScenePath = "Assets/Scenes/SmokeTest.unity";
        public const string DefaultOutputRelativePath = "Builds/SmokeTest.apk";

        private const string OutputPathArg = "-outputPath";

        /// <summary>Menu entry: Build/Android APK</summary>
        [MenuItem("Build/Android APK")]
        public static void BuildAndroidApkMenu()
        {
            var result = BuildAndroid(DefaultOutputPath());
            if (result.Success)
            {
                EditorUtility.DisplayDialog("Android APK", "Build succeeded:\n" + result.Path, "OK");
                EditorUtility.RevealInFinder(result.Path);
            }
            else
            {
                EditorUtility.DisplayDialog("Android APK", "Build failed. See the Console for details.\n" + result.Message, "OK");
            }
        }

        /// <summary>Menu entry: Build/Create Smoke Test Scene (optional; the scene is only a container)</summary>
        [MenuItem("Build/Create Smoke Test Scene")]
        public static void CreateSceneMenu()
        {
            EnsureScene();
            AssetDatabase.Refresh();
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath);
        }

        /// <summary>
        /// Batchmode entry point:
        /// Unity -batchmode -nographics -quit -projectPath . -buildTarget Android
        ///       -executeMethod NewGame.SmokeTest.EditorTools.SmokeTestBuilder.BuildAndroidCli
        /// Optional: -outputPath path/to/file.apk. Exits with code 0 on success, 1 on failure.
        /// </summary>
        public static void BuildAndroidCli()
        {
            var output = GetArg(OutputPathArg) ?? DefaultOutputPath();
            var result = BuildAndroid(output);
            EditorApplication.Exit(result.Success ? 0 : 1);
        }

        private static string ProjectRoot()
        {
            return Directory.GetParent(Application.dataPath).FullName;
        }

        private static string DefaultOutputPath()
        {
            return Path.Combine(ProjectRoot(), DefaultOutputRelativePath);
        }

        private static string GetArg(string name)
        {
            var args = System.Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length - 1; i++)
            {
                if (args[i] == name)
                {
                    return args[i + 1];
                }
            }

            return null;
        }

        private static void ApplyAndroidSettings()
        {
            PlayerSettings.companyName = "NewGame";
            PlayerSettings.productName = "SmokeTest";
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, ApplicationId);
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
            EditorUserBuildSettings.buildAppBundle = false;
        }

        private static void EnsureScene()
        {
            if (File.Exists(Path.Combine(ProjectRoot(), ScenePath)))
            {
                return;
            }

            Directory.CreateDirectory(Path.Combine(ProjectRoot(), "Assets/Scenes"));

            // Additive so an already-open, possibly unsaved scene is left untouched.
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorSceneManager.CloseScene(scene, true);
            AssetDatabase.Refresh();
        }

        private struct BuildOutcome
        {
            public bool Success;
            public string Path;
            public string Message;
        }

        private static BuildOutcome BuildAndroid(string outputPath)
        {
            if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Android, BuildTarget.Android))
            {
                const string message = "Android Build Support (with SDK/NDK/OpenJDK and IL2CPP) is not installed for this editor. Add it via Unity Hub > Installs > Add modules.";
                Debug.LogError("[SmokeTest] " + message);
                return new BuildOutcome { Success = false, Path = outputPath, Message = message };
            }

            ApplyAndroidSettings();
            EnsureScene();

            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };

            var outputDir = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            var options = new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = outputPath,
                target = BuildTarget.Android,
                targetGroup = BuildTargetGroup.Android,
                options = BuildOptions.None,
            };

            var report = BuildPipeline.BuildPlayer(options);
            var summary = report.summary;
            var success = summary.result == BuildResult.Succeeded;

            var text = "[SmokeTest] Build " + summary.result + ": " + summary.outputPath
                       + " (" + summary.totalSize + " bytes, " + summary.totalErrors + " errors, "
                       + summary.totalWarnings + " warnings)";
            if (success)
            {
                Debug.Log(text);
            }
            else
            {
                Debug.LogError(text);
            }

            return new BuildOutcome { Success = success, Path = outputPath, Message = text };
        }
    }
}
