using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    static readonly int positionsID = Shader.PropertyToID("_Positions");
    static readonly int resolutionID = Shader.PropertyToID("_Resolution");
    static readonly int stepID = Shader.PropertyToID("_Step");
    static readonly int timeID = Shader.PropertyToID("_Time");

    public const int CHUNK_SIZE = 64;
    public const int CHUNK_BLOCK_AMOUNT = CHUNK_SIZE * CHUNK_SIZE;

    [Range(1, maxResolution)]
    public int resolution = 100;
    public const int maxResolution = 1000;

    [SerializeField]
    private ComputeShader computeShader = default;

    [SerializeField]
    private Material material = default;

    [SerializeField]
    private Mesh mesh = default;

    private ComputeBuffer positionsBuffer;
    private ComputeBuffer argsBuffer;

    private void OnEnable() 
    {
        positionsBuffer = new ComputeBuffer(maxResolution * maxResolution, 3 * 4);
        argsBuffer = new ComputeBuffer(3, sizeof(int));

        computeShader.SetBuffer(0, positionsID, positionsBuffer);
        material.SetBuffer(positionsID, positionsBuffer);
    }

    private void OnDisable() 
    {
        positionsBuffer.Release();
        positionsBuffer = null;

        argsBuffer.Release();
        argsBuffer = null;
    }

    private void UpdateOnGPU() 
    {
        var step = 2f / resolution;
        var groups  = Mathf.CeilToInt(resolution / 8f);

        computeShader.SetInt(resolutionID, resolution);
        computeShader.SetFloat(stepID, step);
        computeShader.SetFloat(timeID, Time.time);

        argsBuffer.SetData(new int[] { groups, groups, 1 }, 0, 0, 3);
        computeShader.DispatchIndirect(0, argsBuffer, 0);

		material.SetFloat(stepID, step);

        var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / resolution));
        Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, resolution * resolution);
    }

    private void Update()
    {
        UpdateOnGPU();
    }
}
