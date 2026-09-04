using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace SkillBox.Course.CharacterMoveComponents
{
    public struct CharacterMoveComponent : IComponentData
    {
        public float2 Direction;
    }
}
