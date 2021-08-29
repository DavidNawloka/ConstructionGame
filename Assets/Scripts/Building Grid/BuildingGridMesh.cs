using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.BuildingGrid
{
    public class BuildingGridMesh : MonoBehaviour
    {
        [SerializeField] Material material;

        MeshRenderer meshRenderer;

        public void InitiatePlane(Texture2D texture)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.material = material;
            meshRenderer.material.mainTexture = texture;
            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
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