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

    private float lastFlickerTime;
    private float flickerDuration = 0.11f;
    private float nextFlickerTime;
    private RenderTexture lastFrame;
    private RenderTexture tempTex1;
    private RenderTexture tempTex2;

    private void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        tempTex1 = src;
        if (ColorShift != null)
        {
            Graphics.Blit(tempTex1, tempTex2, ColorShift);
            tempTex1 = tempTex2;
        }
        if (BlockDistort != null)
        {
            Graphics.Blit(tempTex1, tempTex2, BlockDistort);
            tempTex1 = tempTex2;
        }
        if (BlackOut != null)
        {
            Graphics.Blit(tempTex1, tempTex2, BlackOut);
            tempTex1 = tempTex2;
        }
        if (CrtLines != null)
        {
            Graphics.Blit(tempTex1, tempTex2, CrtLines);
            tempTex1 = tempTex2;
        }
        if (Noise != null)
        {
            Graphics.Blit(tempTex2, dst, Noise);
            tempTex1 = tempTex2;
        }
        dst = tempTex1;
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
        if (ColorShift != null)
        {
            ColorShift.SetFloat("_Intensity", intensity * 0.05f);
        }
        if (CrtLines != null)
        {
            CrtLines.SetFloat("_Intensity", intensity);
        }
        if (Noise != null)
        {
            Noise.SetFloat("_Intensity", intensity * intensity * 0.7f);
        }
        if (BlackOut != null) {
            BlackOut.SetFloat("_Intensity", intensity * intensity * 0.5f);
        }


        if (BlockDistort != null) {
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
}
