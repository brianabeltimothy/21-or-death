using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OutlineSelection : MonoBehaviour
{
    [SerializeField] private Material highlightMaterial;
    private Material originalMaterial;

    void Start()
    {
    
    }

    void Update()
    {
        // Create a ray from the mouse pointer
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Declare a RaycastHit object to store the hit information
        RaycastHit hit;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObject = hit.collider.gameObject;

            // Check if the hit object has the "Selectable" tag
            if (hitObject.CompareTag("Selectable"))
            {
                // Store the original material if it hasn't been stored yet
                if (originalMaterial == null)
                    originalMaterial = hitObject.GetComponent<Renderer>().material;

                // Change material to highlightMaterial
                hitObject.GetComponent<Renderer>().material = highlightMaterial;
            }
            else
            {
                // Revert material back to originalMaterial if it's not null
                if (originalMaterial != null)
                    hitObject.GetComponent<Renderer>().material = originalMaterial;
            }
        }
        else
        {
            // Revert material back to originalMaterial if the ray doesn't hit anything
            if (originalMaterial != null)
                GetComponent<Renderer>().material = originalMaterial;
        }
    }

}
