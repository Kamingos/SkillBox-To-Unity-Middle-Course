using Unity.Entities;

namespace SkillBox.Course.HealthComponents
{
    // Здоровье сущности (например игрока)
    public struct HealthComponent : IComponentData
    {
        public float CurrentHealth;
        public float MaxHealth;
    }

    // Способность нанести урон той сущности, у которой есть HealthComponent
    public struct ApplyDamageComponent : IComponentData
    {
        public float Damage;
    }

    // Способность вылечить ту сущность, у которой есть HealthComponent
    public struct ApplyHealComponent : IComponentData
    {
        public float Heal;
    }
}
