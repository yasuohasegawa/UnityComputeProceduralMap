using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCompute : MonoBehaviour {
    public Mesh mesh;
    public Material material;
    public ComputeShader computeShader;

    private Bounds bounds;
    private ComputeBuffer argsBuffer;
    private uint[] args = new uint[5];

    public void SetUp(uint num)
    {
        bounds = new Bounds(Vector3.zero, new Vector3(num / 3, num / 3, num / 3));

        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        args[0] = mesh.GetIndexCount(0);
        args[1] = num;
        args[2] = mesh.GetIndexStart(0);
        args[3] = mesh.GetBaseVertex(0);
        args[4] = 0;
        argsBuffer.SetData(args);
    }

    public void Draw()
    {
        Graphics.DrawMeshInstancedIndirect(mesh, 0, material, bounds, argsBuffer);
    }

    public void Dispose()
    {
        argsBuffer.Release();
    }
}
