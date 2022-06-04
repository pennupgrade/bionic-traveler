namespace BionicTraveler
{
    using System.IO;
    using UnityEngine;

    public class Screenshotter : MonoBehaviour
    {
        public RenderTexture renderTexture;

        void Update()
        {
            // Here we take screenshots when the player hits the S key, but it could
            // just as well have been a button click, time elapsing, or some other
            // condition.
            if (Input.GetKeyDown(KeyCode.S))
            {
                TakeScreenshot();
            }
        }

        public void TakeScreenshot()
        {
            // Force a render to the target texture.
            Camera.main.targetTexture = renderTexture;
            Camera.main.Render();

            // Texture.ReadPixels reads from whatever texture is active. Ours needs to
            // be active. But let's remember the old one so we can restore it later.
            RenderTexture oldRenderTexture = RenderTexture.active;
            RenderTexture.active = renderTexture;

            // Grab ALL of the pixels.
            Texture2D raster = new Texture2D(renderTexture.width, renderTexture.height);
            raster.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            raster.Apply();

            // Write them to disk. Change the path and type as you see fit.
            File.WriteAllBytes("screenshot.png", raster.EncodeToJPG());

            // Restore previous settings.
            Camera.main.targetTexture = null;
            RenderTexture.active = oldRenderTexture;

            Debug.Log("Screenshot saved.");
        }
    }
}
