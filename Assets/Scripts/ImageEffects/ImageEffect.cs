using UnityEngine;

[ExecuteInEditMode]
public class ImageEffect : MonoBehaviour {

    public Material EffectMaterial;
    public Material BlockDistort;


    private RenderTexture lastFrame;
    private Texture tempTex;

    private void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        Graphics.Blit(src, dst, BlockDistort);
    }

    private void DataMosh() {

    }
}
