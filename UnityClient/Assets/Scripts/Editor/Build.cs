using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class Build : MonoBehaviour 
{
    /// <summary>
    /// Project path
    /// </summary>
    static readonly string projectPath = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));

    /// <summary>
    /// Unity Build APK Path
    /// </summary>
    static readonly string apkPath = projectPath + "/bin/UnityReactTest.apk";

    /// <summary>
    /// Exported Android project path.
    /// </summary>
    static readonly string exportAndroidProjectPath = Path.GetFullPath(Path.Combine(projectPath, "..")) + "/ReactAndroid/ReactProject/android";

    /// <summary>
    /// React Export Path
    /// </summary>
    static readonly string exportPath = Path.GetFullPath(Path.Combine(projectPath, "..")) + "/ReactAndroid/ReactProject/Export";

    [MenuItem("Build/Android as Project %g", false, 2)]
    public static void DoBuildAsDependency()
    {
        if (Directory.Exists(exportAndroidProjectPath))
        {
            UnityEngine.Debug.Log(string.Format("Deleting ExportAndroidProjectFolder={0}", exportAndroidProjectPath));
            Directory.Delete(exportAndroidProjectPath, true);
        }

        string result = BuildAndroid(exportAndroidProjectPath);
            
        if (!string.IsNullOrEmpty(result))
            throw new Exception("Build failed: " + result);

        //UnityEngine.Debug.Log("Copy buildPath to exportPath");
        //Copy(Path.Combine(buildPath, "src"), Path.Combine(exportPath, "src"));
        //Copy(Path.Combine(buildPath, "libs"), Path.Combine(exportPath, "libs"));

        //BuildGradle();
        UnityEngine.Debug.Log(string.Format("Done export Unity Project to Android project path={0}", exportAndroidProjectPath));
    }

    [MenuItem("Build/Android APK %g", false, 1)]
    public static void DoBuildAndroidAPK()
    {
        UnityEngine.Debug.Log(string.Format("Checking All Paths...\nProjectPath={0} \nAndroidAPKPath={1} \nexportPath={2}", projectPath, apkPath,exportPath));

        if (File.Exists(apkPath))
        {
            UnityEngine.Debug.Log(string.Format("Deleting apkFile={0}", apkPath));
            File.Delete(apkPath);
        }

        string result = BuildAndroid(apkPath, false);

        if (!string.IsNullOrEmpty(result))
            throw new Exception("Build failed: " + result);

        UnityEngine.Debug.Log(string.Format("Done AndroidAPK path={0}", apkPath));

    }

    /// <summary>
    /// Helper method to Builds Android.
    /// </summary>
    /// <returns>The AP.</returns>
    public static string BuildAndroid(string path, bool externalBuild=true) 
    {
        UnityEngine.Debug.Log(string.Format("Building Android APK to Path={0}", apkPath));

        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;

        BuildOptions options = BuildOptions.AcceptExternalModificationsToPlayer;

        if (externalBuild == false)
        {
            options = BuildOptions.Development;
        }

        string status = BuildPipeline.BuildPlayer (
            GetEnabledScenes(),
            path,
            BuildTarget.Android,
            options
        );

        return status;
    }

    /// <summary>
    /// Gets all the enabled scenes.
    /// </summary>
    /// <returns>The enabled scenes.</returns>
    static string[] GetEnabledScenes()
    {
        var scenes = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => s.path)
            .ToArray();

        return scenes;
    }

    /// <summary>
    /// Helper method to Copy the specified source and destinationPath.
    /// </summary>
    /// <param name="source">Source.</param>
    /// <param name="destinationPath">Destination path.</param>
    static void Copy(string source, string destinationPath)
    {
        if(Directory.Exists(destinationPath))
            Directory.Delete(destinationPath, true);

        Directory.CreateDirectory(destinationPath);

        foreach (string dirPath in Directory.GetDirectories(source, "*",
            SearchOption.AllDirectories))
            Directory.CreateDirectory(dirPath.Replace(source, destinationPath));

        foreach (string newPath in Directory.GetFiles(source, "*.*",
            SearchOption.AllDirectories))
            File.Copy(newPath, newPath.Replace(source, destinationPath), true);
    }

    static void BuildGradle()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "sh",
                Arguments = "build.sh",
                UseShellExecute = false,
                WorkingDirectory = projectPath,
                RedirectStandardError = true,
            };

        var process = new Process { StartInfo = startInfo};
        process.Start();
        process.WaitForExit(300000);
        AssetDatabase.Refresh();

        UnityEngine.Debug.LogFormat("Built dependencies (exit code: {0})", process.ExitCode);
        var errorOutput = process.StandardError.ReadToEnd().Trim();
        if(!string.IsNullOrEmpty(errorOutput))
            UnityEngine.Debug.LogWarningFormat("Warnings and potential errors:\n" + errorOutput);
    }
}
