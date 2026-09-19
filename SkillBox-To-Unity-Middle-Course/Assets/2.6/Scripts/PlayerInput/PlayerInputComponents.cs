using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace SkillBox.Course.PlayerInputComponents
{
    public struct PlayerUnputRefComponent : IComponentData
    {
        public UnityObjectRef<FixedJoystick> Value;
    }

    public struct PlayerInputData : IComponentData
    {
        public float2 DirectionInput;
        public float Sprint;
    }
}
