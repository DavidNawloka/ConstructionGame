using CON.BuildingGrid;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CON.Core
{
    public class ScreenCaptureSaving : MonoBehaviour
    {
        private BuildingGridMesh buildingGridMesh;

        private Canvas mainCanvas;

        private void Start()
        {
            
        }
        public IEnumerator CaptureAndSaveScreenshot(string saveFile)
        {
            mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();
            buildingGridMesh = FindObjectOfType<BuildingGridMesh>();
            mainCanvas.enabled = false;
            bool isGridMeshActive = buildingGridMesh.IsActive();
            buildingGridMesh.SetActiveMesh(false);
            yield return new WaitForEndOfFrame();
            //about to save an image capture
            Texture2D screenImage = new Texture2D(Screen.width, Screen.height);


            //Get Image from screen
            screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenImage.Apply();

            //Convert to png
            byte[] imageBytes = screenImage.EncodeToPNG();

            //Save image to file
            System.IO.File.WriteAllBytes(GetPathFromSaveFile(saveFile), imageBytes);
            mainCanvas.enabled = true;
            buildingGridMesh.SetActiveMesh(isGridMeshActive);
        }

        public Texture2D LoadScreenshot(string saveFile)
        {
            byte[] screenshotBytes = File.ReadAllBytes(GetPathFromSaveFile(saveFile));

            Texture2D screenshotTexture = new Texture2D(2, 2);
            screenshotTexture.LoadImage(screenshotBytes);
            screenshotTexture.Apply();

            return screenshotTexture;
        }
        private string GetPathFromSaveFile(string saveFile)
        {
            return Path.Combine(Application.persistentDataPath, saveFile + (".png"));
        }
    }

}