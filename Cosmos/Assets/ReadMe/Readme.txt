Configure between FlatScreen and VR

* Dont use PlatformManager.cs as it will create problem in script execution order.

* Activate each game objects in each scene manually that are platform specifics.

* PlatformConfigSO data need to be set to platform specific.

* In Startup Scene, SceneLoaderNO is having reference of both FlatScreen ans well as VR specific ClientLoadingScreen.cs, 
		the right one is getting referenced during runtime at awake as per the data of PlatformConfigSO

* In Char Select Scene, don't change the ClientCharacterSelectState game object to separate game objects containing ClientCharacterSelectState.cs and 			ServerCharacterSelectState.cs