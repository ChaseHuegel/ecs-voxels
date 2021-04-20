using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Assertions;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;

namespace Swordfish.ecs
{

public struct RenderColor : IComponentData
{
    public Vector4 Value;
}

[AlwaysUpdateSystem]
public class RenderSystem : ComponentSystem
{
    private EntityQuery _query;
    private ComputeBuffer _buffer;
    private int entityCount;

    protected override void OnCreateManager()
    {
        _buffer = new ComputeBuffer(3, 4 * sizeof(float));
        _query = GetEntityQuery(typeof(RenderMesh), typeof(RenderColor));
    }

    protected override void OnUpdate()
    {
        entityCount = _query.CalculateEntityCount();

        if (entityCount > 0)
        {
            NativeArray<Unity.Entities.Entity> entities = _query.ToEntityArray(Allocator.TempJob);
            NativeArray<RenderColor> colors = _query.ToComponentDataArray<RenderColor>(Allocator.TempJob);
            NativeArray<Color> colorBuffer = new NativeArray<Color>(entityCount, Allocator.TempJob);

            RenderMesh renderer = World.Active.EntityManager.GetSharedComponentData<RenderMesh>(entities[0]);

            for (int i = 0; i < entityCount; i++)
            {
                colorBuffer[i] = colors[i].Value;
            }

            _buffer.SetData(colorBuffer);
            renderer.material.SetBuffer("_myColorBuffer", _buffer);

            entities.Dispose();
            colors.Dispose();
            colorBuffer.Dispose();
        }
    }
}

}