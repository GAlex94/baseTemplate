using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Показ тостов.
/// </summary>
public class ToastMessage : MonoBehaviour
{
    private static ToastMessage m_instance;
    private static ToastMessage Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<ToastMessage>();
            }
            if (m_instance == null)
            {
                m_instance = new GameObject("Auto_ToastMessage").AddComponent<ToastMessage>();
            }
            return m_instance;
        }
    }

    private static CustomToast m_customToastInstance;
    private static CustomToast CustomToastInstance
    {
        get
        {
            if (m_customToastInstance == null)
            {
                m_customToastInstance = FindObjectOfType<CustomToast>();
            }
            return m_customToastInstance;
        }
    }

    /// <summary>
    /// Вызов нативного тоста с сообщением (пока только Android).
    /// </summary>
    /// <param name="message"></param>
    public static void ShowNative(string message)
    {
        Instance.ShowToastOnUiThread(message);
    }

    /// <summary>
    /// Вызов универсального тоста с сообщением.
    /// </summary>
    /// <param name="message"></param>
    public static void ShowCustom(string message)
    {
        if (CustomToastInstance != null)
        {
            CustomToastInstance.Show(message);
        }
    }

    private string toastString;
    private AndroidJavaClass UnityPlayer;
    private AndroidJavaObject currentActivity;
    private AndroidJavaObject context;    

    void Awake()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
        }
    }

    void ShowToastOnUiThread(string toastString)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            this.toastString = toastString;
            currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(ShowToast));
        }
    }

    void ShowToast()
    {
        AndroidJavaClass Toast = new AndroidJavaClass("android.widget.Toast");
        AndroidJavaObject javaString = new AndroidJavaObject("java.lang.String", toastString);
        AndroidJavaObject toast = Toast.CallStatic<AndroidJavaObject>("makeText", context, javaString, Toast.GetStatic<int>("LENGTH_SHORT"));
        toast.Call("show");
    }
}
