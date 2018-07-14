using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactManager : MonoBehaviour {


    void OnGUI() 
    {
        if (GUI.Button(new Rect(Screen.width/2 - 100 , Screen.height/2 - 50, 250, 200), "Launch react native activity")) {
            Debug.Log("Button clicked");

            AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");

            AndroidJavaClass reactNativeActivity = new AndroidJavaClass("com.nkemavaha.natereactnativeandroidmodule.ReactLauncher");
            reactNativeActivity.CallStatic("StartReact", currentActivity);
        }
    }

    public void OnReactResponse(string response)
    {
        Debug.LogError("OnReactResponse back message=" + response);
    }
	
}
