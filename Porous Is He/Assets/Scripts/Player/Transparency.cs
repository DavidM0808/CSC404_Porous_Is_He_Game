using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transparency : MonoBehaviour
{
    // This script is attached to Po

    public bool aiming = false;

    [SerializeField] private LayerMask playerLayerMask = new LayerMask();
    [SerializeField] private SkinnedMeshRenderer meshRenderer;

    [Header("Transparent Materials")]
    [SerializeField] private Material transparentMaterial;
    private Material originalMaterial;
    private bool changed = true;

    void Start()
    {
        originalMaterial = meshRenderer.material;
        transparentMaterial.mainTextureScale = originalMaterial.mainTextureScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (aiming)
        {
            CheckForTransparency();
            changed = true;
        } else
        {
            ResetTransparency();
        }

    }

    private void CheckForTransparency()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, playerLayerMask))
        {
            meshRenderer.material = transparentMaterial;
        }
        else
        {
            meshRenderer.material = originalMaterial;
        }
    }

    private void ResetTransparency()
    {
        if (changed)
        {
            meshRenderer.material = originalMaterial;
            changed = false;
        }
    }

}
