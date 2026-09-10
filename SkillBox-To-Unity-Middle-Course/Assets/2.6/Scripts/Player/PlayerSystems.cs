using SkillBox.Course.CharacterAnimatorComponents;
using SkillBox.Course.CharacterMoveComponents;
using SkillBox.Course.GameObjectScripts;
using SkillBox.Course.PlayerComponents;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace SkillBox.Course.PlayerSystems
{
    // здесь происходит инициализация игрока
    public partial struct PlayerAuthoringSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var entityBuffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (flag, entity) in SystemAPI.Query<EnabledRefRW<IsPlayerNotAuthoredFlag>>().WithEntityAccess())
            {
                Debug.Log("Player trying");
                if (PlayerRefsSingleton.Instance == null)
                    continue;

                entityBuffer.AddComponent(entity, new RigidBodyRefComponent { RigidBodyRef = PlayerRefsSingleton.Instance.Rigidbody });
                entityBuffer.AddComponent(entity, new AnimatorRefComponent { AnimatorRef = PlayerRefsSingleton.Instance.Animator });

                Debug.Log("Player Authored");

                flag.ValueRW = false;

            }

            entityBuffer.Playback(state.EntityManager);
            entityBuffer.Dispose();
        }
    }
}
