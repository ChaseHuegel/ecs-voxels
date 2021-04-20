using UnityEngine;
using Unity.Jobs;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

namespace Swordfish.ecs
{

public struct Billboard : IComponentData {}

public class BillboardSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach( (ref Rotation r, ref Billboard b) =>
        {
            r.Value = Camera.main.transform.rotation;
        });

        // Entities.WithAll<Rotation, Billboard>().ForEach( (Entity e, ref Rotation r) =>
        // {
        //     r.Value = Camera.main.transform.rotation;
        // });
    }
}

}