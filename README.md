# UnityReactTest

This project is basically an exploratory investigation to see if it is possible to integrate `React` to Unity Android App.

`Last Update: 07/15/2018`

# High-Level Summary of what it does
The idea is to build `React` project and integrate into `Android` module (i.e. `aar` or `jar`).
Then Unity will invoke `React` component via the `Android` module we just created.

To invoke `React` component from within `Unity` app, `Unity` project will need to be exported as a `Gradle` project due to the issue of `React` does not support  64-bit and if your existing Unity project is large and has integrated a bunch of 3rd party library, you may have exceeded `DEX` counts limit by just adding `React-Native` library. These can only be resolved via a custom `build.gradle`.


# What you need
- Unity 5.6.5p1
- Node installed via Homebrew
- React-Native version 0.56.0
- Android Studio
- Android SDK from version 16 to the latest one
- Android Build Tools at least version 25 (Recommended 27)

# Where things are.
```
ReactAndroid							// React Project
.../ReactProject
.../.../native_android					// Android App and Module projects for React
.../.../android/EXPORT_PROJECT_NAME		// Exported Unity to Android Project
UnityClient								// Unity Project
.../Assets/Plugins/Android/				// Android Plugins and Unity's custom gradle template
```


# How-to-Guide (High-level)

Follow https://facebook.github.io/react-native/docs/integration-with-existing-apps for step 1 to step 3.

## 1.) Create React-Native Project.

If using this `React-Native` from this repo (`ReactAndroid/ReactProject`), run the following command to reconstruct `android` folder:
```
react-native upgrade
```

## 2.) Build `index.android.bundle` from your `React-Native` Project. 
This is basically what Android Application/module will use for rendering your React-Native project.
Think of it as your React executable file.

Run this script:
```
sh build.sh
```

It is essentially equivalent to running the following command to build `index.android.bundle` with additional steps of copy the bundle file your project folder.
```
react-native bundle --platform android --dev false --entry-file index.js --bundle-output android/app/src/main/assets/index.android.bundle --assets-dest android/app/src/main/res
```

Everytime you've made a change to your React-Native project. You will need to run this command in order to have your changes reflected.


## 3.) Create Android Application and check if your `React` was integrated and works properly.
If you're unable to use or import `com.facebook.react.ReactRootView` and `com.facebook.react.ReactInstanceManager`,
it is bacause `Maven` is unable to find `React-Native` package version we need (see *3.2*).

Please check the followings:

### 3.1) Ensure your path to your local `react-native/android` is correct.
```
allprojects {
    repositories {
        maven {
            // All of React Native (JS, Android binaries) is installed from npm
            url "$rootDir/../node_modules/react-native/android"
        }
        ...
    }
    ...
}
```

### 3.2) DO *NOT* DO THIS in `build.gradle`. 
```
dependencies {
	...
    compile "com.facebook.react:react-native:+" // From node_modules
}
```

** *DO* THIS INSTEAD, Specify `react-native` version to use. **

```
dependencies {
	...
    compile "com.facebook.react:react-native:0.56.0" // From node_modules
}
```

### 3.3) Rebuild your Android project

If you use `gradle` command,
```
./gradlew clean build
```


## 4.) Create Android Module.
Once step#3 works and there is no issue, 
- Create a new Android Module Project.
- Modify `build.gradle` to include `React-native` as dependencies, should be similar to step#1 to step#3.
- Move all React related activity to this module.
- Create a static method to launch `React` activity.
- Add this module as a dependency for your Android Project in step#3.
- Test your Android Project from step#3 to see if it still works.

## 5.) Export your Android Module as `aar`.

Once you reach this, Congratulations!

## Now the most challenging part,

## 6.) Create Unity Project to communicate to Android Module.
- Create a button in Unity, which is basically going to call the static method from step#4 via JavaObject and JavaClassObject
i.e.
```
AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
AndroidJavaObject currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");

AndroidJavaClass reactNativeActivity = new AndroidJavaClass("YOUR_PACKAGE_NAME.YOUR_MODULE_JAVA_CLASS");
reactNativeActivity.CallStatic("YOUR_STATIC_METHOD", currentActivity);

```

## 7.) Copy your Android Module `aar` file to `Assets/Plugins/Android` folder.
This will get copied over to `Gradle` project once exported.

## 8.) Export Unity project to Android Gradle project. 
https://docs.unity3d.com/Manual/android-gradle-overview.html

We need to do this because of the followings:
- `React` does NOT support 64 bit.
- Avoid exceed the limit of Android dex counts.
- Unity uses old `Gradle` version, which lacks some of the functionality we may need (i.e. ability to exclude 64 bit).

### There are 2 ways to do this:

### 8.1) Use `Gradle` option from Unity's `Build Settings`
- Under `File` -> `Build Settings` 
- Check `Export Project`
- Change `Build System` to `Gradle (New)`
- Click Export

OR

(preferably)

### 8.2) Create a custom Build/export option.

#### 8.2.1) Create `mainTemplate.gradle` in `Assets/Plugins/Android` folder.

#### 8.2.2) Create `Build.cs` to custom build/export your Unity Android project.
```
	[MenuItem("Build/Android Project %g", false, 1)]
	public static void BuildAndroid()
	{
		...
		// Export as Gradle/Android project
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;

        BuildOptions options = BuildOptions.AcceptExternalModificationsToPlayer;

        string status = BuildPipeline.BuildPlayer (
            GetEnabledScenes(),
            path,	// This should point to 'YOUR_REACT_NATIVE_PROJECT/android'.
            BuildTarget.Android,
            options
        );

        return status;
    }
``` 


This will create your Unity's Android Gradle project. 

NOTE: It should reside in your React-Native project's `android` folder due to recommendation from Facebook:
```
To ensure a smooth experience, create a new folder for your integrated React Native project, then copy your existing Android project to an /android subfolder.
```

NOTE: Please make sure to be able to build and launch your APK successfully (minus `React` part). Otherwise, we would have a non-functioning `Gradle` project.


## 9.) Your Unity's Android Gradle Project, open it with Android studio, 
but ** DO NOT ** use the default `gradle` setting suggested by Android Studio.

This is located in `ReactAndroid/ReactProject/android/UnityReactTest`. It was exported from Unity.

This is basically Unity's `Android` project building with `Gradle`.

### 9.1) Update `build.gradle` with the following options:

#### 9.1.1) Enable MultiDex to avoid Dex counts blowing up under `defaultConfig` section.
```
	multiDexEnabled true
```

#### 9.1.2) Exclude 64 bit under `defaultConfig` section.
```
	ndk {
        abiFilters "armeabi-v7a", "x86"
     }
```


#### 9.1.3) Add `React-Native` dependencies.
```
	dependencies {
		...
		compile 'com.facebook.react:react-native:0.56.0'
	}
```

#### 9.1.4) Add `Android` dependencies.
```
	dependencies {
		...
		compile 'com.android.support:appcompat-v7:27.1.1'
		compile 'com.android.support.constraint:constraint-layout:1.1.2'
	}
```


#### 9.1.5) Apply `react.gradle`.
```
	project.ext.react = [
		entryFile: "index.js"
	]

	apply from: "../../node_modules/react-native/react.gradle"

```


#### 9.1.6) Apply `jcenter()` under `respositories` of `allprojects` and `buildscript` sections to ensure `React-Native` package has all required dependencies.
```
buildscript {
	repositories {
		maven { url 'https://maven.google.com' }
		jcenter()
	}

allprojects {
   repositories {
   		...
	   jcenter()
   }
}
```

This ensures `React-Native` package has all the required dependencies.


#### 9.1.7) Build `Gradle Project` to see there is no error.

### 9.2) Copy over external dependencies to `libs` folder from your `React-Native` project folder.
- Your android module `aar` from `ReactAndroid/ReactProject/native_android/YOUR_MODULE_NAME/builds/outputs/YOUR_MODULE.ARR`
- `react-native-0.56.0.aar` from your `ReactAndroid/ReactProjet/node_modules/react-native/android/com/facebook/react/react-native/0.56.0/` project.

NOTE: `node_modules` folder was generated after you create `React-Native` project via `react-native` command, run `npm install`, or run `react-native run-android`.

### 9.3) Copy `index.android.bundle` to `ReactAndroid/ReactProject/android/UnityReactTest/src/main/assets/` folder.

### 9.3) Build `Make Project` in Android Studio.

### 9.4) Deploy to Android Device either from Android studio or `adb install` command.

If you've set things up correctly, you'd be able to trigger `React` project from within `Unity`.



# Caveat/Troubleshooting
- No support for 64 bit. Must exclude 64 bit by modify `build.gradle`.
- Dex counts will blow up if your Unity project contains a lot of 3rd party library. React-Native library will add at least 5000+ more to dex count.
- Don't use default Gradle version when import Unity Android project to Android Studio. Otherwise, you may get into dependencies hell.
- If react-native library is missing dependencies, make sure to add `jcenter()` to `build.gradle`.

# Author
Nate Kemavaha

# References
## Thank you for all contributors to these references. Without them, we wouldn't have this project working.
- Could not find dependency: https://stackoverflow.com/questions/41570435/gradle-error-could-not-find-com-android-tools-buildgradle2-2-3
- Troubleshooting Android Guide: http://shobhitsamaria.com/troubleshooting-unity-build-android-platform/
- Unable to load `index.android.bundle`: https://stackoverflow.com/questions/44446523/unable-to-load-script-from-assets-index-android-bundle-on-windows
- Tip for how to change `index.android.js` entry: https://github.com/facebook/react-native/issues/3442
- Unity-React Project from `marijn` for a different approach to `Unity` and `React`: https://github.com/marijnz/unity-react
- How to include external `aar` using Gradle: https://stackoverflow.com/questions/16682847/how-to-manually-include-external-aar-package-using-new-gradle-android-build-syst
- Unity3D and React-Native Troubleshooting List: https://answers.unity.com/questions/1083233/unity3d-react-native.html
- Export and running Unity3D to Android Studio: https://stackoverflow.com/questions/38980792/exporting-and-running-unity3d-project-to-android-studio
- Proguard and how to check dependencies: https://stackoverflow.com/questions/43496103/prograurd-duplicate-zip-entry
- Resolve could not find `appcompat-v7`: https://github.com/react-community/lottie-react-native/issues/203
- Connecting the dots between `react-native` and `Unity3D`: https://medium.com/codeexplorers/connecting-the-dots-between-react-native-and-unity-3d-using-gradle-67f93b92c254
- Too many field references (aka `DEX` limit): https://stackoverflow.com/questions/42582850/too-many-field-references-70613-max-is-65536
- When to use `gradle.properties` vs `settings.gradle`: https://stackoverflow.com/questions/45387971/when-to-use-gradle-properties-vs-settings-gradle
- `React-Native` Android app as a Module: http://www.ard.ninja/blog/react-native-android-app-as-a-module/
- Yes it is possible to Show `Unity3D` in React (different approach): https://medium.com/@beaulieufrancois/show-unity3d-view-in-react-native-application-yes-its-possible-852923389f2d
- Crash after requesting `React-Native` Bundle :https://github.com/facebook/react-native/issues/10925
- `aar` file doesn't include transitive dependencies: http://www.riptutorial.com/android-gradle/example/10329/the-aar-file-doesn-t-include-the-transitive-dependencies
- `android:keyboardNavigationCluster` not found: https://stackoverflow.com/questions/45301203/no-resource-found-that-matches-the-given-name-attr-androidkeyboardnavigationc
- `React-Native` deployment at `WalmartLabs`: https://medium.com/walmartlabs/react-native-at-walmartlabs-cdd140589560
- `gradle` duplicate entry/collide dependencies when enable `multidex`: https://stackoverflow.com/questions/28168063/gradle-duplicate-entry-java-util-zip-zipexception
- `zipException` duplicate entry: http://gurusurend.com/zipexception-duplicate-entry-android-studio/
