using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicTriangle : MonoBehaviour
{
    public MeshFilter[] sections;
    public TileShaderProperty[] shaderProperties;

    public SurfaceInfo surfaceInfo;
    public SurfaceInfo surfaceInfoSand;
    public SurfaceInfo surfaceInfoGrass;    
    public SurfaceInfo surfaceInfoFertile;
    public SurfaceInfo surfaceInfoLake;
    public SurfaceInfo surfaceInfoCliffGrass;
    public SurfaceInfo surfaceInfoCliffSand;
    
    public bool landmarkTile = false;
    public bool landmarkCoreTile = false;

     public int backL = 0;
     public int backR = 0;
     public int rightL = 0;
     public int rightR = 0;
     public int leftL = 0;
     public int leftR = 0;

    public void init() {
        sections = this.GetComponentsInChildren<MeshFilter>();
        shaderProperties = this.GetComponentsInChildren<TileShaderProperty>();
    }

    public void setLandmark(HexDirection dir) {
        landmarkCoreTile = dir == HexDirection.HexEast;
        landmarkTile = surfaceInfo.isLandmark;
        sections[0].gameObject.SetActive(!landmarkTile || landmarkCoreTile);
        sections[1].gameObject.SetActive(!landmarkTile);
        sections[2].gameObject.SetActive(!landmarkTile);
        sections[3].gameObject.SetActive(!landmarkTile);
        sections[4].gameObject.SetActive(!landmarkTile);
    }

    public void setType(TileType type) {
        switch(type) {
            default:
            case TileType.Grass:
                surfaceInfo = surfaceInfoGrass;
                break;
            case TileType.Sand:
                surfaceInfo = surfaceInfoSand;
                break;
            case TileType.Fertile:
                surfaceInfo = surfaceInfoFertile;
                break;
            case TileType.Lake:
                surfaceInfo = surfaceInfoLake;
                break;
            case TileType.CliffsGrass:
                surfaceInfo = surfaceInfoCliffGrass;
                break;
            case TileType.CliffsSand:
                surfaceInfo = surfaceInfoCliffSand;
                break;
        }
        sections[0].mesh = surfaceInfo.centerMesh;
        refeshShader();
    }

    public void setAngle(int mesh, int steps) {
        /*if(steps == 0) {
            sections[mesh].gameObject.SetActive(false);
            return;
        } else {
            sections[mesh].gameObject.SetActive(true);
        }
        sections[mesh].mesh = surfaceInfo.sideMeshVariants[steps-1];*/
        sections[mesh].mesh = surfaceInfo.sideMeshVariants[steps];
    }

    public void refeshShader() {
        foreach(TileShaderProperty tsp in shaderProperties) {
            /*tsp.surfaceTex = surfaceInfo.diffuseTexture;
            tsp.alphaTex = surfaceInfo.alphaTexture;
            tsp.worldTex = surfaceInfo.worldTexture;
            tsp.worldTexMask = surfaceInfo.maskTexture;*/
            tsp.mat_regular = surfaceInfo.matReg;
            tsp.mat_ghost = surfaceInfo.matGhost;
            tsp.mat_warn = surfaceInfo.matWarn;
            tsp.mat_ghost_warn = surfaceInfo.matGhostWarn;
            tsp.refeshShader();
        }
    }

    bool testType(TileSystem tileSystem, HexCoordinate coord, TileType type) {
        List<HexTile> other = tileSystem.getTilesAt(coord);
        if(other.Count > 0) {
            HexTile first = other[0];
            TileType otherType = TileTypeUtil.getEffectiveType(first.tileInfo.segmentType[(int)first.directionGlobal2Internal(coord.section)]);
            return otherType == TileTypeUtil.getEffectiveType(type);
        } else {
            return false;
        }
    }
    public void gameUpdate(TileSystem tileSystem, HexCoordinate coord) {
        HexCoordinate tmp;
        int i = 0;

        //back - L:
        tmp = new HexCoordinate(coord.x, coord.y, coord.section);
        for(i = 0; i < 5; ++i) {
            tmp = tmp.getTriNeighbor((i%2==0)?TriDirection.Back:TriDirection.Right);
            if(testType(tileSystem,tmp,surfaceInfo.type)) {
                break;
            }
        }
        backL = i;

        //back - R:
        tmp = new HexCoordinate(coord.x, coord.y, coord.section);
        for(i = 0; i < 5; ++i) {
            tmp = tmp.getTriNeighbor((i%2==0)?TriDirection.Back:TriDirection.Left);
            if(testType(tileSystem,tmp,surfaceInfo.type)) {
                break;
            }
        }
        backR = i;

        //right - L
        tmp = new HexCoordinate(coord.x, coord.y, coord.section);
        for(i = 0; i < 5; ++i) {
            tmp = tmp.getTriNeighbor(TriDirection.Right);
            if(testType(tileSystem,tmp,surfaceInfo.type)) {
                break;
            }
        }
        rightL = i;

        //right - R
        tmp = new HexCoordinate(coord.x, coord.y, coord.section);
        for(i = 0; i < 5; ++i) {
            tmp = tmp.getTriNeighbor((i%2==0)?TriDirection.Right:TriDirection.Back);
            if(testType(tileSystem,tmp,surfaceInfo.type)) {
                break;
            }
        }
        rightR = i;

        //left - L
        tmp = new HexCoordinate(coord.x, coord.y, coord.section);
        for(i = 0; i < 5; ++i) {
            tmp = tmp.getTriNeighbor((i%2==0)?TriDirection.Left:TriDirection.Back);
            if(testType(tileSystem,tmp,surfaceInfo.type)) {
                break;
            }
        }
        leftL = i;
        
        //left - R
        tmp = new HexCoordinate(coord.x, coord.y, coord.section);
        for(i = 0; i < 5; ++i) {
            tmp = tmp.getTriNeighbor(TriDirection.Left);
            if(testType(tileSystem,tmp,surfaceInfo.type)) {
                break;
            }
        }
        leftR = i;

        // update plain connectors
        if(leftR == 0 || rightL == 0) {
            tmp = new HexCoordinate(coord.x, coord.y, coord.section);
            tmp = tmp.getTriNeighbor(TriDirection.Left);
            int seriesA = 0; // how many identical triangles are around me?
            int seriesB = 0; // how many identical triangles are around me?
            int seriesC = 0;
            int series = 0;
            if(testType(tileSystem,tmp,surfaceInfo.type)) {
                ++seriesA;
                tmp = tmp.getTriNeighbor(TriDirection.Left);
                if(testType(tileSystem,tmp,surfaceInfo.type)) {
                    ++seriesA;
                    tmp = tmp.getTriNeighbor(TriDirection.Left);
                    if(testType(tileSystem,tmp,surfaceInfo.type))
                        ++seriesA;
                } else {
                    tmp = tmp.getTriNeighbor(TriDirection.Left);
                    if(testType(tileSystem,tmp,surfaceInfo.type))
                        ++seriesC;
                }
            } else {
                tmp = tmp.getTriNeighbor(TriDirection.Left);
                if(testType(tileSystem,tmp,surfaceInfo.type)) 
                    ++seriesC;
                tmp = tmp.getTriNeighbor(TriDirection.Left);
                if(testType(tileSystem,tmp,surfaceInfo.type))
                    ++seriesC;
            }
            tmp = new HexCoordinate(coord.x, coord.y, coord.section);
            tmp = tmp.getTriNeighbor(TriDirection.Right);
            if(testType(tileSystem,tmp,surfaceInfo.type)) {
                ++seriesB;
                tmp = tmp.getTriNeighbor(TriDirection.Right);
                if(testType(tileSystem,tmp,surfaceInfo.type)) {
                    ++seriesB;
                    tmp = tmp.getTriNeighbor(TriDirection.Right);
                    if(testType(tileSystem,tmp,surfaceInfo.type))
                        ++seriesB;
                } else {
                    tmp = tmp.getTriNeighbor(TriDirection.Right);
                    if(testType(tileSystem,tmp,surfaceInfo.type))
                        ++seriesC;
                }
            } else {
                tmp = tmp.getTriNeighbor(TriDirection.Right);
                if(testType(tileSystem,tmp,surfaceInfo.type))
                    ++seriesC;
                tmp = tmp.getTriNeighbor(TriDirection.Right);
                if(testType(tileSystem,tmp,surfaceInfo.type))
                    ++seriesC;
            }

            series = seriesA + seriesB + seriesC*2;
            if(series >= 3) { //if 4+ we can just use the thick connectors
                if(leftR == 0)
                    leftR = 7; //long
                if(rightL == 0)
                    rightL = 7; //long
            } else if(seriesA == 1 && seriesB == 1) {
                if(leftR == 0)
                    leftR = 6; //flat
                if(rightL == 0)
                    rightL = 6; //flat
            }
        }

        if(rightR == 0 || backR == 0) {
            tmp = new HexCoordinate(coord.x, coord.y, coord.section);
            tmp = tmp.getTriNeighbor(TriDirection.Right);
            int seriesA = 0; // how many identical triangles are around me?
            int seriesB = 0; // how many identical triangles are around me?
            int seriesC = 0;
            int series = 0;
            if(testType(tileSystem,tmp,surfaceInfo.type)) {
                ++seriesA;
                tmp = tmp.getTriNeighbor(TriDirection.Back);
                if(testType(tileSystem,tmp,surfaceInfo.type)) {
                    ++seriesA;
                    tmp = tmp.getTriNeighbor(TriDirection.Right);
                    if(testType(tileSystem,tmp,surfaceInfo.type))
                        ++seriesA;
                } else {
                    tmp = tmp.getTriNeighbor(TriDirection.Right);
                    if(testType(tileSystem,tmp,surfaceInfo.type))
                        ++seriesC;
                }
            } else {
                tmp = tmp.getTriNeighbor(TriDirection.Back);
                if(testType(tileSystem,tmp,surfaceInfo.type))
                    ++seriesC;
                tmp = tmp.getTriNeighbor(TriDirection.Right);
                if(testType(tileSystem,tmp,surfaceInfo.type))
                    ++seriesC;
            }
            tmp = new HexCoordinate(coord.x, coord.y, coord.section);
            tmp = tmp.getTriNeighbor(TriDirection.Back);
            if(testType(tileSystem,tmp,surfaceInfo.type)) {
                ++seriesB;
                tmp = tmp.getTriNeighbor(TriDirection.Left);
                if(testType(tileSystem,tmp,surfaceInfo.type)) {
                    ++seriesB;
                    tmp = tmp.getTriNeighbor(TriDirection.Back);
                    if(testType(tileSystem,tmp,surfaceInfo.type))
                        ++seriesB;
                } else {
                    tmp = tmp.getTriNeighbor(TriDirection.Back);
                    if(testType(tileSystem,tmp,surfaceInfo.type))
                        ++seriesC;
                }
            } else {
                tmp = tmp.getTriNeighbor(TriDirection.Left);
                if(testType(tileSystem,tmp,surfaceInfo.type))
                    ++seriesC;
                tmp = tmp.getTriNeighbor(TriDirection.Back);
                if(testType(tileSystem,tmp,surfaceInfo.type))
                    ++seriesC;
            }

            series = seriesA + seriesB + seriesC*2;
            if(series >= 3) { //if 4+ we can just use the thick connectors
                if(rightR == 0)
                    rightR = 7; //long
                if(backR == 0)
                    backR = 7; //long
            } else if(seriesA == 1 && seriesB == 1) {
                if(rightR == 0)
                    rightR = 6; //flat
                if(backR == 0)
                    backR = 6; //flat
            }
        }

        if(leftL == 0 || backL == 0) {
            tmp = new HexCoordinate(coord.x, coord.y, coord.section);
            tmp = tmp.getTriNeighbor(TriDirection.Left);
            int seriesA = 0; // how many identical triangles are around me?
            int seriesB = 0; // how many identical triangles are around me?
            int seriesC = 0;
            int series = 0;
            if(testType(tileSystem,tmp,surfaceInfo.type)) {
                ++seriesA;
                tmp = tmp.getTriNeighbor(TriDirection.Back);
                if(testType(tileSystem,tmp,surfaceInfo.type)) {
                    ++seriesA;
                    tmp = tmp.getTriNeighbor(TriDirection.Left);
                    if(testType(tileSystem,tmp,surfaceInfo.type))
                        ++seriesA;
                } else {
                    tmp = tmp.getTriNeighbor(TriDirection.Left);
                    if(testType(tileSystem,tmp,surfaceInfo.type))
                        ++seriesC;
                }
            } else {
                tmp = tmp.getTriNeighbor(TriDirection.Back);
                if(testType(tileSystem,tmp,surfaceInfo.type))
                    ++seriesC;
                tmp = tmp.getTriNeighbor(TriDirection.Left);
                if(testType(tileSystem,tmp,surfaceInfo.type))
                    ++seriesC;
            }
            tmp = new HexCoordinate(coord.x, coord.y, coord.section);
            tmp = tmp.getTriNeighbor(TriDirection.Back);
            if(testType(tileSystem,tmp,surfaceInfo.type)) {
                ++seriesB;
                tmp = tmp.getTriNeighbor(TriDirection.Right);
                if(testType(tileSystem,tmp,surfaceInfo.type)) {
                    ++seriesB;
                    tmp = tmp.getTriNeighbor(TriDirection.Back);
                    if(testType(tileSystem,tmp,surfaceInfo.type))
                        ++seriesB;
                } else {
                    tmp = tmp.getTriNeighbor(TriDirection.Back);
                    if(testType(tileSystem,tmp,surfaceInfo.type))
                        ++seriesC;
                }
            } else {
                tmp = tmp.getTriNeighbor(TriDirection.Right);
                if(testType(tileSystem,tmp,surfaceInfo.type))
                    ++seriesC;
                tmp = tmp.getTriNeighbor(TriDirection.Back);
                if(testType(tileSystem,tmp,surfaceInfo.type))
                    ++seriesC;
            }

            series = seriesA + seriesB + seriesC*2;
            if(series >= 3) { //if 4+ we can just use the thick connectors
                if(leftL == 0)
                    leftL = 7; //long
                if(backL == 0)
                    backL = 7; //long
            } else if(seriesA == 1 && seriesB == 1) {
                if(leftL == 0)
                    leftL = 6; //flat
                if(backL == 0)
                    backL = 6; //flat
            }
        }

        // prevent self-glitch
        if(backL == 3) {
            tmp = new HexCoordinate(coord.x, coord.y, coord.section);
            tmp = tmp.getTriNeighbor(TriDirection.Left);
            if(!testType(tileSystem,tmp,surfaceInfo.type)) {
                backL = 5;
                leftL = 5;
            }
        }
        if(backR == 3) {
            tmp = new HexCoordinate(coord.x, coord.y, coord.section);
            tmp = tmp.getTriNeighbor(TriDirection.Right);
            if(!testType(tileSystem,tmp,surfaceInfo.type)){
                backR = 5;
                rightR = 5;
            }
        }
        if(leftL == 3) {
            tmp = new HexCoordinate(coord.x, coord.y, coord.section);
            tmp = tmp.getTriNeighbor(TriDirection.Back);
            if(!testType(tileSystem,tmp,surfaceInfo.type)) {
                leftL = 5;
                backL = 5;
            }
        }
        if(rightR == 3) {
            tmp = new HexCoordinate(coord.x, coord.y, coord.section);
            tmp = tmp.getTriNeighbor(TriDirection.Back);
            if(!testType(tileSystem,tmp,surfaceInfo.type)) {
                rightR = 5;
                backR = 5;
            }
        }
        if(leftR == 3) {
            tmp = new HexCoordinate(coord.x, coord.y, coord.section);
            tmp = tmp.getTriNeighbor(TriDirection.Right);
            if(!testType(tileSystem,tmp,surfaceInfo.type)) {
                leftR = 5;
                rightL = 5;
            }
        }
        if(rightL == 3) {
            tmp = new HexCoordinate(coord.x, coord.y, coord.section);
            tmp = tmp.getTriNeighbor(TriDirection.Left);
            if(!testType(tileSystem,tmp,surfaceInfo.type)) {
                rightL = 5;
                leftR = 5;
            }
        }

        setAngle(6,backL);
        setAngle(5,backR);
        setAngle(4,rightL);
        setAngle(3,rightR);
        setAngle(2,leftL);
        setAngle(1,leftR);

        /*setAngle(1,L);
        setAngle(2,L);
        setAngle(3,R);
        setAngle(4,R);
        setAngle(5,B);
        setAngle(6,B);*/
    }

    private void OnValidate() {
        init();
    }
}
