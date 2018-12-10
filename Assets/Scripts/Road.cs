using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Road : BaseCompute
{
    const int BLOCK_SIZE = 16;

    private ComputeBuffer positionBuffer;
    private ComputeBuffer argsBuffer;
    private uint quadsSize = 150;
    private uint numeberOfQuads;

    private Vector3[] positions;


    void Awake()
    {
        numeberOfQuads = quadsSize * quadsSize;
    }

    // Use this for initialization
    void Start()
    {
        // 12 = float (4 byte x 3）
        positionBuffer = new ComputeBuffer((int)numeberOfQuads, 12);
        positions = new Vector3[numeberOfQuads];

        float size = 0.1f;
        for (int i = 0; i < numeberOfQuads; i++)
        {
            Vector3 pos = Vector3.zero;
            pos.x = (Mathf.Floor((float)i % (float)quadsSize) * size) - (quadsSize * 0.5f)* size;
            pos.y = 0f;
            pos.z = (Mathf.Floor((float)i / (float)quadsSize) * size) - (quadsSize * 0.5f)* size;

            //Debug.Log(pos);

            positions[i] = pos;
        }

        positionBuffer.SetData(positions);
        material.SetBuffer("PositionBuffer", positionBuffer);

        SetUp(numeberOfQuads);
    }

    void Update()
    {
        int kernelId = computeShader.FindKernel("CSMain");
        computeShader.SetBuffer(kernelId, "PositionBuffer", positionBuffer);

        int groupSize = Mathf.CeilToInt(numeberOfQuads / BLOCK_SIZE);
        computeShader.Dispatch(kernelId, groupSize, 1, 1);
        computeShader.SetFloat("_Time", Time.time);

        Draw();
    }

    void OnDestroy()
    {
        positionBuffer.Release();

        Dispose();
    }
}
