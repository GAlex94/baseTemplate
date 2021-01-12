using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace baseTemplate
{
    [ExecuteInEditMode]
    public class TakeScreenshot : MonoBehaviour
    {
        [SerializeField]
        private int textureWidth = 512;

        [SerializeField]
        private int textureHeight = 512;

        [SerializeField] private string TEXTURE_PATH = "Assets/Textures/";
        private bool takeHiResShot = false;
        private Camera camera;
        
        public int countScreenShot;
        void Awake()
        {
            camera = GetComponent<Camera>();
        }

        [ContextMenu("Take screen shot")]
        private void TakeHiResShot()
        {
            takeHiResShot = true;
        }


        [ExecuteInEditMode]
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                countScreenShot++;
                RenderTexture rt = new RenderTexture(textureWidth, textureHeight, 24);
                camera.targetTexture = rt;
                Texture2D screenShot = new Texture2D(textureWidth, textureHeight, TextureFormat.RGB24, false);
                camera.Render();
                RenderTexture.active = rt;
                screenShot.ReadPixels(new Rect(0, 0, textureWidth, textureHeight), 0, 0);
                camera.targetTexture = null;
                RenderTexture.active = null; // JC: added to avoid errors
                DestroyImmediate(rt);

                byte[] bytes = screenShot.EncodeToPNG();
                var path = "Assets/TakeScreenshot/screenshot_" + countScreenShot;
                System.IO.File.WriteAllBytes(path, bytes);
                Debug.Log(string.Format("Took screenshot to: {0}", TEXTURE_PATH));
               
                takeHiResShot = false;
            }
        }

        [ExecuteInEditMode]
        void LateUpdate()
        {
            if (takeHiResShot)
            {
                RenderTexture rt = new RenderTexture(textureWidth, textureHeight, 24);
                camera.targetTexture = rt;
                Texture2D screenShot = new Texture2D(textureWidth, textureHeight, TextureFormat.RGB24, false);
                camera.Render();
                RenderTexture.active = rt;
                screenShot.ReadPixels(new Rect(0, 0, textureWidth, textureHeight), 0, 0);
                camera.targetTexture = null;
                RenderTexture.active = null; // JC: added to avoid errors
                DestroyImmediate(rt);

                byte[] bytes = screenShot.EncodeToPNG();
                System.IO.File.WriteAllBytes(TEXTURE_PATH, bytes);
                Debug.Log(string.Format("Took screenshot to: {0}", TEXTURE_PATH));

                takeHiResShot = false;
            }
        }
    }
}
