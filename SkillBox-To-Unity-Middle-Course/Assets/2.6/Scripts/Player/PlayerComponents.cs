using Unity.Entities;
using UnityEngine;

namespace SkillBox.Course.PlayerComponents
{
    public struct PlayerTag : IComponentData { }
    public struct IsPlayerNotAuthoredFlag : IComponentData, IEnableableComponent { }
}
