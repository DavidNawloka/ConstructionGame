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
            meshRenderer.sharedMaterial.mainTexture = texture;
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            meshFilter.mesh = BuildingGridAssetManager.GetGridMesh();

            ToggleMesh(false);
        }

        public void UpdateTexture(Texture2D updatedTexture)
        {
            meshRenderer.material.mainTexture = updatedTexture;
            meshRenderer.UpdateGIMaterials();
        }

        public void ToggleMesh(bool isActive)
        {
            meshRenderer.enabled = isActive;
        }
    }

}