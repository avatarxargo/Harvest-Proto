using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System; 

// Structs for encoding placement conditions and requirements. 

public enum TriDirection {
    Left, Right, Back
}

public enum TriComboType {
    AT_LEAST_ONE, ALL, NONE
}

/// <summary>
/// List of requirements for placing a particular tri (subsection of hex)
///     /\
///  L /  \ R
///   /____\
///   B(ack)
/// </summary>
public abstract class TriRequirement : ScriptableObject {
    public abstract bool check(TileSystem system, HexCoordinate coord);

    /*bool drawEditor(int depth, int off, int maxwidth);
    TriRequirement getReplacement();
    int getHeight();*/
}

public struct TriRequirementTools {

    public class TriRequirementTypePopup : PopupWindowContent
    {   
        public TriRequirement req = null;

        public override Vector2 GetWindowSize()
        {
            return new Vector2(200, 150);
        }
        public override void OnGUI(Rect rect) {}
        /*public override void OnGUI(Rect rect)
        {
            GUILayout.Label("Select new type:", EditorStyles.boldLabel);
            
            if(GUILayout.Button("Multi")) {
                req = new TriRequirementCombo(TriComboType.ALL, new List<TriRequirement>{});
                this.editorWindow.Close();
            }
            /*if(GUILayout.Button("Dir")) {
                req = new TriRequirementCombo(TriComboType.ALL, new List<TriRequirement>{});
            }
            if(GUILayout.Button("Surface Type")) {
                req = new TriRequirementSurfaceType(new List<TileType> {TileType.Grass});
                this.editorWindow.Close();
            }
        }*/

        public override void OnOpen()
        {
            //Debug.Log("Popup opened: " + this);
        }

        public override void OnClose()
        {
            //Debug.Log("Popup closed: " + this);
        }
    }

    public static TriRequirement dropDownPick(TriRequirementTypePopup popup) {
        if(GUILayout.Button("Type", GUILayout.Width(50))) {
            popup.req = null;
            Rect buttonRect = GUILayoutUtility.GetLastRect();
            PopupWindow.Show(buttonRect, popup);
        }
        if(popup.req != null) {
            TriRequirement tmp = popup.req;
            popup.req = null;
            return tmp;
        }
        return null;
    } 
}

// ===============================================================================
/*
public class TriRequirementCombo : ScriptableObject, TriRequirement {
    TriComboType comboType;
    List<TriRequirement> comboRequirements;
    bool ed_foldout;
    public TriRequirement replacement;
    public TriRequirementTools.TriRequirementTypePopup switcher;

    public TriRequirementCombo(TriComboType _comboType, List<TriRequirement> _comboRequirements) {
        comboType = _comboType;
        comboRequirements = _comboRequirements;
        ed_foldout = true;
        replacement = null;
        switcher = new TriRequirementTools.TriRequirementTypePopup();
    }
    public TriRequirement getReplacement() {
        TriRequirement tmp = replacement;
        replacement = null;
        return tmp;
    }

    public int getHeight() {
        int total = 70;
        if(ed_foldout)
            foreach(TriRequirement req in comboRequirements) {
                total += req.getHeight();
            }
        return total;
    }

    public bool check(TileSystem system, HexCoordinate coord) {
        return true;
    }
    public bool drawEditor(int depth, int off, int maxwidth) {
        EditorGUI.indentLevel = depth;
        //
        EditorGUILayout.BeginHorizontal();
        ed_foldout = EditorGUILayout.Foldout(ed_foldout, "Multiple");
        replacement = TriRequirementTools.dropDownPick(switcher);
        if(depth > 0)
            if(GUILayout.Button("X", GUILayout.Width(20))) {
                EditorGUILayout.EndHorizontal();
                return true;
            }
        EditorGUILayout.EndHorizontal();
        //
        if(ed_foldout) {
            comboType = (TriComboType)EditorGUILayout.EnumPopup("combo type",comboType);
            EditorGUILayout.Separator();
            int cumulativeOffset = off;
            foreach(TriRequirement req in comboRequirements) {
                EditorGUI.DrawRect(new Rect(depth*20+5, cumulativeOffset+50+5, maxwidth-depth*20-10, req.getHeight()-10), new Color(0.27f,0.00f,0.40f));
                GUILayout.BeginArea(new Rect(depth*20, cumulativeOffset+50, maxwidth-depth*20, req.getHeight()));
                if(req.drawEditor(depth + 1, cumulativeOffset, maxwidth)) {
                    cumulativeOffset += req.getHeight();
                    comboRequirements.Remove(req);
                    GUILayout.EndArea();
                    continue;
                } else 
                    cumulativeOffset += req.getHeight();
                TriRequirement rep = req.getReplacement();
                if(rep != null) {
                    comboRequirements.Insert(comboRequirements.IndexOf(req),rep);
                    comboRequirements.Remove(req);
                }
                GUILayout.EndArea();
                EditorGUILayout.Separator();
            }
            GUILayout.Space(cumulativeOffset-off);
            if(GUILayout.Button("+")) {
                comboRequirements.Add(new TriRequirementSurfaceType(new List<TileType> {TileType.Grass}));
            }
        }
        return false;
    }
}

// ===============================================================================
public class TriRequirementDir : ScriptableObject, TriRequirement {
    TriDirection direction;
    TriRequirement requirement;
    bool ed_foldout;
    public TriRequirement replacement;
    public TriRequirementTools.TriRequirementTypePopup switcher;

    public TriRequirementDir(TriDirection _direction, TriRequirement _requirement) {
        direction = _direction;
        requirement = _requirement;
        ed_foldout = true;
        replacement = null;
        switcher = new TriRequirementTools.TriRequirementTypePopup();
    }
    public TriRequirement getReplacement() {
        TriRequirement tmp = replacement;
        replacement = null;
        return tmp;
    }

    public int getHeight() {
        return 500;
    }
    public bool check(TileSystem system, HexCoordinate coord) {
        return true;
    }
    public bool drawEditor(int depth, int off, int maxwidth) {
        EditorGUI.indentLevel = depth;
        return false;
    }
}

// ===============================================================================
public class TriRequirementSurfaceType : ScriptableObject, TriRequirement {
    List<TileType> types;
    TriComboType condition;
    bool ed_foldout;
    public TriRequirement replacement;
    public TriRequirementTools.TriRequirementTypePopup switcher;
    public TriRequirementSurfaceType(List<TileType> _types) {
        condition = TriComboType.AT_LEAST_ONE;
        types = _types;
        ed_foldout = true;
        replacement = null;
        switcher = new TriRequirementTools.TriRequirementTypePopup();
    }
    public TriRequirement getReplacement() {
        TriRequirement tmp = replacement;
        replacement = null;
        return tmp;
    }

    public int getHeight() {
        if(ed_foldout)
            return 130;
        else
            return 25;
    }
    public bool check(TileSystem system, HexCoordinate coord) {
        return true;
    }

    public bool drawEditor(int depth, int off, int maxwidth) {
        EditorGUI.indentLevel = 2*depth;
        //
        EditorGUILayout.BeginHorizontal();
        ed_foldout = EditorGUILayout.Foldout(ed_foldout, "Surface Type");
        replacement = TriRequirementTools.dropDownPick(switcher);
        if(replacement!=null) {
            Debug.Log("REPLACE"+replacement.ToString());
        }
        if(depth > 0)
            if(GUILayout.Button("X", GUILayout.Width(20))) {
                EditorGUILayout.EndHorizontal();
                return true;
            }
        EditorGUILayout.EndHorizontal();
        //
        if(ed_foldout) {
            condition = (TriComboType)EditorGUILayout.EnumPopup("condition type",condition);
            foreach(TileType type in Enum.GetValues(typeof(TileType))) {
                bool val = types.Contains(type);
                bool res = EditorGUILayout.Toggle(type.ToString(), val);
                if(val && !res) {
                    types.Remove(type);
                }
                if(!val && res) {
                    types.Add(type);
                }
            }
        }
        return false;
    }
}
*/