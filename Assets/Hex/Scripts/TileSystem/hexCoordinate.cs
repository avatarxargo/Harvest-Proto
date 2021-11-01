using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public struct HexCoordinate {
    public int x, y, level;
    public HexDirection section;

    public const float GRID_SPACING = 1.039f;
    public const float INDICATOR_SCALE = 1;

    public HexCoordinate(int x, int y, HexDirection sec = 0) {
        this.x = x;
        this.y = y;
        this.level = 0;
        section = sec;
    }
    public HexCoordinate(int x, int y, int level, HexDirection sec = 0) {
        this.x = x;
        this.y = y;
        this.level = level;
        section = sec;
    }
    public HexCoordinate(Vector2Int pos, HexDirection sec = 0) {
        this.x = pos.x;
        this.y = pos.y;
        this.level = 0;
        section = sec;
    }
    public HexCoordinate(Vector3Int pos, HexDirection sec = 0) {
        this.x = pos.x;
        this.y = pos.y;
        this.level = pos.z;
        section = sec;
    }

    /// <summary>
    /// Compare just x and y
    /// </summary>
    public static bool operator %(HexCoordinate a, HexCoordinate b) => a.x == b.x && a.y == b.y;

    /// <summary>
    /// Compare x, y and section
    /// </summary>
    public static bool operator ^(HexCoordinate a, HexCoordinate b) => (a.x == b.x && a.y == b.y && a.section == b.section);
    public static HexCoordinate operator +(HexCoordinate a, Vector2Int b) {
        return new HexCoordinate(a.x + b.x, a.y +b.y, a.level, a.section);
    }
    
    public Vector2Int vec2() => new Vector2Int(x,y);
    public Vector3Int vec3() => new Vector3Int(x,y,(int)section);

    /// <summary>
    /// Returns a coordinate offset in the given direction
    /// </summary>
    public HexCoordinate offset(HexDirection dir) {
        bool oddy = 0==Mathf.Abs(y)%2;
    
        if(oddy) {
            switch(dir) {
                case HexDirection.HexEast:
                    return new HexCoordinate(x + 1,y + 0,level,section);
                case HexDirection.HexSouthEast:
                    return new HexCoordinate(x + 1,y - 1,level,section);
                case HexDirection.HexSouthWest:
                    return new HexCoordinate(x + 0,y - 1,level,section);
                case HexDirection.HexWest:
                    return new HexCoordinate(x - 1,y + 0,level,section);
                case HexDirection.HexNorthWest:
                    return new HexCoordinate(x + 0,y + 1,level,section);
                case HexDirection.HexNorthEast:
                    return new HexCoordinate(x + 1,y + 1,level,section);
            }
        } else {
            switch(dir) {
                case HexDirection.HexEast:
                    return new HexCoordinate(x + 1,y + 0,level,section);
                case HexDirection.HexSouthEast:
                    return new HexCoordinate(x + 0,y - 1,level,section);
                case HexDirection.HexSouthWest:
                    return new HexCoordinate(x - 1,y - 1,level,section);
                case HexDirection.HexWest:
                    return new HexCoordinate(x - 1,y + 0,level,section);
                case HexDirection.HexNorthWest:
                    return new HexCoordinate(x - 1,y + 1,level,section);
                case HexDirection.HexNorthEast:
                    return new HexCoordinate(x + 0,y + 1,level,section);
            }
        }
        return new HexCoordinate(0,0,level,section);
    }

    public Vector2 toPosition(float hexRadius) {
        bool odd = 0==Mathf.Abs(y)%2;
        float wy = y*(1.5f*hexRadius);
        float wx = ((odd?0.0f:-0.5f)+x)*(2*0.8660254037f*hexRadius);
        return new Vector2(wx,wy);
    }

    public static HexCoordinate snapToHexGrid(Transform transform, float hexRadius) {
        float otherLength = 0.8660254037f * hexRadius;
        float my_x = transform.position.x;
        float my_y = transform.position.y;
        float my_z = transform.position.z;
        int zfactor = (int)Mathf.Floor((my_z)/(1.5f*hexRadius)+0.5f);
        bool odd = Mathf.Abs(zfactor)%2==0;
        int xfactor = (int)Mathf.Floor((odd?0.0f:0.5f)+(my_x)/(2*otherLength)+0.5f);
        float snapz = zfactor*(1.5f*hexRadius);
        float snapx = ((odd?0.0f:-0.5f)+xfactor)*(2*otherLength);
        //
        float mainrot = transform.eulerAngles.y;
        int sec = (int)Mathf.Floor(mainrot/60.0f+0.5f)%6;
        return new HexCoordinate(xfactor,zfactor,0,(HexDirection)sec);
    }
    public static HexCoordinate snapToHexGrid(Vector3 position, float angle, float hexRadius) {
        float otherLength = 0.8660254037f * hexRadius;
        float my_x = position.x;
        float my_y = position.y;
        float my_z = position.z;
        int zfactor = (int)Mathf.Floor((my_z)/(1.5f*hexRadius)+0.5f);
        bool odd = Mathf.Abs(zfactor)%2==0;
        int xfactor = (int)Mathf.Floor((odd?0.0f:0.5f)+(my_x)/(2*otherLength)+0.5f);
        float snapz = zfactor*(1.5f*hexRadius);
        float snapx = ((odd?0.0f:-0.5f)+xfactor)*(2*otherLength);
        //
        float mainrot = angle;
        int sec = (int)Mathf.Floor(mainrot/60.0f+0.5f)%6;
        return new HexCoordinate(xfactor,zfactor,0,(HexDirection)sec);
    }

    public HexCoordinate getTriNeighbor(TriDirection dir) {
        switch(dir) {
            case TriDirection.Back:
            default: {
                HexCoordinate other = this.offset(this.section);
                other.section = HexDirectionUtil.rotate(this.section,3);
                return other;
            }
            case TriDirection.Left: {
                HexCoordinate other = new HexCoordinate(this.x,this.y,this.level,HexDirectionUtil.rotate(this.section,1));
                return other;
            }
            case TriDirection.Right: {
                HexCoordinate other = new HexCoordinate(this.x,this.y,this.level,HexDirectionUtil.rotate(this.section,-1));
                return other;
            }
        }
    }

    public static void visualizeHex(HexCoordinate coord, float height) {
        Vector2 realcoord = coord.toPosition(GRID_SPACING);
        Vector3 center = new Vector3(realcoord.x,height,realcoord.y);
        Gizmos.DrawLine(center + new Vector3(INDICATOR_SCALE*0,0,INDICATOR_SCALE*1), center + new Vector3(-0.866f,0,0.5f)); //NW
        Gizmos.DrawLine(center + new Vector3(INDICATOR_SCALE*-0.866f,0,INDICATOR_SCALE*0.5f), center + new Vector3(-0.866f,0,-0.5f)); //W
        Gizmos.DrawLine(center + new Vector3(INDICATOR_SCALE*-0.866f,0,INDICATOR_SCALE*-0.5f), center + new Vector3(0,0,-1)); //SW
        Gizmos.DrawLine(center + new Vector3(INDICATOR_SCALE*0,0,INDICATOR_SCALE*-1), center + new Vector3(0.866f,0,-0.5f)); //SE
        Gizmos.DrawLine(center + new Vector3(INDICATOR_SCALE*0.866f,0,INDICATOR_SCALE*-0.5f), center + new Vector3(0.866f,0,0.5f)); //E
        Gizmos.DrawLine(center + new Vector3(INDICATOR_SCALE*0.866f,0,INDICATOR_SCALE*0.5f), center + new Vector3(0,0,1)); //NE
    }

    public static void visualizeTri(HexCoordinate coord, float height, Color color) {
        Gizmos.color = color;
        Vector2 realcoord = coord.toPosition(GRID_SPACING);
        Vector3 center = new Vector3(realcoord.x,height,realcoord.y);
        switch (coord.section) {
            case HexDirection.HexEast:
                Gizmos.DrawLine(center + new Vector3(INDICATOR_SCALE*0.866f,0,INDICATOR_SCALE*-0.5f), center + new Vector3(INDICATOR_SCALE*0.866f,0,INDICATOR_SCALE*0.5f)); //E
                Gizmos.DrawLine(center + new Vector3(INDICATOR_SCALE*0.866f,0,INDICATOR_SCALE*-0.5f), center);
                Gizmos.DrawLine(center + new Vector3(INDICATOR_SCALE*0.866f,0,INDICATOR_SCALE*0.5f), center);
                break;
            case HexDirection.HexSouthEast:
                Gizmos.DrawLine(center + new Vector3(INDICATOR_SCALE*0,     0,INDICATOR_SCALE*-1), center + new Vector3(INDICATOR_SCALE*0.866f,0,INDICATOR_SCALE*-0.5f)); //SE
                Gizmos.DrawLine(center + new Vector3(INDICATOR_SCALE*0.866f,0,INDICATOR_SCALE*-0.5f), center);
                Gizmos.DrawLine(center + new Vector3(INDICATOR_SCALE*0,     0,INDICATOR_SCALE*-1), center);
                break;
            case HexDirection.HexSouthWest:
                Gizmos.DrawLine(center + new Vector3(INDICATOR_SCALE*-0.866f,0,INDICATOR_SCALE*-0.5f), center + new Vector3(INDICATOR_SCALE*0,0,INDICATOR_SCALE*-1)); //SW
                Gizmos.DrawLine(center + new Vector3(INDICATOR_SCALE*0      ,0,INDICATOR_SCALE*-1), center);
                Gizmos.DrawLine(center + new Vector3(INDICATOR_SCALE*-0.866f,0,INDICATOR_SCALE*-0.5f), center);
                break;
            case HexDirection.HexWest:
                Gizmos.DrawLine(center + new Vector3(INDICATOR_SCALE*-0.866f,0,INDICATOR_SCALE*0.5f), center + new Vector3(INDICATOR_SCALE*-0.866f,0,INDICATOR_SCALE*-0.5f)); //W
                Gizmos.DrawLine(center + new Vector3(INDICATOR_SCALE*-0.866f,0,INDICATOR_SCALE*-0.5f), center);
                Gizmos.DrawLine(center + new Vector3(INDICATOR_SCALE*-0.866f,0,INDICATOR_SCALE*0.5f), center);
                break;
            case HexDirection.HexNorthWest:
                Gizmos.DrawLine(center + new Vector3(INDICATOR_SCALE*0      ,0,INDICATOR_SCALE*1), center + new Vector3(INDICATOR_SCALE*-0.866f,0,INDICATOR_SCALE*0.5f)); //NW
                Gizmos.DrawLine(center + new Vector3(INDICATOR_SCALE*-0.866f,0,INDICATOR_SCALE*0.5f), center);
                Gizmos.DrawLine(center + new Vector3(INDICATOR_SCALE*0      ,0,INDICATOR_SCALE*1), center);
                break;
            case HexDirection.HexNorthEast:
                Gizmos.DrawLine(center + new Vector3(INDICATOR_SCALE*0.866f ,0,INDICATOR_SCALE*0.5f), center + new Vector3(INDICATOR_SCALE*0,0,INDICATOR_SCALE*1)); //NE
                Gizmos.DrawLine(center + new Vector3(INDICATOR_SCALE*0      ,0,INDICATOR_SCALE*1), center);
                Gizmos.DrawLine(center + new Vector3(INDICATOR_SCALE*0.866f ,0,INDICATOR_SCALE*0.5f), center);
                break;
            default:
                break;
        }
    }

    public static void visualizeConnectionPair(HexCoordinate coord, EntityConnectionPair pair, float height, Color color) {
        HexCoordinate inCoord = new HexCoordinate(coord.x, coord.y, HexDirectionUtil.rotate(coord.section, (int)pair.inletSegment));
        Vector2 coordIn = getTriSectionCenter(inCoord,pair.inletDirection);
        Vector3 coordIn3 = new Vector3(coordIn.x, height, coordIn.y);
        HexCoordinate outCoord = new HexCoordinate(coord.x, coord.y, HexDirectionUtil.rotate(coord.section, (int)pair.outletSegment));
        Vector2 coordOut = getTriSectionCenter(outCoord,pair.outletDirection);
        Vector3 coordOut3 = new Vector3(coordOut.x, height, coordOut.y);

        Handles.DrawBezier(coordIn3,coordOut3,coordIn3,coordOut3, color,null,6);
    }

    public static void visualizeHexLink(HexCoordinate a, HexCoordinate b, float height, Color color) {
        Vector2 ac = getHexSectionCenter(a);
        Vector3 ac3 = new Vector3(ac.x, height, ac.y);
        Vector2 bc = getHexSectionCenter(b);
        Vector3 bc3 = new Vector3(bc.x, height, bc.y);
        Handles.DrawBezier(ac3,bc3,ac3,bc3, color, null,6);
    }
    public static void visualizeTriLink(HexCoordinate a, TriDirection ad, HexCoordinate b, TriDirection bd, float height, Color color) {
        Vector2 ac = getTriSectionCenter(a,ad);
        Vector3 ac3 = new Vector3(ac.x, height, ac.y);
        Vector2 bc = getTriSectionCenter(b,bd);
        Vector3 bc3 = new Vector3(bc.x, height, bc.y);
        Handles.DrawBezier(ac3,bc3,ac3,bc3, color, null,6);
    }

    public static void visualizeFlag(HexCoordinate coord, TriDirection dir, string icon, float height, int offset) {
        Vector2 realcoord = coord.toPosition(GRID_SPACING);
        Vector3 point = new Vector3(realcoord.x, height+(offset*0.1f), realcoord.y);
        float tri = GRID_SPACING/5;
        switch (coord.section) {
            case HexDirection.HexEast:
                point += new Vector3(GRID_SPACING/2,0,0);
                switch (dir) {
                    case TriDirection.Back:
                    default:
                        point += new Vector3(tri,0,0); break;
                    case TriDirection.Left:
                        point += new Vector3(tri*-0.5f,0,tri*-0.866f); break;
                    case TriDirection.Right:
                        point += new Vector3(tri*-0.5f,0,tri*0.866f); break;
                }
                break;
            case HexDirection.HexSouthEast:
                point += new Vector3(GRID_SPACING/4,0,-0.866f*GRID_SPACING/2);
                switch (dir) {
                    case TriDirection.Left:
                    default:
                        point += new Vector3(-tri,0,0); break;
                    case TriDirection.Right:
                        point += new Vector3(tri*0.5f,0,tri*0.866f); break;
                    case TriDirection.Back:
                        point += new Vector3(tri*0.5f,0,tri*-0.866f); break;
                }
                break;
            case HexDirection.HexSouthWest:
                point += new Vector3(-GRID_SPACING/4,0,-0.866f*GRID_SPACING/2);
                switch (dir) {
                    case TriDirection.Right:
                    default:
                        point += new Vector3(tri,0,0); break;
                    case TriDirection.Back:
                        point += new Vector3(tri*-0.5f,0,tri*-0.866f); break;
                    case TriDirection.Left:
                        point += new Vector3(tri*-0.5f,0,tri*0.866f); break;
                }
                break;
            case HexDirection.HexWest:
                point += new Vector3(-GRID_SPACING/2,0,0);
                switch (dir) {
                    case TriDirection.Back:
                    default:
                        point += new Vector3(-tri,0,0); break;
                    case TriDirection.Left:
                        point += new Vector3(tri*0.5f,0,tri*0.866f); break;
                    case TriDirection.Right:
                        point += new Vector3(tri*0.5f,0,tri*-0.866f); break;
                }
                break;
            case HexDirection.HexNorthWest:
                point += new Vector3(-GRID_SPACING/4,0,0.866f*GRID_SPACING/2);
                switch (dir) {
                    case TriDirection.Left:
                    default:
                        point += new Vector3(tri,0,0); break;
                    case TriDirection.Right:
                        point += new Vector3(tri*-0.5f,0,tri*-0.866f); break;
                    case TriDirection.Back:
                        point += new Vector3(tri*-0.5f,0,tri*0.866f); break;
                }
                break;
            case HexDirection.HexNorthEast:
                point += new Vector3(GRID_SPACING/4,0,0.866f*GRID_SPACING/2);
                switch (dir) {
                    case TriDirection.Right:
                    default:
                        point += new Vector3(-tri,0,0); break;
                    case TriDirection.Back:
                        point += new Vector3(tri*0.5f,0,tri*0.866f); break;
                    case TriDirection.Left:
                        point += new Vector3(tri*0.5f,0,tri*-0.866f); break;
                }
                break;
            default:
                break;
        }
        Gizmos.DrawIcon(point, icon, true);
    }

    public static Vector2 getHexSectionCenter(HexCoordinate coord) {
        Vector2 point = coord.toPosition(GRID_SPACING);
        switch (coord.section) {
            case HexDirection.HexEast:
                point += new Vector2(GRID_SPACING/2,0);
                break;
            case HexDirection.HexSouthEast:
                point += new Vector2(GRID_SPACING/4,-0.866f*GRID_SPACING/2);
                break;
            case HexDirection.HexSouthWest:
                point += new Vector2(-GRID_SPACING/4,-0.866f*GRID_SPACING/2);
                break;
            case HexDirection.HexWest:
                point += new Vector2(-GRID_SPACING/2,0);
                break;
            case HexDirection.HexNorthWest:
                point += new Vector2(-GRID_SPACING/4,0.866f*GRID_SPACING/2);
                break;
            case HexDirection.HexNorthEast:
                point += new Vector2(GRID_SPACING/4,0.866f*GRID_SPACING/2);
                break;
            default:
                break;
        }
        return point;
    }
    public static Vector2 getTriSectionCenter(HexCoordinate coord, TriDirection dir) {
        Vector2 point = coord.toPosition(GRID_SPACING);
        float tri = GRID_SPACING/5;
        switch (coord.section) {
            case HexDirection.HexEast:
                point += new Vector2(GRID_SPACING/2,0);
                switch (dir) {
                    case TriDirection.Back:
                    default:
                        point += new Vector2(tri,0); break;
                    case TriDirection.Left:
                        point += new Vector2(tri*-0.5f,tri*-0.866f); break;
                    case TriDirection.Right:
                        point += new Vector2(tri*-0.5f,tri*0.866f); break;
                }
                break;
            case HexDirection.HexSouthEast:
                point += new Vector2(GRID_SPACING/4,-0.866f*GRID_SPACING/2);
                switch (dir) {
                    case TriDirection.Left:
                    default:
                        point += new Vector2(-tri,0); break;
                    case TriDirection.Right:
                        point += new Vector2(tri*0.5f,tri*0.866f); break;
                    case TriDirection.Back:
                        point += new Vector2(tri*0.5f,tri*-0.866f); break;
                }
                break;
            case HexDirection.HexSouthWest:
                point += new Vector2(-GRID_SPACING/4,-0.866f*GRID_SPACING/2);
                switch (dir) {
                    case TriDirection.Right:
                    default:
                        point += new Vector2(tri,0); break;
                    case TriDirection.Back:
                        point += new Vector2(tri*-0.5f,tri*-0.866f); break;
                    case TriDirection.Left:
                        point += new Vector2(tri*-0.5f,tri*0.866f); break;
                }
                break;
            case HexDirection.HexWest:
                point += new Vector2(-GRID_SPACING/2,0);
                switch (dir) {
                    case TriDirection.Back:
                    default:
                        point += new Vector2(-tri,0); break;
                    case TriDirection.Left:
                        point += new Vector2(tri*0.5f,tri*0.866f); break;
                    case TriDirection.Right:
                        point += new Vector2(tri*0.5f,tri*-0.866f); break;
                }
                break;
            case HexDirection.HexNorthWest:
                point += new Vector2(-GRID_SPACING/4,0.866f*GRID_SPACING/2);
                switch (dir) {
                    case TriDirection.Left:
                    default:
                        point += new Vector2(tri,0); break;
                    case TriDirection.Right:
                        point += new Vector2(tri*-0.5f,tri*-0.866f); break;
                    case TriDirection.Back:
                        point += new Vector2(tri*-0.5f,tri*0.866f); break;
                }
                break;
            case HexDirection.HexNorthEast:
                point += new Vector2(GRID_SPACING/4,0.866f*GRID_SPACING/2);
                switch (dir) {
                    case TriDirection.Right:
                    default:
                        point += new Vector2(-tri,0); break;
                    case TriDirection.Back:
                        point += new Vector2(tri*0.5f,tri*0.866f); break;
                    case TriDirection.Left:
                        point += new Vector2(tri*0.5f,tri*-0.866f); break;
                }
                break;
            default:
                break;
        }
        return point;
    }

}

public enum HexDirection {
    HexEast, // (1,0)
    HexSouthEast, // (1,-1)
    HexSouthWest, // (0,-1)
    HexWest, // (-1,0)
    HexNorthWest, // (0,1)
    HexNorthEast  // (1,1)
}


public static class HexDirectionUtil {
    public static HexDirection rotate(HexDirection dir, int amount) {
        return (HexDirection)(((int)dir+12+amount)%6);
    }

    public static HexCoordinate rotate(HexCoordinate coord, int amount) {
        return new HexCoordinate(coord.x, coord.y, coord.level, rotate(coord.section,amount));
    }
}

public enum HexStatus {
    LOOSE, SEEKING, ANCHORED
}