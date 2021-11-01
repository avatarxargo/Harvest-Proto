using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TileInfoWindow : EditorWindow
{
    /*bool hexSelected = false;
    HexTile tile = null;
    TileShaderProperty tileShader = null;
    private Texture2D[] tex = new Texture2D[4];*/

    public HarvestGameState hgs;

    [MenuItem("Hextile/Tile Spawner")]
    public static void showWindow() {
        TileInfoWindow tiw = GetWindow<TileInfoWindow>("Tile Spawner");
        if(!tiw.hgs)
            tiw.findGameState();
    }
    void OnGUI() {
        hgs = (HarvestGameState)EditorGUILayout.ObjectField((Object)hgs, typeof(HarvestGameState), true);

        if(!hgs) {
            GUILayout.Label("No Game State Selected", EditorStyles.centeredGreyMiniLabel);
            if(GUILayout.Button("Scan Game State"))
            {
                findGameState();
            }
        }
        else {
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Restart Game"))
            {
                hgs.restartGame();
            }
            if(GUILayout.Button("Clear All"))
            {
                hgs.clearAll();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(20);
            int perRow = 4;

            for(int i = 0; i < hgs.prefabTileTypes.Count; ++i) {
                TileInfo ti = hgs.prefabTileTypes[i];

                if(i%perRow == 0) {
                    GUILayout.BeginHorizontal();
                }

                if(GUILayout.Button(ti.cardTex, GUILayout.Width(60), GUILayout.Height(60)))
                //if(GUILayout.Button(ti.prefabName))
                {
                    hgs.spawnProtoTile(ti);
                }

                if(i == hgs.prefabTileTypes.Count-1 || i%perRow == (perRow-1)) {
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.Space(20);

            for(int i = 0; i < hgs.prefabEntityTypes.Count; ++i) {
                EntityInfo ei = hgs.prefabEntityTypes[i];

                if(i%perRow == 0) {
                    GUILayout.BeginHorizontal();
                }

                if(GUILayout.Button(ei.protoTex, GUILayout.Width(60), GUILayout.Height(60)))
                //if(GUILayout.Button(ti.prefabName))
                {
                    hgs.spawnProtoEntity(ei);
                }

                if(i == hgs.prefabEntityTypes.Count-1 || i%perRow == (perRow-1)) {
                    GUILayout.EndHorizontal();
                }
            }

            /*foreach(EntityInfo ei in hgs.prefabEntityTypes) {
                if(GUILayout.Button(ei.prefabName))
                {
                    hgs.spawnProtoEntity(ei);
                }
            }*/
            /*if(GUILayout.Button("Spawn ProtoTile"))
            {
                gs.spawnProtoTile(tgt.protoSpawnindex);
            }

            if(GUILayout.Button("Spawn ProtoEntity"))
            {
                gs.spawnProtoEntity(tgt.protoSpawnindex);
            }

            if(GUILayout.Button("Clear All"))
            {
                gs.clearAll();
            }*/
        }
    }

    private void OnValidate() {
        /*tex[0] = Resources.Load<Texture2D>("Prototiles/grass");
        tex[1] = Resources.Load<Texture2D>("Prototiles/sand");
        tex[2] = Resources.Load<Texture2D>("Prototiles/cliffs");
        tex[3] = Resources.Load<Texture2D>("Prototiles/fertile");*/
        findGameState();
    }

    private void findGameState() {
        Scene scene = SceneManager.GetActiveScene();
        GameObject[] gos = scene.GetRootGameObjects();
        foreach(GameObject go in gos) {
            HarvestGameState _hgs = go.GetComponentInChildren<HarvestGameState>();
            if(_hgs) {
                hgs = _hgs;
                return;
            }
        }
    }

    /*void drawTypeDialog(Vector2Int pos) {
        TileType oldtype = tileShader.type;
            tileShader.type = (TileType)EditorGUI.EnumPopup(new Rect(position.width/2 + pos.x, pos.y, 80, 15), "", tileShader.type);
            if(tileShader.type != oldtype)
                tileShader.setType(tileShader.type);
    }

    void OnGUI() {

        if(!hexSelected)
            GUILayout.Label("No Tile Selected", EditorStyles.centeredGreyMiniLabel);
        else {
            //selected = GUILayout.Toolbar(selected,items);
            GUILayout.Label(tile.name, EditorStyles.centeredGreyMiniLabel);
            bool oldghost = tile.isGhost;
            tile.isGhost = EditorGUILayout.Toggle("ghost", tile.isGhost);
            if(tile.isGhost != oldghost)
                tile.setGhost(tile.isGhost);

            drawTypeDialog(new Vector2Int(-50-80,70));
            drawTypeDialog(new Vector2Int(50,70));
            drawTypeDialog(new Vector2Int(-70-80,130));
            drawTypeDialog(new Vector2Int(70,130));
            drawTypeDialog(new Vector2Int(-50-80,190));
            drawTypeDialog(new Vector2Int(50,190));

            GUI.DrawTexture(new Rect( position.width/2-60,80,120,120), tex[(int)tileShader.type]);
        }
    }

    public void OnInspectorUpdate()
    {
        if (Selection.activeTransform)
        {
            tile = Selection.activeTransform.GetComponent<HexTile>();
        } else {
            tile = null;
        }
        hexSelected = (tile != null);
        if(hexSelected) {
            tileShader = tile.GetComponentInChildren<TileShaderProperty>();
        }
        this.Repaint();
    }*/
}
