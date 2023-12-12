using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionV2 : MonoBehaviour
{   
    //TODO check name
    public string deviceName = "ESP32_BT";
    private bool IsConnected;
    // Start is called before the first frame update
    void Start(){
    // commented for simplicity
    /*#if UNITY_2020_2_OR_NEWER
        #if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.CoarseLocation)
          || !Permission.HasUserAuthorizedPermission(Permission.FineLocation)
          || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_SCAN")
          || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_ADVERTISE")
          || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_CONNECT"))
                    Permission.RequestUserPermissions(new string[] {
                        Permission.CoarseLocation,
                            Permission.FineLocation,
                            "android.permission.BLUETOOTH_SCAN",
                            "android.permission.BLUETOOTH_ADVERTISE",
                             "android.permission.BLUETOOTH_CONNECT"
                    });
        #endif
    #endif*/

        IsConnected = false;
        BluetoothService.CreateBluetoothObject();
        Connect();
       
    }

    // for debug purposes
    public void GetDevicesButton()
    {
       string[] devices = BluetoothService.GetBluetoothDevices();

        foreach(var d in devices)
        {
            Debug.Log(d);
        }
    }

    public void Send(string message)
    {
        if (IsConnected && (message != "" || message != null))
            BluetoothService.WritetoBluetooth(message);
        else
            BluetoothService.WritetoBluetooth("Not connected");
    }

    public void Connect()
    {
        if (!IsConnected)
        {
            IsConnected =  BluetoothService.StartBluetoothConnection(deviceName);
            BluetoothService.Toast(deviceName +" status: " + IsConnected);
        }
    } 
    public void Stop()
    {
        if (IsConnected)
        {
            BluetoothService.StopBluetoothConnection();
        }
        Application.Quit();
    }
}
