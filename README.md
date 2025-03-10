# Screen Locker Manager
Simple Screen Lock Manager for Waiting, Loading, or Scene Switching

**Note:** To run the example from this repository, you need to install **DoTween** and **Zenject (Extenject).**

---

## How to install
Select one of the following methods:

1. From Unity package.<br/>Select latest release from the https://github.com/vcow/lib-screen-locker/releases and download __screen-locker.unitypackage__ from Assets section.

2. From Git URL.<br/>Go to __Package Manager__, press __+__ in the top left of window and select __Install package from git URL__. Enter the URL below:
```
https://github.com/vcow/lib-screen-locker.git#upm
```
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;or
```
https://github.com/vcow/lib-screen-locker.git#2.0.0
```
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;if you want to install exactly 2.0.0 version.

3. From OpenUPM.<br/>Go to __Edit -> Project Settings -> Package Manager__ and add next scoked registry:
* __Name__: package.openupm.com
* __URL__: https://package.openupm.com
* __Scope(s)__: com.vcow.screen-locker

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Press __Save__, then go to __Package Manager__ and install __Scene Select Tool__ from the __My Registries -> package.openupm.com__ section.

4. Add to the ```manifest.json```.<br/>Open ```mainfest.json``` and add next string to the ```dependencies``` section:
```
{
  "dependencies": {
    "com.vcow.screen-locker": "https://github.com/vcow/lib-screen-locker.git#upm",
    ...
  }
}
```
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;or
```
{
  "dependencies": {
    "com.vcow.screen-locker": "https://github.com/vcow/lib-screen-locker.git#2.0.0",
    ...
  }
}
```
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;if you want to install exactly 2.0.0 version.

---

## Setup
Setting up the Manager involves creating a list of screen lockers. The screen locker settings can be implemented in any convenient way but must implement the `IScreenLockerSettings` interface. An instance of the settings must be provided to the Manager as a constructor argument upon creation.

---

## Screen Lockers
All screen lockers must inherit from the base class `BaseScreenLocker` and have a **unique key**. In addition to the key, the following methods must be implemented:

```csharp
public abstract void Activate(object[] args = null, bool immediately = false);
```
This method is called when the screen lock starts. It can be used to implement a scene hiding animation.

**Important:** The Manager expects that `Activate()` changes the locker’s `State` to `ScreenLockerState.ToActive`, and then to `ScreenLockerState.Active` upon completion of the hiding process—or directly to `ScreenLockerState.Active` if the hiding occurs instantly without effects.

Additional arguments (`args`) may be passed to the locker during activation, and their processing depends entirely on the specific implementation.

```csharp
public abstract void Deactivate(bool immediately = false);
```
This method is called when the screen unlock starts. It can be used to implement a scene opening animation.

**Important:** The Manager expects that `Deactivate()` changes the locker’s `State` to `ScreenLockerState.ToInactive`, and then to `ScreenLockerState.Inactive` upon completion of the opening process—or directly to `ScreenLockerState.Inactive` if the opening occurs instantly without effects.

```csharp
public abstract void Force();
```
This method is called in exceptional cases to **immediately** complete the hiding or opening of the screen by the locker.

---

## Usage
Using the Manager is as simple as calling `Lock()` and `Unlock()`. You can also check whether any screen lock is active by reading the `IsLocked` flag.

```csharp
void Lock(string key, Action completeCallback = null, object[] args = null);
```
Locks the scene using the screen locker with the specified **key**. Additional arguments may be passed to the locker. The `completeCallback` is called when the scene is fully hidden.

```csharp
void Unlock(string key = null, Action<string> completeCallback = null);
```
Unlocks the scene. You can explicitly specify the **key** of the locker to be removed. If no key is specified, all currently active screen lockers will be deactivated. The `completeCallback` is called when the scene is fully revealed.

---

## Adding New Lockers at Runtime
The Manager allows **registering or re-registering** screen lockers during runtime. To register a new screen locker at runtime, use:

```csharp
void SetScreenLocker(ScreenLockerBase screenLockerBasePrefab);
```

**Note:** If the newly added screen locker has the same key as a previously added one, the latter will be **overwritten**.

---

## InstantiateScreenLockerHook
If a screen locker requires additional initialization upon creation, you can use the `InstantiateScreenLockerHook`. This hook is passed to the Manager’s constructor and is invoked **each time** a screen locker is instantiated.

