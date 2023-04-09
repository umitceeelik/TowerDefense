using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

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
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000))
            {
                if (hit.collider.gameObject.CompareTag("Object"))
                {
                    Select(hit.collider.gameObject);
                }
            }
        }

        if (Input.GetMouseButtonDown(1) && selectedObject != null)
        {
            Deselect();
        }
    }

    private void Select(GameObject go)
    {
        if (go == selectedObject) return;
        if (selectedObject != null) Deselect();
        
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
        buildingManager.pendingObject = selectedObject;
    }

    public void Delete()
    {
        GameObject objToDestroy = selectedObject;
        Deselect();
        Destroy(objToDestroy);
    }
}
