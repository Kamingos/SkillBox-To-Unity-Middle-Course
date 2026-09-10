using SkillBox.Course.CharacterMoveComponents;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace SkillBox.Course.PlayerComponentsSystems
{
    public partial struct MoveToLocalTransformSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (dir, velocity) in SystemAPI.Query<CharacterMoveComponent, RefRW<RigidBodyRefComponent>>())
            {
                if (velocity.ValueRW.RigidBodyRef == null)
                    continue;

                velocity.ValueRW.RigidBodyRef.Value.linearVelocity = dir.Direction;
            }
        }
    }
}
