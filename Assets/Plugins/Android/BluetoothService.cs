using System;
using UnityEngine;
namespace Plugins.Android {
  public class BluetoothService {

    private static AndroidJavaClass _unityPlayer;
    private static AndroidJavaObject _activity;
    private static AndroidJavaObject _context;
    private static AndroidJavaClass _unity3dbluetoothplugin;
    private static AndroidJavaObject _bluetoothConnector;


    // creating an instance of the bluetooth class from the plugin 
    public static void CreateBluetoothObject() {
      if (Application.platform != RuntimePlatform.Android) return;
      _unityPlayer = new("com.unity3d.player.UnityPlayer");
      _activity = _unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
      _context = _activity.Call<AndroidJavaObject>("getApplicationContext");
      _unity3dbluetoothplugin = new("com.example.unity3dbluetoothplugin.BluetoothConnector");
      _bluetoothConnector = _unity3dbluetoothplugin.CallStatic<AndroidJavaObject>("getInstance");
    }

    public static string[] GetBluetoothDevices() {
      if (Application.platform != RuntimePlatform.Android) return null;
      try {
        return _bluetoothConnector.Call<string[]>("GetBluetoothDevices");
      } catch (Exception _) {
        Toast("No Device found");
        return null;
      }
    }

    // starting bluetooth connection with device named "DeviceName"
    // print the status on the screen using native android Toast
    public static bool StartBluetoothConnection(string deviceName) {
      if (Application.platform != RuntimePlatform.Android) return false;
      try {
        var connectionStatus = _bluetoothConnector.Call<string>("StartBluetoothConnection", deviceName);
        Toast("Start connection status: " + connectionStatus);
        if (connectionStatus == "Connected")
          return true;
      } catch (Exception) {
        Toast("Start connection error");
      }

      return false;
    }


    // should be called inside OnApplicationQuit
    // stop connection with the bluetooth device
    public static void StopBluetoothConnection() {
      if (Application.platform != RuntimePlatform.Android) return;
      try {
        _bluetoothConnector.Call("StopBluetoothConnection");
        Toast("Connection stoped");
      } catch (Exception) {
        Toast("Stop connection error");
      }
    }

    // write data as a string to the bluetooth device
    public static void WriteToBluetooth(string data) {
      if (Application.platform != RuntimePlatform.Android) return;
      try {
        _bluetoothConnector.Call("WriteData", data);
      } catch (Exception) {
        Toast("Write data error");
      }
    }


    //read data from the bluetooth device
    // if there is an error or there is no data coming, this method will return "" as an output
    public static string ReadFromBluetooth() {
      if (Application.platform != RuntimePlatform.Android) return "";
      try {
        return _bluetoothConnector.Call<string>("ReadData");
      } catch (Exception e) {
        _bluetoothConnector.Call("PrintOnScreen", _context, "Read data error");
      }

      return "";
    }

    private static void Toast(string data) {
      _bluetoothConnector.Call("PrintOnScreen", _context, data);
      Debug.Log(data);
    }
  }
}
