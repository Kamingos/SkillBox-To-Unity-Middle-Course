﻿using SkillBox.Course.CharacterAnimatorComponents;
using SkillBox.Course.CharacterMoveComponents;
using SkillBox.Course.GameObjectScripts;
using SkillBox.Course.PlayerComponents;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace SkillBox.Course.PlayerSystems
{
    // Инициализация игрока
    public partial struct PlayerAuthoringSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var entityBuffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (flag, entity) in SystemAPI.Query<EnabledRefRW<IsPlayerNotAuthoredFlag>>().WithEntityAccess())
            {
                Debug.Log("Player trying");
                if (PlayerRefsSingleton.Instance == null)
                    continue;

                entityBuffer.AddComponent(entity, new RigidBodyRefComponent { RigidBodyRef = PlayerRefsSingleton.Instance.Rigidbody });
                entityBuffer.AddComponent(entity, new AnimatorRefComponent { AnimatorRef = PlayerRefsSingleton.Instance.Animator });

                Debug.Log("Player Authored");

                flag.ValueRW = false;

            }

            entityBuffer.Playback(state.EntityManager);
            entityBuffer.Dispose();
        }
    }

    // Позиция сущности → GameObject (после физики DOTS)
    [UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
    public partial struct PlayerEntityToHybridSyncSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (transform, rigidBodyRef) in SystemAPI.Query<RefRO<LocalTransform>, RigidBodyRefComponent>())
            {
                var rigidBody = rigidBodyRef.RigidBodyRef.Value;
                if (rigidBody == null)
                    continue;

                rigidBody.position = transform.ValueRO.Position;
                rigidBody.rotation = transform.ValueRO.Rotation;
            }
        }
    }
}
