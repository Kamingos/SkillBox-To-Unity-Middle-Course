using SkillBox.Course.HealthComponents;
using SkillBox.Course.PlayerComponents;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

namespace SkillBox.Course.HealthSystems
{
    [UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
    public partial struct ApplyDamageSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SimulationSingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var healthLookup = SystemAPI.GetComponentLookup<HealthComponent>();
            var damageLookup = SystemAPI.GetComponentLookup<ApplyDamageComponent>(true);

            state.Dependency = new ApplyDamageTriggerJob
            {
                HealthLookup = healthLookup,
                DamageLookup = damageLookup
            }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
        }

        [BurstCompile]
        private struct ApplyDamageTriggerJob : ITriggerEventsJob
        {
            public ComponentLookup<HealthComponent> HealthLookup;

            [ReadOnly] public ComponentLookup<ApplyDamageComponent> DamageLookup;

            public void Execute(TriggerEvent triggerEvent)
            {
                var entityA = triggerEvent.EntityA;
                var entityB = triggerEvent.EntityB;

                if (HealthLookup.HasComponent(entityA) && DamageLookup.HasComponent(entityB))
                {
                    Apply(entityA, entityB);
                }
                else if (HealthLookup.HasComponent(entityB) && DamageLookup.HasComponent(entityA))
                {
                    Apply(entityB, entityA);
                }
            }

            private void Apply(Entity healthEntity, Entity damageEntity)
            {
                var health = HealthLookup[healthEntity];

                health.CurrentHealth = math.max(0f, health.CurrentHealth - DamageLookup[damageEntity].Damage);

                HealthLookup[healthEntity] = health;
            }
        }
    }

    [UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
    public partial struct ApplyHealSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SimulationSingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var healthLookup = SystemAPI.GetComponentLookup<HealthComponent>();
            var healLookup = SystemAPI.GetComponentLookup<ApplyHealComponent>(true);

            state.Dependency = new ApplyHealTriggerJob
            {
                HealthLookup = healthLookup,
                HealLookup = healLookup
            }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
        }

        [BurstCompile]
        private struct ApplyHealTriggerJob : ITriggerEventsJob
        {
            public ComponentLookup<HealthComponent> HealthLookup;

            [ReadOnly] public ComponentLookup<ApplyHealComponent> HealLookup;

            public void Execute(TriggerEvent triggerEvent)
            {
                var entityA = triggerEvent.EntityA;
                var entityB = triggerEvent.EntityB;

                if (HealthLookup.HasComponent(entityA) && HealLookup.HasComponent(entityB))
                {
                    Apply(entityA, entityB);
                }
                else if (HealthLookup.HasComponent(entityB) && HealLookup.HasComponent(entityA))
                {
                    Apply(entityB, entityA);
                }
            }

            private void Apply(Entity healthEntity, Entity healEntity)
            {
                var health = HealthLookup[healthEntity];

                health.CurrentHealth = math.min(health.MaxHealth, health.CurrentHealth + HealLookup[healEntity].Heal);

                HealthLookup[healthEntity] = health;
            }
        }
    }

    public partial struct PlayerHealthDebugSystem : ISystem
    {
        private float _lastHealth;
        private bool _hasLastHealth;

        public void OnUpdate(ref SystemState state)
        {
            foreach (var health in SystemAPI.Query<RefRO<HealthComponent>>().WithAll<PlayerTag>())
            {
                var currentHealth = health.ValueRO.CurrentHealth;

                if (_hasLastHealth && math.abs(_lastHealth - currentHealth) < 0.0001f)
                    continue;

                _hasLastHealth = true;
                _lastHealth = currentHealth;

                Debug.Log($"[PlayerHealth] HP: {currentHealth}/{health.ValueRO.MaxHealth}");
            }
        }
    }
}
