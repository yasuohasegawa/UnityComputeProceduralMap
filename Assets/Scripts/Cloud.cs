using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Cloud : BaseCompute
{
    const int BLOCK_SIZE = 10;

    private ComputeBuffer positionBuffer;
    private ComputeBuffer alphaBuffer;
    private ComputeBuffer rotationMatrixBuffer;
    private uint numeberOfQuads = 100;

    private Vector3[] positions;
    private Vector3 maxPos = new Vector3(15f, 0f, 15f);

    void Awake()
    {

    }

    // Use this for initialization
    void Start()
    {
        // 12 = float (4 byte x 3）
        positionBuffer = new ComputeBuffer((int)numeberOfQuads, 12);
        alphaBuffer = new ComputeBuffer((int)numeberOfQuads, Marshal.SizeOf(typeof(float)));
        rotationMatrixBuffer = new ComputeBuffer((int)numeberOfQuads, Marshal.SizeOf(typeof(Matrix4x4)));
        positions = new Vector3[numeberOfQuads];

        Matrix4x4[] mat = new Matrix4x4[numeberOfQuads];
        float[] alphas = new float[numeberOfQuads];
        for (int i = 0; i < numeberOfQuads; i++)
        {
            positions[i] = new Vector3(Random.Range(-maxPos.x, maxPos.x), maxPos.y, Random.Range(-maxPos.z, maxPos.z));
            mat[i] = Matrix4x4.identity;
            alphas[i] = 1.0f;
        }

        positionBuffer.SetData(positions);
        alphaBuffer.SetData(alphas);
        rotationMatrixBuffer.SetData(mat);
        material.SetBuffer("PositionBuffer", positionBuffer);
        material.SetBuffer("AlphaBuffer", alphaBuffer);
        material.SetBuffer("RotationMatrixBuffer", rotationMatrixBuffer);

        SetUp(numeberOfQuads);
    }

    void Update()
    {
        int kernelId = computeShader.FindKernel("CSMain");
        computeShader.SetBuffer(kernelId, "PositionBuffer", positionBuffer);
        computeShader.SetBuffer(kernelId, "AlphaBuffer", alphaBuffer);
        computeShader.SetBuffer(kernelId, "RotationMatrixBuffer", rotationMatrixBuffer);

        int groupSize = Mathf.CeilToInt(numeberOfQuads / BLOCK_SIZE);
        computeShader.Dispatch(kernelId, groupSize, 1, 1);
        computeShader.SetFloat("_Time", Time.time);

        Draw();
    }

    void OnDestroy()
    {
        positionBuffer.Release();
        alphaBuffer.Release();
        rotationMatrixBuffer.Release();

        Dispose();
    }
}