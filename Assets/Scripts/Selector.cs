using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using static UnityEditor.PlayerSettings;

public class Selector : MonoBehaviour
{
    public GameObject selectedObject;
    public TextMeshProUGUI objNameText;
    private BuildingManager buildingManager;
    public GameObject objUi;


    // Start is called before the first frame update
    void Start()
    {
        buildingManager = BuildingManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetMouseButtonDown(0))
        // {
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //RaycastHit hit;

        Ray ray = buildingManager.isThirdPersonCam ?
            new Ray(buildingManager._rayOrigin.position, buildingManager._camera.transform.forward * buildingManager._rayDistance)
            : buildingManager._camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, (buildingManager.isThirdPersonCam ? buildingManager._rayDistance : 1000), buildingManager.selectingLayermask) && buildingManager.pendingObject is null)
        {
            if (hit.collider.gameObject.CompareTag("Object") && selectedObject == null)
            {
                Select(hit.collider.gameObject);
            }
            /*else if (hit.collider.gameObject is null && selectedObject != null)
            {
                Deselect();
            }*/
            else if (hit.collider.gameObject.CompareTag("Object") && selectedObject == hit.collider.gameObject && Input.GetKeyDown(KeyCode.R))
            {
                Move();
            }
            else if (Input.GetKeyDown(KeyCode.E) && selectedObject == hit.collider.gameObject)
            {
                Delete();
            }
        }

        if (Physics.Raycast(ray, out hit, (buildingManager.isThirdPersonCam ? buildingManager._rayDistance : 1000)) && selectedObject != null && buildingManager.pendingObject is null)
        {
            if (hit.collider.gameObject is null || hit.collider.gameObject != selectedObject &&  selectedObject != null)
            {
                Deselect();
            }
        }




        //}
        /*
        if (Input.GetMouseButtonDown(1) && selectedObject != null)
        {
            Deselect();
        }
        */
    }

    private void Select(GameObject go)
    {
        if (go == selectedObject) return;
        //if (selectedObject != null) Deselect();

        Outline outline = go.GetComponent<Outline>();

        if (outline == null)
            go.AddComponent<Outline>();
        else
            outline.enabled = true;

        objNameText.text = go.name;
        objUi.SetActive(true);
        selectedObject = go;
    }

    private void Deselect()
    {
        objUi.SetActive(false);
        selectedObject.GetComponent<Outline>().enabled = false;
        selectedObject = null;
    }

    public void Move()
    {
        if (buildingManager.pendingObject is null)
            buildingManager.pendingObject = selectedObject;
    }

    public void Delete()
    {
        GameObject objToDestroy = selectedObject;
        Deselect();
        Destroy(objToDestroy);
    }
}
