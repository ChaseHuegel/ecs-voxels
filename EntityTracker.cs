using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public interface IReceiveEntity
{
    void SetReceivedEntity(Unity.Entities.Entity entity);
}

public enum EntityTrackingMode
{
    SEND,
    RECIEVE
}

public class EntityTracker : MonoBehaviour, IReceiveEntity
{
    public EntityTrackingMode trackMode = EntityTrackingMode.RECIEVE;

    private Unity.Entities.Entity EntityToTrack = Unity.Entities.Entity.Null;
    public void SetReceivedEntity(Unity.Entities.Entity entity)
    {
        EntityToTrack = entity;
        GameMaster.Instance.entitiesToObjectMap.Add(entity, this.transform);
    }

    public Unity.Entities.Entity GetReceivedEntity()
    {
        return EntityToTrack;
    }

    public Vector3 getPosition()
    {
        EntityManager entityManager = GameMaster.Instance.entityManager;
        return entityManager.GetComponentData<Translation>(EntityToTrack).Value;
    }

    public Quaternion getRotation()
    {
        EntityManager entityManager = GameMaster.Instance.entityManager;
        return entityManager.GetComponentData<Rotation>(EntityToTrack).Value;
    }

    private void FixedUpdate()
    {
        if (EntityToTrack != Unity.Entities.Entity.Null)
        {
            try
            {
                EntityManager entityManager = GameMaster.Instance.entityManager;

                if (trackMode == EntityTrackingMode.RECIEVE)
                {
                    transform.position = entityManager.GetComponentData<Translation>(EntityToTrack).Value;
                    transform.rotation = entityManager.GetComponentData<Rotation>(EntityToTrack).Value;
                }
                else if (trackMode == EntityTrackingMode.SEND)
                {
                    entityManager.SetComponentData(EntityToTrack, new Translation { Value = transform.position });
		            entityManager.SetComponentData(EntityToTrack, new Rotation { Value = transform.rotation });
                }
            }
            catch
            {
                // Dirty way to check for an Entity that no longer exists.
                EntityToTrack = Unity.Entities.Entity.Null;
            }
        }
    }
}