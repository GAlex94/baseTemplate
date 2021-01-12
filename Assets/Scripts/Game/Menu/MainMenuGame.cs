using UnityEngine;

namespace baseTemplate
{
    public class MainMenuGame : Singleton<MainMenuGame>, IGame
    {
        public void StartGame()
        {
            var fakeLoading = GUIController.Instance.FoundScreen<ScreenFakeLoading>();
            fakeLoading.Init(() =>
            {
                if (AdmobController.Instance != null)
                {
                    if (!PlayerPrefs.HasKey("IS_FIRST_MENU"))
                    {
                        PlayerPrefs.SetInt("IS_FIRST_MENU", 1);
                        AdmobController.Instance.IsStartShow = true;
                    }
                    else
                    {
                        if (!AdmobController.Instance.IsStartShow)
                        {
                            AdmobController.Instance.IsStartShow = true;
                            AdmobController.Instance.ShowStart();
                        }
                        else
                        {
                            //AdmobController.Instance.Show(0, false);
                        }
                    }
                }
            });

           /* var screenMenu = GUIController.Instance.FoundScreen<ScreenMenu>();
            if(screenMenu != null)
                GUIController.Instance.ShowScreen(screenMenu);*/
        }
    }
}