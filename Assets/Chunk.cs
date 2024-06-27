using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;

public class Chunk : MonoBehaviour
{
    private Voxel[,,] voxels;
    private int chunkSize = 16;

    public void InitializeChunk(int _)
    {
        this.chunkSize = _;
        voxels = new Voxel[chunkSize, chunkSize, chunkSize];
        InitializeVoxels();
        GenerateMesh();
    }

    private void InitializeVoxels()
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    voxels[x, y, z] = new Voxel(position: transform.position + new Vector3(x, y, z), isActive: true);
                }
            }
        }
    }

    private void GenerateMesh()
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    if (voxels[x, y, z].isActive)
                    {
                        GenerateMeshForVoxel(vertices, triangles, uvs, x, y, z);
                    }
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.material = new Material(Shader.Find("Standard"));
        }
    }

    private void GenerateMeshForVoxel(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs, int x, int y, int z)
    {
        Vector3 voxelPosition = new Vector3(x, y, z);

        if (IsFaceExposed(x, y + 1, z)) // Top face
        {
            AddFace(vertices, triangles, uvs, voxelPosition, Vector3.up);
        }
        if (IsFaceExposed(x, y - 1, z)) // Bottom face
        {
            AddFace(vertices, triangles, uvs, voxelPosition, Vector3.down);
        }
        if (IsFaceExposed(x - 1, y, z)) // Left face
        {
            AddFace(vertices, triangles, uvs, voxelPosition, Vector3.left);
        }
        if (IsFaceExposed(x + 1, y, z)) // Right face
        {
            AddFace(vertices, triangles, uvs, voxelPosition, Vector3.right);
        }
        if (IsFaceExposed(x, y, z + 1)) // Front face
        {
            AddFace(vertices, triangles, uvs, voxelPosition, Vector3.forward);
        }
        if (IsFaceExposed(x, y, z - 1)) // Back face
        {
            AddFace(vertices, triangles, uvs, voxelPosition, Vector3.back);
        }
    }

    private void AddFace(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs, Vector3 position, Vector3 direction)
    {
        Vector3[] faceVertices = new Vector3[4];
        if (direction == Vector3.up)
        {
            faceVertices[0] = position + new Vector3(0.5f, 0.5f * direction.y, -0.5f);
            faceVertices[1] = position + new Vector3(-0.5f, 0.5f * direction.y, -0.5f);
            faceVertices[2] = position + new Vector3(-0.5f, 0.5f * direction.y, 0.5f);
            faceVertices[3] = position + new Vector3(0.5f, 0.5f * direction.y, 0.5f);
        }
        else if(direction == Vector3.down)
        {
            faceVertices[0] = position + new Vector3(-0.5f, 0.5f * direction.y, -0.5f);
            faceVertices[1] = position + new Vector3(0.5f, 0.5f * direction.y, -0.5f);
            faceVertices[2] = position + new Vector3(0.5f, 0.5f * direction.y, 0.5f);
            faceVertices[3] = position + new Vector3(-0.5f, 0.5f * direction.y, 0.5f);
        }
        else if (direction == Vector3.left) 
        {
            faceVertices[0] = position + new Vector3(-0.5f, -0.5f, -0.5f);
            faceVertices[1] = position + new Vector3(-0.5f, -0.5f, 0.5f);
            faceVertices[2] = position + new Vector3(-0.5f, 0.5f, 0.5f);
            faceVertices[3] = position + new Vector3(-0.5f, 0.5f, -0.5f);
        }
        else if (direction == Vector3.right)
        {
            faceVertices[0] = position + new Vector3(0.5f, -0.5f, -0.5f);
            faceVertices[1] = position + new Vector3(0.5f, 0.5f, -0.5f);
            faceVertices[2] = position + new Vector3(0.5f, 0.5f, 0.5f);
            faceVertices[3] = position + new Vector3(0.5f, -0.5f, 0.5f);
        }
        else if (direction == Vector3.forward) 
        {
            faceVertices[0] = position + new Vector3(-0.5f, -0.5f, 0.5f);
            faceVertices[1] = position + new Vector3(0.5f, -0.5f, 0.5f);
            faceVertices[2] = position + new Vector3(0.5f, 0.5f, 0.5f);
            faceVertices[3] = position + new Vector3(-0.5f, 0.5f, 0.5f);
        }
        else if (direction == Vector3.back)         {
            faceVertices[0] = position + new Vector3(-0.5f, -0.5f, -0.5f);
            faceVertices[1] = position + new Vector3(-0.5f, 0.5f, -0.5f);
            faceVertices[2] = position + new Vector3(0.5f, 0.5f, -0.5f);
            faceVertices[3] = position + new Vector3(0.5f, -0.5f, -0.5f);
        }

        int vertexIndex = vertices.Count;
        vertices.AddRange(faceVertices);
        triangles.Add(vertexIndex + 0);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 0);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);

        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(1, 0));
        uvs.Add(new Vector2(1, 1));
        uvs.Add(new Vector2(0, 1));
    }

    private bool IsFaceExposed(int x, int y, int z)
    {
       
        if (x < 0 || x >= chunkSize || y < 0 || y >= chunkSize || z < 0 || z >= chunkSize)
        {
            return true;
        }
        return !voxels[x, y, z].isActive;
    }

    public void SetVoxelActive(int x, int y, int z, bool isActive)
    {
        if (x >= 0 && x < chunkSize && y >= 0 && y < chunkSize && z >= 0 && z < chunkSize)
        {
            voxels[x, y, z].isActive = isActive;
            GenerateMesh(); 
        }
    }

}
