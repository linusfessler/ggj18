using UnityEngine;

[ExecuteInEditMode]
public class ConsoleEffect : MonoBehaviour
{

    [Range(0, 1)]
    public float intensity = 0;
    private float lastFrameIntensity;
    public Material CrtLines;
    public Material Noise;
    public Material BarrelDistort;

    private RenderTexture tempTex1;
    private RenderTexture tempTex2;

    private void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        Graphics.Blit(src, tempTex1, CrtLines);
        Graphics.Blit(tempTex1, tempTex2, Noise);
        Graphics.Blit(tempTex2, dst, BarrelDistort);
    }

    private void Start()
    {
        lastFrameIntensity = intensity;
        Camera cam = GetComponent<Camera>();
        tempTex1 = new RenderTexture(cam.pixelWidth, cam.pixelWidth, 16, RenderTextureFormat.ARGB32);
        tempTex1.Create();
        tempTex2 = new RenderTexture(cam.pixelWidth, cam.pixelWidth, 16, RenderTextureFormat.ARGB32);
        tempTex2.Create();
    }

    private void Update()
    {
        UpdateIntensity();
    }

    private void UpdateIntensity()
    {
        CrtLines.SetFloat("_Intensity", intensity);
        Noise.SetFloat("_Intensity", intensity * intensity * 0.2f);
    }
}

