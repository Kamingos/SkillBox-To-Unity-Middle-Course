using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace SkillBox.Course.CharacterMoveComponents
{
    public struct CharacterMoveComponent : IComponentData
    {
        public float3 Direction;

        public float Speed;
    }

    public struct RigidBodyRefComponent : IComponentData
    {
        public UnityObjectRef<Rigidbody> RigidBodyRef;
    }

}
