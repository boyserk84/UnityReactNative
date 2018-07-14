using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactManager : MonoBehaviour {


    void OnGUI() {
        if (GUI.Button(new Rect(10, 70, 200, 30), "Launch react native activity")) {
            Debug.Log("Button clicked");

            AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");

            AndroidJavaClass reactNativeActivity = new AndroidJavaClass("com.nkemavaha.react.ReactNativeActivity");
            reactNativeActivity.CallStatic("Launch", currentActivity);
        }
    }

    public void OnReactResponse(string response)
    {
        Debug.LogError("OnReactResponse back message=" + response);
    }
	
}
