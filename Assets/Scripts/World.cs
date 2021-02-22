using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    // Shader properties ID
    private static readonly int positionsID = Shader.PropertyToID("_Positions"),
                                resolutionID = Shader.PropertyToID("_Resolution"),
                                stepID = Shader.PropertyToID("_Step"),
                                timeID = Shader.PropertyToID("_Time");

    // The maximum amount of cubes per axis (x and y)
    private const int MAX_RESOLUTION = 1000;
    // The maximum amount of instances.
    private const int MAX_INSTANCES = MAX_RESOLUTION * MAX_RESOLUTION;

    // The current amount of cubes.
    // This is variable ! 😉
    [Range(1, MAX_RESOLUTION)]
    public int resolution = 100;
    
    // The shader that processing
    // cubes data.
    [SerializeField]
    private ComputeShader computeShader = default;

    // The material of each cube.
    // (is the same for all)
    [SerializeField]
    private Material material = default;

    // The mesh that must be instanced.
    // (default is the cube mesh)
    [SerializeField]
    private Mesh mesh = default;

    // Buffer that contains all cube position.
    private ComputeBuffer positionsBuffer;

    // Buffer that contains the amount of threads
    // group.
    private ComputeBuffer argsBuffer;

    private void OnEnable() 
    {
        positionsBuffer = new ComputeBuffer(MAX_INSTANCES, 3 * sizeof(float));
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
