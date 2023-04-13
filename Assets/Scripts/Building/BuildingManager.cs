using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance;

    public GameObject[] objects;
    public GameObject pendingObject;
    [SerializeField] private Material[] materials;

    private Vector3 pos;

    private RaycastHit hit;
    [SerializeField] public LayerMask placementLayermask;
    [SerializeField] public LayerMask selectingLayermask;

    public float rotateAmount;

    public float gridSize;
    bool gridOn = true;
    public bool canPlace = true;

    [SerializeField] private Toggle gridToggle;

    [SerializeField] public Transform _rayOrigin;
    [SerializeField] public float _rayDistance;
    public bool isThirdPersonCam;
    public Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
        isThirdPersonCam = true;
        if (Instance == null)
            Instance = this;
    }

    void Update()
    {
        if (pendingObject != null)
        {
            if (gridOn)
                pendingObject.transform.position = new Vector3(
                    RoundToNearestGrid(pos.x),
                    RoundToNearestGrid(pos.y),
                    RoundToNearestGrid(pos.z)
                    );
            else
                pendingObject.transform.position = pos;


            if (Input.GetMouseButtonDown(0) && canPlace)
            {
                PlaceObject();
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                RotateObject();
            }

            if (pendingObject != null)
            {
                UpdateMaterials();
            }
        }    
    }
    private void FixedUpdate()
    {
        Ray ray = isThirdPersonCam ? new Ray(_rayOrigin.position, _camera.transform.forward * _rayDistance) : _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, (isThirdPersonCam ? _rayDistance : 1000), placementLayermask))
        {
            pos = hit.point;
        }
    }

    public void PlaceObject()
    {
        pendingObject.GetComponent<MeshRenderer>().material = materials[2];
        pendingObject = null;
    }

    public void RotateObject()
    {
        pendingObject.transform.Rotate(Vector3.up, rotateAmount);
    }

    public void SelectObject(int index)
    {
        pendingObject = Instantiate(objects[index], pos, transform.rotation);
    }

    
    private void UpdateMaterials()
    {
        if (canPlace)
            pendingObject.GetComponent<MeshRenderer>().material = materials[0];
        else
            pendingObject.GetComponent<MeshRenderer>().material = materials[1];
    }

    public void ToggleGrid()
    {
        if (gridToggle.isOn)
            gridOn = true;
        else
            gridOn = false;
    }

    float RoundToNearestGrid(float pos)
    {
        float xDiff = pos % gridSize;
        pos -= xDiff;
        if (xDiff > (gridSize / 2))
        {
            pos += gridSize;
        }
        return pos;
    }
}
