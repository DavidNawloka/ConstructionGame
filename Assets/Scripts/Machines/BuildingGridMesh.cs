using CON.Elements;
using CON.Machines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Machines
{
    public class BuildingGridMesh : MonoBehaviour
    {
        [SerializeField] Material material;

        MeshRenderer meshRenderer;

        public void BuildPlane(int width, int height, Texture2D texture)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.material = material;
            meshRenderer.material.mainTexture = texture;


            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();

            Mesh mesh = new Mesh();

            Vector3[] vertices = new Vector3[4]
            {
            new Vector3(0, 0, 0),
            new Vector3(width, 0, 0),
            new Vector3(0, 0, height),
            new Vector3(width, 0, height),
            };
            Vector2[] uv = new Vector2[4]
            {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
            };
            int[] tris = new int[6]
            {
            0, 2, 1,
            2, 3, 1
            };
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = tris;

            mesh.RecalculateNormals();
            meshFilter.mesh = mesh;

            ToggleMesh(false);
        }

        public void UpdateTexture(Texture2D updatedTexture)
        {
            meshRenderer.material.mainTexture = updatedTexture;
        }

        public void ToggleMesh(bool isActive)
        {
            meshRenderer.enabled = isActive;
        }
    }

}