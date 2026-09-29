﻿using SkillBox.Course.CharacterDashComponents;
using SkillBox.Course.CharacterMoveComponents;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace SkillBox.Course.PlayerComponentsSystems
{
    [UpdateBefore(typeof(PhysicsSimulationGroup))]
    public partial struct CharacterMoveToPhysicsVelocitySystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (dir, velocity) in SystemAPI.Query<CharacterMoveComponent, RefRW<PhysicsVelocity>>())
            {
                velocity.ValueRW.Linear = dir.Direction * dir.Speed;
            }
        }
    }

    [UpdateBefore(typeof(PhysicsSimulationGroup))]
    [UpdateAfter(typeof(CharacterMoveToPhysicsVelocitySystem))]
    public partial struct CharacterMoveToMoveDirectionSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (dir, transform) in SystemAPI.Query<CharacterMoveComponent, RefRW<LocalTransform>>())
            {
                if (math.lengthsq(dir.Direction) < 0.0001f)
                    continue;

                var nextRotation = quaternion.LookRotationSafe(math.normalize(dir.Direction), math.up());

                transform.ValueRW.Rotation = math.slerp(transform.ValueRO.Rotation, nextRotation, math.saturate(deltaTime * 10f));
            }
        }
    }
}
