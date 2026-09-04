using Unity.Entities;
using UnityEngine;

namespace SkillBox.Course.PlayerComponents
{
    public struct PlayerData : IComponentData
    {
        UnityObjectRef<Rigidbody> PlayerRBRef;
        UnityObjectRef<Animator> PlayerAnimatorRef;
    }
}
