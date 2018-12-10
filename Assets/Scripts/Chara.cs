using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Actually, you can do this kind of things with the Geometry Shader, but it is what it is.
// There is a good point about that you can replace any mesh you want.
public class Chara : BaseCompute
{
    const int BLOCK_SIZE = 16;

    private ComputeBuffer positionBuffer;
    private uint numeberOfMeshes;

    private Vector3[] positions;

    private Mesh snapShotMesh;
    private SkinnedMeshRenderer skinned;

    private Matrix4x4[] boneMatrices;
    private Matrix4x4[] vertexMatrices;
    private Matrix4x4[] bindposes;
    private BoneWeight[] boneWeightes;
    private Vector3[] vertices;
    private Transform[] bones;


    void Awake()
    {
        skinned = GetComponent<SkinnedMeshRenderer>();
        numeberOfMeshes = (uint)skinned.sharedMesh.vertexCount;
        boneMatrices = new Matrix4x4[skinned.bones.Length];
        vertexMatrices = new Matrix4x4[numeberOfMeshes];
        boneWeightes = new BoneWeight[numeberOfMeshes];

        bones = skinned.bones;
    }

    // Use this for initialization
    void Start()
    {
        // 12 = float (4 byte x 3）
        positionBuffer = new ComputeBuffer((int)numeberOfMeshes, 12);
        positions = new Vector3[numeberOfMeshes];

        snapShotMesh = skinned.sharedMesh;
        vertices = snapShotMesh.vertices;
        bindposes = snapShotMesh.bindposes;
        //skinned.BakeMesh(snapShotMesh);

        for (int i = 0; i < numeberOfMeshes; i++)
        {
            Vector3 vert = snapShotMesh.vertices[i];
            positions[i] = vert;

            vertexMatrices[i] = new Matrix4x4();

            boneWeightes[i] = snapShotMesh.boneWeights[i];
        }

        positionBuffer.SetData(positions);
        material.SetBuffer("PositionBuffer", positionBuffer);

        SetUp(numeberOfMeshes);
    }

    void Update()
    {
        for (int i = 0; i < boneMatrices.Length; i++)
        {
            boneMatrices[i] = bones[i].localToWorldMatrix * bindposes[i];
        }
        
        for (int b = 0; b < numeberOfMeshes; b++){

            BoneWeight weight = boneWeightes[b];

            Matrix4x4 bm0 = boneMatrices[weight.boneIndex0];
            Matrix4x4 bm1 = boneMatrices[weight.boneIndex1];
            Matrix4x4 bm2 = boneMatrices[weight.boneIndex2];
            Matrix4x4 bm3 = boneMatrices[weight.boneIndex3];
            
            vertexMatrices[b] = Matrix4x4.identity;

            for (int n = 0; n < 16; n++){
                vertexMatrices[b][n] =
                    bm0[n] * weight.weight0 +
                    bm1[n] * weight.weight1 +
                    bm2[n] * weight.weight2 +
                    bm3[n] * weight.weight3;
            }

            positions[b] = vertexMatrices[b].MultiplyPoint3x4(vertices[b]);

        }

        positionBuffer.SetData(positions);

        int kernelId = computeShader.FindKernel("CSMain");
        computeShader.SetBuffer(kernelId, "PositionBuffer", positionBuffer);

        int groupSize = Mathf.CeilToInt(numeberOfMeshes / BLOCK_SIZE);
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
