using System;
using System.Collections.Generic;
using System.Text;
using Unity.Entities;
using Unity.Mathematics;

namespace SkillBox.Course.PlayerInputComponents
{
    public struct PlayerInputData : IComponentData
    {
        public float2 DirectionInput;
        public float DashBtn;
    }
}
