using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileInfo)), CanEditMultipleObjects]
public class TileInfoEditor : Editor
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

    void drawTypeDialog(TileInfo tile, int idx, Vector2Int pos) {
        tile.segmentType[idx] = (TileType)EditorGUI.EnumPopup(new Rect(mywidth/2 + pos.x, pos.y, 80, 15), "", tile.segmentType[idx]);
    }

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

        TileInfo dpv = (TileInfo) target;
        dpv.staticPreview = new Texture2D(width, height, TextureFormat.RGBA32, false);
        
        if (dpv == null || dpv.cardTex == null) {
            EditorUtility.SetDirty(target);
            return dpv.staticPreview;
        }

        Texture2D sprite = AssetPreview.GetAssetPreview(dpv.cardTex);
        Texture2D backdrop = Resources.Load("Tex/TileInfoBG") as Texture2D;
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
        TileInfo tile = (TileInfo) target;

        serializedObject.Update();

        if(tile.requireRepaint) {
            tile.requireRepaint = false;
            EditorUtility.SetDirty(tile);
        }

        GUILayout.Label("Configure Tile", EditorStyles.boldLabel);
        tile.prefabName = GUILayout.TextField(tile.prefabName);
        GUILayout.Space(340f);
        
        SerializedObject so = new SerializedObject(tile);
        {
            SerializedProperty flagsProperty = so.FindProperty("flagsE");
            EditorGUILayout.PropertyField(flagsProperty, true);
            so.ApplyModifiedProperties();
        }
        {
            SerializedProperty flagsProperty = so.FindProperty("flagsSE");
            EditorGUILayout.PropertyField(flagsProperty, true);
            so.ApplyModifiedProperties();
        }
        {
            SerializedProperty flagsProperty = so.FindProperty("flagsSW");
            EditorGUILayout.PropertyField(flagsProperty, true);
            so.ApplyModifiedProperties();
        }
        {
            SerializedProperty flagsProperty = so.FindProperty("flagsW");
            EditorGUILayout.PropertyField(flagsProperty, true);
            so.ApplyModifiedProperties();
        }
        {
            SerializedProperty flagsProperty = so.FindProperty("flagsNW");
            EditorGUILayout.PropertyField(flagsProperty, true);
            so.ApplyModifiedProperties();
        }
        {
            SerializedProperty flagsProperty = so.FindProperty("flagsNE");
            EditorGUILayout.PropertyField(flagsProperty, true);
            so.ApplyModifiedProperties();
        }
        //flags

        mywidth = EditorGUIUtility.currentViewWidth;

        tile.cardTex = EditorGUI.ObjectField(new Rect( mywidth/2-60,40,50,50), tile.cardTex, typeof(Texture2D), false) as Texture2D;
        tile.meshTex = EditorGUI.ObjectField(new Rect( mywidth/2+10,40,50,50), tile.meshTex, typeof(Texture2D), false) as Texture2D;
        //tile.mesh = EditorGUI.ObjectField(new Rect( mywidth/2+80,10,50,50), tile.mesh, typeof(Mesh), false) as Mesh;
        //tile.material = EditorGUI.ObjectField(new Rect( mywidth/2-80,10,50,50), tile.material, typeof(Material), false) as Material;

        drawTypeDialog(tile,2,new Vector2Int(-50-80,70+30));
        drawTypeDialog(tile,3,new Vector2Int(50,70+30));
        drawTypeDialog(tile,1,new Vector2Int(-70-80,130+30));
        drawTypeDialog(tile,4,new Vector2Int(70,130+30));
        drawTypeDialog(tile,0,new Vector2Int(-50-80,190+30));
        drawTypeDialog(tile,5,new Vector2Int(50,190+30));

        if( tile.cardTex)
            GUI.DrawTexture(new Rect( mywidth/2-60,80+30,120,120), tile.cardTex);
        if( tile.meshTex)
            GUI.DrawTexture(new Rect( mywidth/2-60,250,120,120), tile.meshTex);
            
        /*if( tile.mesh && tile.material) {
            GUILayout.Label("Mesh Preview", EditorStyles.boldLabel);
            GUILayout.Space(200);
            tile.material.enableInstancing = true;
            Quaternion previewRotation = Quaternion.identity;
            previewRotation.eulerAngles = new Vector3(0.0f, -45.0f, 0.0f);
            Graphics.DrawMesh( tile.mesh, Vector3.zero, previewRotation,  tile.material, 0, Camera);
            Handles.DrawCamera(new Rect(mywidth/2-100, 370, 200, 200), Camera, DrawCameraMode.TexturedWire);
        }*/
        serializedObject.ApplyModifiedProperties();
    }
}
