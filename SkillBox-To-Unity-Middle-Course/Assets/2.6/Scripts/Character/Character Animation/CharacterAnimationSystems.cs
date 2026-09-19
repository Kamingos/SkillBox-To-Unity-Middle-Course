using SkillBox.Course.CharacterAnimatorComponents;
using SkillBox.Course.CharacterDashComponents;
using SkillBox.Course.CharacterMoveComponents;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using UnityEngine;

namespace SkillBox.Course.CharacterAnimatorSystems
{
    [UpdateAfter(typeof(PhysicsSimulationGroup))]
    public partial struct MoveComponentAnimationSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (animator, animData, move) in SystemAPI.Query<RefRW<AnimatorRefComponent>, RefRW<AnimatorDataComponent>, CharacterMoveComponent>().WithDisabled<CharacterDashEnabledTimer>())
            {
                if (math.length(move.Direction.xy) < 0.01f)
                {
                    if (animData.ValueRW.AnimationType == CharacterAnimationType.IDLE)
                        continue;

                    animData.ValueRW.AnimationType = CharacterAnimationType.IDLE;

                    animator.ValueRW.AnimatorRef.Value.CrossFade(CharacterAnimationType.IDLE.ToString(), 0.1f);
                    continue;
                }

                if (animData.ValueRW.AnimationType == CharacterAnimationType.WALK)
                    continue;

                animData.ValueRW.AnimationType = CharacterAnimationType.WALK;

                animator.ValueRW.AnimatorRef.Value.CrossFade(CharacterAnimationType.WALK.ToString(), 0.1f);
            }
        }
    }

    [UpdateAfter(typeof(PhysicsSimulationGroup))]
    [UpdateAfter(typeof(MoveComponentAnimationSystem))]
    public partial struct DashComponentAnimationSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (animator, animData) in SystemAPI.Query<RefRW<AnimatorRefComponent>, RefRW<AnimatorDataComponent>>().WithAll<CharacterDashEnabledTimer>())
            {
                if (animData.ValueRW.AnimationType == CharacterAnimationType.RUN)
                    continue;

                animData.ValueRW.AnimationType = CharacterAnimationType.RUN;

                animator.ValueRW.AnimatorRef.Value.CrossFade(CharacterAnimationType.RUN.ToString(), 0.1f);
            }
        }
    }
}
