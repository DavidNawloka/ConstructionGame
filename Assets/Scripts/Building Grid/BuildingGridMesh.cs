using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.BuildingGrid
{
    [ExecuteAlways]
    public class BuildingGridMesh : MonoBehaviour
    {
        [SerializeField] Material material;

        MeshRenderer meshRenderer;

        public void InitiatePlane(Texture2D texture)
        {
            meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.material = material;
            
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            meshFilter.mesh = BuildingGridAssetManager.GetGridMesh();
            UpdateTexture(texture);

            SetActiveMesh(false);
        }

        public void UpdateTexture(Texture2D updatedTexture)
        {
            meshRenderer.material.mainTexture = updatedTexture;
            meshRenderer.UpdateGIMaterials();
        }

        public void SetActiveMesh(bool isActive)
        {
            meshRenderer.enabled = isActive;
        }
        public bool IsActive()
        {
            return meshRenderer.enabled;
        }
    }

}