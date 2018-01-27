using UnityEngine;

[ExecuteInEditMode]
public class ImageEffect : MonoBehaviour {

    [Range(0, 1)]
    public float intensity = 0;
    private float lastFrameIntensity;
    public Material ColorShift;
    public Material BlockDistort;


    private RenderTexture lastFrame;
    private RenderTexture tempTex;

    private void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        tempTex = new RenderTexture(512, 512, 16, RenderTextureFormat.ARGB32);
        tempTex.Create();
        Graphics.Blit(src, tempTex, ColorShift);
        Graphics.Blit(tempTex, dst, BlockDistort);
    }

    private void Start()
    {
        lastFrameIntensity = intensity;
    }

    private void Update()
    {
        if (Mathf.Abs(intensity - lastFrameIntensity) > 0.01) {
            UpdateIntensity();
        }
    }

    private void UpdateIntensity() {

    }
}
