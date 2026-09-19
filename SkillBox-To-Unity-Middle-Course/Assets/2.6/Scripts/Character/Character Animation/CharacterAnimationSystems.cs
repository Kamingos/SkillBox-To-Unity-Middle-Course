using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace SkillBox.Course.CharacterAnimatorComponents
{
    public struct AnimatorRefComponent : IComponentData
    {
        public UnityObjectRef<Animator> AnimatorRef;
    }

}
