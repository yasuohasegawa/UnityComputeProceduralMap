using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Particle : BaseCompute
{
    private ComputeBuffer positionBuffer;
    private ComputeBuffer positionOffsetBuffer;
    private ComputeBuffer rotationMatrixBuffer;
    private uint numeberOfQuads = 30;

    private Vector3[] positions;
    private Vector3 maxPos = new Vector3(10f, 1.5f, 10f);


    void Awake()
    {

    }

    // Use this for initialization
    void Start()
    {
        // 12 = float (4 byte x 3）
        positionBuffer = new ComputeBuffer((int)numeberOfQuads, 12);
        positionOffsetBuffer = new ComputeBuffer((int)numeberOfQuads, 12);
        rotationMatrixBuffer = new ComputeBuffer((int)numeberOfQuads, Marshal.SizeOf(typeof(Matrix4x4)));
        positions = new Vector3[numeberOfQuads];

        Matrix4x4[] mat = new Matrix4x4[numeberOfQuads];
        for (int i = 0; i < numeberOfQuads; i++)
        {
            positions[i] = new Vector3(Random.Range(-maxPos.x, maxPos.x), Random.Range(maxPos.y, maxPos.y+1f), Random.Range(-maxPos.z, maxPos.z));
            mat[i] = Matrix4x4.identity;
        }

        positionBuffer.SetData(positions);
        positionOffsetBuffer.SetData(positions);
        rotationMatrixBuffer.SetData(mat);
        material.SetBuffer("PositionBuffer", positionBuffer);
        material.SetBuffer("PositionOffsetBuffer", positionOffsetBuffer);
        material.SetBuffer("RotationMatrixBuffer", rotationMatrixBuffer);

        SetUp(numeberOfQuads);
    }

    void Update()
    {
        int kernelId = computeShader.FindKernel("CSMain");
        computeShader.SetBuffer(kernelId, "PositionBuffer", positionBuffer);
        computeShader.SetBuffer(kernelId, "PositionOffsetBuffer", positionOffsetBuffer);
        computeShader.SetBuffer(kernelId, "RotationMatrixBuffer", rotationMatrixBuffer);

        int groupSize = Mathf.CeilToInt(numeberOfQuads);
        computeShader.Dispatch(kernelId, groupSize, 1, 1);
        computeShader.SetFloat("_Time", Time.time);

        Draw();
    }

    void OnDestroy()
    {
        positionBuffer.Release();
        positionOffsetBuffer.Release();
        rotationMatrixBuffer.Release();

        Dispose();
    }
}
