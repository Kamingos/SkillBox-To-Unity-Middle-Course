using Unity.Entities;
using Unity.Mathematics;

namespace SkillBox.Course.CharacterDashComponents
{
    public struct CharacterDashComponent : IComponentData
    {
        public float3 Direction;

        public float DashSpeed;
    }
    public struct CharacterDashEnabledTimer : IComponentData, IEnableableComponent
    {
        public double DashStartTime;
        public double SprintReloadDuration;
    }

}
