using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndMoveProto : MonoBehaviour
{
    private bool dragging;
    private bool isTouching;
    private ProtoTile dragTarget;
    public Camera sceneCamera;
    public TileSystem tileSystem;

    public float verticalShift = 2.0f;
    private int mask = 1 << 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(sceneCamera && tileSystem)
            clickTest();
    }

    public void clickTest() {

        if(Input.GetMouseButtonUp(0) && dragging) {
            dragging = false;
            dragTarget = null;
        }

        RaycastHit mouseHit = new RaycastHit();
        Ray mouseRay = sceneCamera.ScreenPointToRay(Input.mousePosition);
        bool mouseDidHit = Input.GetMouseButton(0) && Physics.Raycast(mouseRay, out mouseHit, Mathf.Infinity, mask);

        foreach (Touch touch in Input.touches) {
            if (touch.fingerId == 0) {
                if (Input.GetTouch(0).phase == TouchPhase.Began) {
                    Debug.Log("First finger entered!");
                    isTouching = true;
                }
                if (Input.GetTouch(0).phase == TouchPhase.Ended) {
                    Debug.Log("First finger left.");
                    isTouching = false;
                    dragging = false;
                    dragTarget = null;
                }
            }
        }
        
        RaycastHit hit = new RaycastHit();
        if(mouseDidHit) {
           hit = mouseHit;
        }
        bool fingerDidHit = false;
        if(isTouching) {
            RaycastHit fingerHit= new RaycastHit();
            Ray fingerRay = sceneCamera.ScreenPointToRay(Input.mousePosition);
            fingerDidHit = Input.GetMouseButton(0) && Physics.Raycast(fingerRay, out fingerHit, Mathf.Infinity, mask);
            
            if(fingerDidHit) {
                hit = fingerHit;
            }
        }

        //    
        if (mouseDidHit || fingerDidHit) {
            //Transform objectHit = hit.transform;
            /*ProtoTile pt = objectHit.GetComponent<ProtoTile>();
            if(pt) {
                dragging = true;
                dragTarget = objectHit;
            }*/
            Vector3 coord = hit.point;
            //Debug.Log(hexCoord.x + " : " + hexCoord.y);

            if (!dragging) {
                HexCoordinate hexCoord = HexCoordinate.snapToHexGrid(coord,0,tileSystem.hexRadius);

                List<HexTile> foundTiles = tileSystem.getTilesAt(hexCoord,false,true);
                if(foundTiles.Count > 0) {
                    if(foundTiles[0].protoParent) {
                        dragging = true;
                        dragTarget = foundTiles[0].protoParent;
                    }
                } else {
                    List<HexEntity> foundEntities = tileSystem.getEntitiesOnHex(hexCoord,false,true);
                    if(foundEntities.Count > 0) {
                        if(foundEntities[0].protoParent) {
                            dragging = true;
                            dragTarget = foundEntities[0].protoParent;
                        }
                    }
                }
            } else {
                if(dragTarget) {
                    dragTarget.transform.position = coord + new Vector3(0,verticalShift,0);

                    if(Input.GetKeyDown("a") || Input.GetKeyDown("<") || Input.GetKeyDown("left")) {
                        dragTarget.transform.Rotate(new Vector3(0,60,0));
                    }
                    if(Input.GetKeyDown("d") || Input.GetKeyDown(">") || Input.GetKeyDown("right")) {
                        dragTarget.transform.Rotate(new Vector3(0,-60,0));
                    }
                    if(Input.GetKeyDown("r") || Input.GetKeyDown("enter")) {
                        dragTarget.requestPlacement = true;
                    }
                }
            }
        }
    }
}
