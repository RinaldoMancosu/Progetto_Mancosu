using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Camera_Controller : MonoBehaviour
{
    private Randomizer_controller randomizer;
    private Vector3 camera;


    [Header("Risoluzione camera")]
    [Space(5)]
    [Tooltip("x: width, y: height")]
    public Vector2Int resolution = new Vector2Int (1080, 720);


    void Start()
    {
        GameObject temp_randomizer = GameObject.Find("Randomizer");
        randomizer = temp_randomizer.GetComponent<Randomizer_controller>();
        camera = Vector3.zero;

        Camera_Resolution(new Vector2Int(resolution[0], resolution[1]));
    }


    //Imposta la risoluzione della camera
    private void Camera_Resolution(Vector2Int newResolution)
    {
        Camera my_camera = GetComponent<Camera>();
        RenderTexture rt = new RenderTexture(newResolution[0], newResolution[1], 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        my_camera.targetTexture = rt;
    }


    public Vector2Int getResolution()
    {
        return resolution;
    }


    public byte[] CamCapture()
    {
        Camera my_camera = GetComponent<Camera>();
 
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = my_camera.targetTexture;

        my_camera.Render();
 
        Texture2D Image = new Texture2D(my_camera.targetTexture.width, my_camera.targetTexture.height, TextureFormat.RGBA64, false);
        Image.ReadPixels(new Rect(0, 0, my_camera.targetTexture.width, my_camera.targetTexture.height), 0, 0);
        RenderTexture.active = currentRT;
 
        var Bytes = Image.EncodeToPNG();
        DestroyImmediate(Image);
 
        return Bytes;
    }
}
