using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace baseTemplate
{
    public enum ScreenLayer
    {
        None = 0,
        Bottom = 1,
        Main = 2,
        Top = 3,
        Popup = 4,
        Message = 5,
    }

    public class GUIController : Singleton<GUIController>
    {
        public RectTransform CanvasRectTransform = null;

        [SerializeField] private GameObject screensRoot = null;
     
        [SerializeField] private GUIScreen[] currentScreens = new GUIScreen[0];

        [SerializeField] private GameObject[] screenPrefabs = new GameObject[0];

        private List<GUIScreen> screens;

        private void Awake()
        {
            Instance = this;
            screens = new List<GUIScreen>();
        }

        private void Start()
        {
            foreach (var curPrefab in screenPrefabs)
            {
                GameObject curScreen = Instantiate(curPrefab);
                curScreen.transform.SetParent(screensRoot != null ? screensRoot.transform : gameObject.transform, false);

                GUIScreen guiScreen = curScreen.GetComponent<GUIScreen>();
                screens.Add(guiScreen);
            }

            foreach (var curScreen in screens)
            {
                curScreen.gameObject.SetActive(false);
            }

            foreach (var curScreen in currentScreens)
            {
                screens.Add(curScreen);
                curScreen.Show();
            }
        }

        public T FoundScreen<T>() where T: GUIScreen
        {
            foreach (var curScreen in screens)
            {
                if (curScreen.GetType() == typeof(T))
                    return curScreen as T;
            }
            return null;
        }

        public GUIScreen FoundScreen(Type typeScreen)
        {
            foreach (var curScreen in screens)
            {
                if (curScreen.GetType() == typeScreen)
                    return curScreen;
            }
            return null;
        }

        public void ShowScreen<T>(bool hideAll = false) where T : GUIScreen
        {
            GUIScreen foundScreen = FoundScreen<T>();
            if (foundScreen != null)
            {
                ShowScreen(foundScreen, hideAll);
            }
            else Debug.LogWarning("Screen " + typeof(T) + " not found!");
        }

        public void ShowScreen(GUIScreen screen, bool hideAll = false)
        {
            if (hideAll)
            {
                foreach (var curScreen in screens)
                {
                    if (curScreen.GetType() != screen.GetType() && curScreen.IsShowed)
                        curScreen.Hide();
                }
            }

            screen.Show();
            SortByLayer();
        }

        public void HideAll()
        {
            foreach (var curScreen in screens)
            {
                curScreen.Hide();
            }

            SortByLayer();
        }

        public void HideScreen<T>() where T : GUIScreen
        {
            GUIScreen foundScreen = FoundScreen<T>();
            if (foundScreen != null)
            {
                foundScreen.Hide();
                SortByLayer();
            }
            else
            {
                Debug.LogWarning("Screen " + typeof(T) + " not found!");
            }
        }

        public void HideScreen(GUIScreen screen)
        {
            screen.Hide();
            SortByLayer();
        }

        private void SortByLayer()
        {
            screens.Sort((screen, guiScreen) =>
            {
                float leftScreen = (float)screen.ScreenLayer + screen.OffsetZ / 100.0f;
                float rightScreen = (float)guiScreen.ScreenLayer + guiScreen.OffsetZ / 100.0f;
                return leftScreen.CompareTo(rightScreen);
            });

            for (int i = 0; i < screens.Count; i++)
            {
                screens[i].transform.SetSiblingIndex(i);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                var screenLast = screens.LastOrDefault(s => s.IsShowed && !s.IgnoreLastScreen);
                if (screenLast != null)
                {
                    screenLast.BackButton();
                }
                else
                {
                    ShowScreen<ScreenMessageExit>();
                }
            }
        }
    }
}
