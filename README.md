# UnityReact Test


# Overview 

1.) Create React-Native Project and build `index.android.bundle`.

2.) Create Android Application and Android Module.

3.) Create Unity Project to call JavaObject from step#2.

4.) Export Unity Project to Android Studio via Gradle.
- Under `File` -> `Build Settings` 
- Check `Export Project`
- Change `Build System` to `Gradle (New)`

# Caveat
- No support for 64 bit. Must exclude 64 bit by adding the following to `Build.gradle`.
```
		ndk {
            abiFilters "armeabi-v7a", "x86"
        }
```
- Dex counts blows up if your Unity project contains a lot of 3rd party library. React-Native library will add 5000+ more references to dex count.
- Don't use default Gradle version when import Unity Android project to Android Studio.
- If react-native library is missing dependencies, make sure to add `jcenter()` to `Build.gradle`.
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


