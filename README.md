# UnityReactTest

This project is basically an exploratory investigation to see if it is possible to integrate `React` to Unity Android App.


# How-to-Guide (High-level)

Follow https://facebook.github.io/react-native/docs/integration-with-existing-apps for step 1 to step 3.

## 1.) Create React-Native Project.


## 2.) Build `index.android.bundle` from your React-Native Project. This is basically what Android Application will use for rendering your React-Native project.
Think of it as your React executable file.
Run this script:
```
sh build.sh
```

It is essentially running the following command to build `index.android.bundle` with additional steps of copy the bundle file your project folder.
```
react-native bundle --platform android --dev false --entry-file index.js --bundle-output android/app/src/main/assets/index.android.bundle --assets-dest android/app/src/main/res
```

Everytime you've made a change to your React-Native project. You will need to run this command in order to have your changes reflected.


## 3.) Create Android Application to check if your React works properly.
If you're unable to use or import `com.facebook.react.ReactRootView` and `com.facebook.react.ReactInstanceManager`,
it is bacause `Maven` is unable to find the latest ReactNative package. 

Please check the followings:

### 3.1) Ensure your path to your local `react-native/android` is correct.
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

### 3.2) DO NOT DO THIS in `build.gradle`. 
```
dependencies {
	...
    compile "com.facebook.react:react-native:+" // From node_modules
}
```

** DO THIS INSTEAD, Specify `react-native` version to use. **

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

## 5.) Export your Android Module as `AAR`.

Once you reach this, Congratulations!
Now the most challenging part,

## 6.) Create Unity Project to communicate to Android Module.
- Create a button in Unity, which is basically going to call the static method from step#4 via JavaObject and JavaClassObject
i.e.
```
AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
AndroidJavaObject currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");

AndroidJavaClass reactNativeActivity = new AndroidJavaClass("YOUR_PACKAGE_NAME.YOUR_MODULE_JAVA_CLASS");
reactNativeActivity.CallStatic("YOUR_STATIC_METHOD", currentActivity);

```

## 7.) Copy your Android Module `AAR` file to `Assets/Plugins/Android` folder.

## 8.) Export Unity project to Android Gradle project. 
https://docs.unity3d.com/Manual/android-gradle-overview.html

We need to do this because of the followings:
- `React` does not support 64 bit.
- Avoid exceed the limit of Android dex counts.
- Unity uses old `Gradle` version, which lacks some of the functionality we may need (i.e. ability to exclude 64 bit).

There are 2 ways to do this:

8.1) Use `Gradle` option from Unity's `Build Settings`
- Under `File` -> `Build Settings` 
- Check `Export Project`
- Change `Build System` to `Gradle (New)`
- Click Export

OR

(preferably)

## 8.2) Create a custom Build/export option.

### 8.2.1) Create `mainTemplate.gradle` in `Assets/Plugins/Android` folder.

### 8.2.2) Create `Build.cs` to custom build/export your Unity Android project.
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


## 9.) Your Unity's Android Gradle Project, open it with Android studio, but do NOT use the default `gradle` setting suggested by Android Studio.

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


#### 9.1.7) Build `Gradle Project`.

### 9.2) Copy over external dependencies to `libs` folder.
- Your android module `aar`.
- `react-native-0.56.0.aar` from your `React-Native` project.

### 9.3) Copy `index.android.bundle` to `src/main/assets/` folder.

### 9.3) Build `Make Project` in Android Studio.

### 9.4) Deploy to Android Device. 



# Caveat
- No support for 64 bit. Must exclude 64 bit by adding the following to `Build.gradle`.

- Dex counts will blow up if your Unity project contains a lot of 3rd party library. React-Native library will add at least 5000+ more to dex count.
- Don't use default Gradle version when import Unity Android project to Android Studio. Otherwise, you may get into dependencies hell.
- If react-native library is missing dependencies, make sure to add `jcenter()` to `Build.gradle`.



