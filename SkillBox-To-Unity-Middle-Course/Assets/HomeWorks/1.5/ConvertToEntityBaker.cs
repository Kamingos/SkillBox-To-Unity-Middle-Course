using Unity.Entities;
using UnityEngine;

namespace Skillbox.Cource.Work_1_5
{
    public class ConvertToEntityBaker : MonoBehaviour
    {
        private class Baker : Baker<ConvertToEntityBaker>
        {
            public override void Bake(ConvertToEntityBaker authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
            }
        }
    }
}
