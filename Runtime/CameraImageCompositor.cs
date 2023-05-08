using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CameraImageCompositor : MonoBehaviour
{
    public Camera cam = default;
    public int sqr = 512;

    private string OutPath => Path.Combine(Application.persistentDataPath, "/p.png");
    private RenderTexture rt;

    private void OnValidate()
    {
        if (cam == null) cam = GetComponent<Camera>();
    }

    [ContextMenu("CreateComposite")]
    public void Create()
    {
        MakeSquarePngFromOurVirtualThingy();
    }

    public void MakeSquarePngFromOurVirtualThingy()
    {
        
        cam.aspect = 1.0f;
        if(rt == null) rt = new RenderTexture(sqr, sqr, 24);
        cam.targetTexture = rt;
        cam.Render();
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(sqr, sqr, TextureFormat.RGB24, false);
        // false, meaning no need for mipmaps
        tex.ReadPixels(new Rect(0, 0, sqr, sqr), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        cam.targetTexture = null;
        File.WriteAllBytes(OutPath, tex.EncodeToPNG());
    }

}
