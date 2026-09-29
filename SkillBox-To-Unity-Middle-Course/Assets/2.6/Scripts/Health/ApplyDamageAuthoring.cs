using SkillBox.Course.HealthComponents;
using Unity.Entities;
using UnityEngine;

namespace SkillBox.Course
{
    public class ApplyDamageAuthoring : MonoBehaviour
    {
        [SerializeField] private float Damage = 10;

        private class Baker : Baker<ApplyDamageAuthoring>
        {
            public override void Bake(ApplyDamageAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new ApplyDamageComponent
                {
                    Damage = authoring.Damage
                });
            }
        }
    }
}
