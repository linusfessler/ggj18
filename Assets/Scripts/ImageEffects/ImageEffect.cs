using UnityEngine;

[ExecuteInEditMode]
public class ImageEffect : MonoBehaviour {

    [Range(0, 1)]
    public float intensity = 0;
    private float lastFrameIntensity;
    public Material ColorShift;
    public Material BlockDistort;
    public Material CrtLines;
    public Material Noise;
    public Material BlackOut;
    public Material BarrelDistort;

    private float lastFlickerTime;
    private float flickerDuration = 0.11f;
    private float nextFlickerTime;
    private RenderTexture lastFrame;
    private RenderTexture tempTex1;
    private RenderTexture tempTex2;

    private void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        tempTex1 = src;
        Graphics.Blit(src, tempTex1, ColorShift);
        Graphics.Blit(tempTex1, tempTex2, BlockDistort);
        Graphics.Blit(tempTex2, tempTex1, BlackOut);
        Graphics.Blit(tempTex1, tempTex2, CrtLines);
        Graphics.Blit(tempTex2, tempTex1, Noise);
        Graphics.Blit(tempTex1, dst, BarrelDistort);
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

    private void UpdateIntensity() {
        ColorShift.SetFloat("_Intensity", intensity * 0.05f);
        CrtLines.SetFloat("_Intensity", intensity);
        Noise.SetFloat("_Intensity", intensity * intensity * 0.7f);
        BlackOut.SetFloat("_Intensity", intensity * intensity * 0.5f);
        if (Time.time > nextFlickerTime)
        {
            //start flicker
            lastFlickerTime = Time.time;
            BlockDistort.SetVector("_SampleOffset", new Vector4(Random.Range(0f, 1f), Random.Range(0f, 1f), 0f, 0f));
            BlockDistort.SetFloat("_Intensity", intensity * 0.2f);
            nextFlickerTime = Time.time + Random.Range(0f, 1.3f - intensity);
        }

        if (Time.time - lastFlickerTime > flickerDuration)
        {
            //stop flicker
            BlockDistort.SetFloat("_Intensity", 0f);
        } 
    }
}
