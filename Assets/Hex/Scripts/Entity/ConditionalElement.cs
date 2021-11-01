using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An object which can be displayed under a condition.
/// </summary>

public class ConditionalElement : MonoBehaviour
{
    public TriRequirement requirement;
    public MeshRenderer mesh;

    public bool isVisible = true;

    public void initialize() {
        mesh = this.GetComponent<MeshRenderer>();
        mesh.enabled = isVisible;
    }

    public void refresh(TileSystem system, HexCoordinate coord) {
        isVisible = requirement.check(system, coord);
        mesh.enabled = isVisible;
    }
    private void OnValidate() {
        initialize();
    }
}