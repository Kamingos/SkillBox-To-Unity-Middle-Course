using SkillBox.Course.HealthComponents;
using Unity.Entities;
using UnityEngine;

namespace SkillBox.Course
{
    public class ApplyHealAuthoring : MonoBehaviour
    {
        [SerializeField] private float Heal = 10;

        private class Baker : Baker<ApplyHealAuthoring>
        {
            public override void Bake(ApplyHealAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new ApplyHealComponent
                {
                    Heal = authoring.Heal
                });
            }
        }
    }
}
