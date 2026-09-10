using SkillBox.Course.CharacterMoveComponents;
using SkillBox.Course.PlayerInputComponents;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Entities;

namespace SkillBox.Course.PlayerInputSystems
{
    public partial struct PlayerMoveSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (move, input) in
                     SystemAPI.Query<RefRW<CharacterMoveComponent>, RefRO<PlayerInputData>>())
            {
                move.ValueRW.Direction = new Unity.Mathematics.float3(input.ValueRO.DirectionInput.x, 0, input.ValueRO.DirectionInput.y);
            }
        }
    }
}
