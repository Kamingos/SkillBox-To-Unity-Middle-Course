using SkillBox.Course.CharacterAnimatorComponents;
using SkillBox.Course.CharacterMoveComponents;
using SkillBox.Course.PlayerComponents;
using SkillBox.Course.PlayerInputComponents;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SkillBox.Course
{
    public class PlayerHybridAuthoring : MonoBehaviour
    {
        public float Speed = 1;
        public float SprintSpeed = 3;

        private class Baker : Baker<PlayerHybridAuthoring>
        {
            public override void Bake(PlayerHybridAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent<PlayerInputData>(entity);

                AddComponent(entity, new CharacterMoveComponent
                {
                    Speed = authoring.Speed
                });
                AddComponent(entity, new CharacterSprintComponent
                {
                    Speed = authoring.SprintSpeed
                });
                                
                // flag
                AddComponent<IsPlayerNotAuthoredFlag>(entity);
                SetComponentEnabled<IsPlayerNotAuthoredFlag>(entity, true);
            }
        }
    }
}
