using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EntityInfo)), CanEditMultipleObjects]
public class EntityInfoEditor : Editor
{
    float mywidth = 300;
   /* private GameObject cam = null;
    private Camera Camera
   {
        get
       {
           if (cam == null)
           {
                cam = new GameObject();
                cam.hideFlags = HideFlags.HideAndDontSave;
                cam.transform.position = new Vector3(0.5f, 2.0f, -5.0f);
                cam.transform.eulerAngles = new Vector3(19.0f, -5.0f, 0.0f);
                cam.AddComponent<Camera>();
                cam.GetComponent<Camera>().depth = 0;
                cam.GetComponent<Camera>().cullingMask = 1;
                cam.GetComponent<Camera>().fieldOfView = 19;
           }
           return cam.GetComponent<Camera>();
       }
   }*/

    // lets us paint a texture nicely to another one (because Unity doesn't somehow provide this)
    private void PaintTexture32Bilinear(ref Texture2D tgt, ref Texture2D src, int _x, int _y, int _width, int _height) {
        // checkign bounds
        if(_x >= tgt.width) {
            Debug.LogError("PaintTexture32Bilinear: x coordinate ["+_x+"] out of target image bounds ["+tgt.width+","+tgt.height+"].");
            return;
        }
        if(_y >= tgt.width) {
            Debug.LogError("PaintTexture32Bilinear: y coordinate ["+_y+"] out of target image bounds ["+tgt.width+","+tgt.height+"].");
            return;
        }
        if((_x + _width) >= tgt.width) {
            _width = tgt.width - _x - 1;
        }
        if((_y + _height) >= tgt.width) {
            _height = tgt.height - _y - 1;
        }

        float delta = 0.33f; //sample inbetween each pixel
        delta = 0;
        Color32[] coltmp = new Color32[1];
        Color32[] rawcols = src.GetPixels32();
        Color32[] cols = tgt.GetPixels32();    
        for(float y = 0; y < _height; ++y) {
            for(float x = 0; x < _width; ++x) {

                Color32 col1 = rawcols[(int)((x/_width)*src.width)+src.width*(int)((y/_height)*src.height)];
                if(col1.a < 10)
                    continue;
                Color32 col2 = rawcols[(int)(((x+delta)/_width)*src.width)+src.width*(int)((y/_height)*src.height)];
                Color32 col3 = rawcols[(int)(((x+delta)/_width)*src.width)+src.width*(int)(((y+delta)/_height)*src.height)];
                Color32 col4 = rawcols[(int)((x/_width)*src.width)+src.width*(int)(((y+delta)/_height)*src.height)];

                Color32 col = new Color32((byte)(((float)col1.r+(float)col2.r+(float)col3.r+(float)col4.r)/4.0f),
                (byte)(((float)col1.g+(float)col2.g+(float)col3.g+(float)col4.g)/4.0f),
                (byte)(((float)col1.b+(float)col2.b+(float)col3.b+(float)col4.b)/4.0f),
                (byte)(((float)col1.a+(float)col2.a+(float)col3.a+(float)col4.a)/4.0f));
                cols[(int)(_x+x) + tgt.width*(int)(_y+y)] = col;
            }
        }
        tgt.SetPixels32(cols);
        tgt.Apply();
        tgt.alphaIsTransparency = true;

    }

    // The preview icon
    public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height) {
        
        Texture2D thumb = new Texture2D(width, height, TextureFormat.RGBA32, false);

        EntityInfo dpv = (EntityInfo) target;
        dpv.staticPreview = new Texture2D(width, height, TextureFormat.RGBA32, false);
        
        if (dpv == null || dpv.protoTex == null) {
            EditorUtility.SetDirty(target);
            return dpv.staticPreview;
        }

        Texture2D sprite = AssetPreview.GetAssetPreview(dpv.protoTex);
        Texture2D backdrop = Resources.Load("Tex/EntityInfoBG") as Texture2D;
        bool valid = sprite != null && backdrop != null;
        
        if(valid) {
            PaintTexture32Bilinear(ref thumb, ref backdrop, 0,0,width,height);
            float aspect = (float)sprite.height / (float)sprite.width;
            int thumbx = 25;
            int thumby = 10;
            int thumbw = 80;
            int thumbh = 80;
            if (aspect > 1) {
                thumbw = (int)(80/aspect);
                thumbx = (int)(25 + (0.5*(80-thumbw)));
            }
            if (aspect < 1) {
                thumbh = (int)(aspect*80);
            }
            PaintTexture32Bilinear(ref thumb, ref sprite, thumbx,thumby,thumbw,thumbh);
            thumb.Apply();
            thumb.alphaIsTransparency = true;
        } else {
            EditorUtility.SetDirty(target);
            return dpv.staticPreview;
        }
        
        EditorUtility.CopySerialized(thumb, dpv.staticPreview);
        return dpv.staticPreview;//dpv.staticPreview;
    }


    public override void OnInspectorGUI()
    {
        //EntityInfo ei = (EntityInfo) target;
        DrawDefaultInspector();
    }
}
