using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {
    [SerializeField]
    private Transform target;
    private float xOffset = 20;

    [Header ("ScreenShot")]
    [SerializeField]
    private int captureWidth = 2048;
    [SerializeField]
    private int captureHeight = 2048;
    [SerializeField]
    private RawImage rawImage;

    [Header ("File Browser")]
    public FileManager fileManager;

    private Camera camera;
    private Texture2D currentImage;
    private bool takeScreenShot;

    private string path;

    // Start is called before the first frame update
    void Start () {
        camera = Camera.main;
        rawImage.gameObject.SetActive (false);
    }

    // Update is called once per frame
    void Update () {
        if (target) {
            Quaternion OriginalRot = transform.rotation;
            transform.LookAt (target);
            Quaternion NewRot = Quaternion.Euler (transform.rotation.eulerAngles.x - xOffset, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
            transform.rotation = OriginalRot;
            transform.rotation = Quaternion.Lerp (transform.rotation, NewRot, 2 * Time.deltaTime);
        }

        if (Input.GetKeyDown (KeyCode.Return)) {
            TakeScreenShot ();
        } else if (Input.GetKeyDown (KeyCode.Space)) {
            StartCoroutine (OpenFileBrowser ());
        }
    }

    void OnPostRender () {
        if (takeScreenShot) {
            takeScreenShot = false;
            RenderTexture renderTexture = camera.targetTexture;
            Texture2D texture = new Texture2D (renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            Rect rect = new Rect (0, 0, renderTexture.width, renderTexture.height);
            texture.ReadPixels (rect, 0, 0);

            fileManager.SavePNG (texture.EncodeToPNG ());
            rawImage.gameObject.SetActive (true);
            rawImage.texture = fileManager.LoadPNG ();

            RenderTexture.ReleaseTemporary (renderTexture);
            camera.targetTexture = null;
        }
    }

    void TakeScreenShot () {
        camera.targetTexture = RenderTexture.GetTemporary (captureWidth, captureHeight);
        takeScreenShot = true;
    }

    public IEnumerator OpenFileBrowser () {
        // PREVENT SPACEBAR INPUT CLOSE DIALOG
        yield return new WaitForSecondsRealtime (0.2f);
        fileManager.FolderBrowserPanel ("Change Save location");
    }
}