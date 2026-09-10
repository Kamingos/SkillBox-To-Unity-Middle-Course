using SkillBox.Course.CharacterAnimatorComponents;
using SkillBox.Course.CharacterMoveComponents;
using SkillBox.Course.PlayerComponents;
using SkillBox.Course.PlayerInputComponents;
using Unity.Entities;
using UnityEngine;

namespace SkillBox.Course
{
    public class PlayerHybridAuthoring : MonoBehaviour
    {
        private class Baker : Baker<PlayerHybridAuthoring>
        {
            public override void Bake(PlayerHybridAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent<PlayerInputData>(entity);

                AddComponent<CharacterMoveComponent>(entity);

                // flag
                AddComponent<IsPlayerNotAuthoredFlag>(entity);
                SetComponentEnabled<IsPlayerNotAuthoredFlag>(entity, true);
            }
        }
    }
}
