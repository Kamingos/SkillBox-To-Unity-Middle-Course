using SkillBox.Course.CharacterDashComponents;
using SkillBox.Course.CharacterMoveComponents;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

namespace SkillBox.Course.PlayerComponentsSystems
{
    public partial struct MoveToLocalTransformSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (dir, rigidBody) in SystemAPI.Query<CharacterMoveComponent, RefRW<RigidBodyRefComponent>>())
            {
                rigidBody.ValueRW.RigidBodyRef.Value.linearVelocity = dir.Direction * dir.Speed;
            }
        }
    }

    public partial struct CharacterMoveToMoveDirectionSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (dir, rigidBody) in SystemAPI.Query<CharacterMoveComponent, RefRW<RigidBodyRefComponent>>())
            {
                var nextRotation = Quaternion.LookRotation(new Vector3(dir.Direction.x, 0f, dir.Direction.z));

                var quatSlerp = Quaternion.RotateTowards(rigidBody.ValueRO.RigidBodyRef.Value.rotation, nextRotation, deltaTime * 540f);

                rigidBody.ValueRW.RigidBodyRef.Value.MoveRotation(quatSlerp);
            }
        }
    }
}
