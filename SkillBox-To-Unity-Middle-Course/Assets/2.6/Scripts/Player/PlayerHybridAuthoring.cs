using SkillBox.Course.CharacterAnimatorComponents;
using SkillBox.Course.CharacterDashComponents;
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
        [SerializeField] private float Speed = 1;

        [SerializeField] private float SprintSpeed = 3;
        [SerializeField] private float SprintDuration = 1;
        [SerializeField] private float SprintReloadDuration = 4;

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


                // Dash
                AddComponent(entity, new CharacterDashComponent
                {
                    DashSpeed = authoring.SprintSpeed
                });


                AddComponent(entity, new CharacterDashEnabledTimer
                {
                    SprintDuration = authoring.SprintDuration
                });

                SetComponentEnabled<CharacterDashEnabledTimer>(entity, false);


                AddComponent(entity, new CharacterDashReloadTimer
                {
                    SprintReloadDuration = authoring.SprintReloadDuration
                });

                SetComponentEnabled<CharacterDashReloadTimer>(entity, false);



                // Flag
                AddComponent<IsPlayerNotAuthoredFlag>(entity);
                SetComponentEnabled<IsPlayerNotAuthoredFlag>(entity, true);

                // Animation
                AddComponent<AnimatorDataComponent>(entity);
            }
        }
    }
}
