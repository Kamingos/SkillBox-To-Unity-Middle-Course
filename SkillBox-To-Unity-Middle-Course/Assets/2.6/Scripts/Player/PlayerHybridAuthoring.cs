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
            }
        }
    }
}
