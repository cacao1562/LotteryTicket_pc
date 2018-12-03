using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NewBehaviourScript : MonoBehaviour {

	public SpriteRenderer spp;
	private Color32[] cc;
	private Color32[] c2;
	private Color32[] c3;
	void Start () {
		
		SpriteRenderer sr = GetComponent<SpriteRenderer>();
		Texture2D tt = duplicateTexture(sr.sprite.texture);
		Rect rect = new Rect(0,0,tt.width, tt.height);
        Sprite sss = Sprite.Create(tt, rect , new Vector2(0.5f, 0.5f));
		Sprite s = sr.sprite;
		cc = new Color32[(int)s.rect.width * (int)s.rect.height];
		c2 = new Color32[(int)s.rect.width * (int)s.rect.height];
		c3 = new Color32[(int)s.rect.width * (int)s.rect.height];
		cc = sss.texture.GetPixels32();
		 spp.sprite = sss;
		// ss.SetPixels(cc);
		Debug.Log("cc = " + cc[0] );

		
		c2 = spp.sprite.texture.GetPixels32();
		Debug.Log("c2 = " + c2[0] );

		for (int i =0; i<c2.Length; i++) {
			c2[i] = new Color(1f,1f,1f,1f);
		}
		spp.sprite.texture.SetPixels32(c2);
		spp.sprite.texture.Apply();
		// ss.Apply();
	}

	 Texture2D render(Texture2D tex) {
            var temp = RenderTexture.GetTemporary(tex.width, tex.height);
            Graphics.Blit(tex, temp);
            // ReadPixelsで直前のレンダリング結果を読み込める
            var copy = new Texture2D(tex.width, tex.height);
            copy.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
            RenderTexture.ReleaseTemporary(temp);
            return copy;
        }
        Texture2D duplicateTexture2(Texture2D source)
            {
                byte[] pix = source.GetRawTextureData();
                Texture2D readableText = new Texture2D(source.width, source.height, source.format, false);
                readableText.LoadRawTextureData(pix);
                readableText.Apply();
                return readableText;
            }
	
	
	 Texture2D duplicateTexture(Texture2D source)
    {
            RenderTexture renderTex = RenderTexture.GetTemporary(
                        source.width,
                        source.height,
                        0,
                        RenderTextureFormat.Default,
                        RenderTextureReadWrite.Linear);

            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            return readableText;
    }

}
