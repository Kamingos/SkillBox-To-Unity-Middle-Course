using SkillBox.Course.HealthComponents;
using SkillBox.Course.PlayerComponents;
using Unity.Entities;
using UnityEngine;

namespace SkillBox.Course
{
    public class PlayerHealthAuthoring : MonoBehaviour
    {
        [SerializeField] private float MaxHealth = 100;
        [SerializeField] private float CurrentHealth = 100;

        private class Baker : Baker<PlayerHealthAuthoring>
        {
            public override void Bake(PlayerHealthAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new HealthComponent
                {
                    MaxHealth = authoring.MaxHealth,
                    CurrentHealth = authoring.CurrentHealth
                });

                AddComponent<PlayerTag>(entity);
            }
        }
    }
}
