using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace SkillBox.Course.CharacterAnimatorComponents
{
    public struct AnimatorRefComponent : IComponentData
    {
        public UnityObjectRef<Animator> AnimatorRef;
    }
    public struct AnimatorDataComponent : IComponentData
    {
        public CharacterAnimationType AnimationType;
    }

    public enum CharacterAnimationType
    {
        IDLE,
        WALK,
        RUN,

    }

}
