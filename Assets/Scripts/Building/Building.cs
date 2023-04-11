using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    private Renderer _renderer;
    private Material _defaultMaterial;

    private bool _flaggedForDelete;
    public bool FlaggedForDelete => _flaggedForDelete;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponentInChildren<Renderer>();
        if (_renderer) _defaultMaterial = _renderer.material;
    }

    public void UpdateMaterial(Material newMaterial)
    {
        if (_renderer.material != newMaterial)  _renderer.material = newMaterial;
    }

    public void PlaceBuilding()
    {
        _renderer.material = _defaultMaterial;
    }

    public void FlagForDelete(Material deleteMat)
    {
        UpdateMaterial(deleteMat);
        _flaggedForDelete = true;
    }

    public void RemoceDeleteFlag()
    {
        UpdateMaterial(_defaultMaterial);
        _flaggedForDelete = false;
    }
}
