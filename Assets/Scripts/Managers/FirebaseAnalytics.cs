/*using Firebase;*/
using UnityEngine;

namespace baseTemplate
{
    public class FirebaseAnalytics : Singleton<FirebaseAnalytics>
    {
    //    private FirebaseApp firebaseApp;

        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        void Start()
        {
            Init();
        }

        private void Init()
        {
            Debug.LogError("FirebaseAnalytics is not Initial (code commented)");

            /*           Debug.Log("Init FirebaseAnalytics");
                       Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
                       {
                           var dependencyStatus = task.Result;
                           Debug.Log("FirebaseAnalytics" + dependencyStatus);
                           if (dependencyStatus == Firebase.DependencyStatus.Available)
                           {
                               firebaseApp = FirebaseApp.DefaultInstance;
                               Firebase.Analytics.FirebaseAnalytics.LogEvent("INIT_FIREBASE");
                               Debug.Log("firebaseApp" + firebaseApp);
                           }
                           else
                           {
                               UnityEngine.Debug.LogError(System.String.Format(
                                   "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                               // Firebase Unity SDK is not safe to use here.
                           }

                           Debug.Log("FirebaseAnalytics id= " + task.Id);
                       });*/
        }
    }
}