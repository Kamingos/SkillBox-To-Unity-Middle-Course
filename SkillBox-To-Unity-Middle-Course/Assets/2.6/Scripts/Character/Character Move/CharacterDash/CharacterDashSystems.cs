using SkillBox.Course.CharacterMoveComponents;
using SkillBox.Course.PlayerInputSystems;
using Unity.Entities;
using UnityEngine;

namespace SkillBox.Course.CharacterDashComponents
{
    [UpdateAfter(typeof(PlayerInputToDashSystem))]
    public partial struct CharacterDashSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (dir, dash) in SystemAPI.Query<RefRW<CharacterMoveComponent>, CharacterDashComponent>().WithAll<CharacterDashEnabledTimer>())
            {
                dir.ValueRW.Direction = dash.Direction * dash.DashSpeed;
            }
        }
    }
    public partial struct CharacterDashTimerUpdateSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var elapsedTime = SystemAPI.Time.ElapsedTime;

            foreach (var (sprint, sprintEnabled) in SystemAPI.Query<RefRO<CharacterDashEnabledTimer>, EnabledRefRW<CharacterDashEnabledTimer>>())
            {
                if (elapsedTime < sprint.ValueRO.DashStartTime + sprint.ValueRO.SprintReloadDuration)
                    continue;
                
                sprintEnabled.ValueRW = false;
                
            }
        }
    }

}
