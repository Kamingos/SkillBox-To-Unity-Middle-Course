using SkillBox.Course.CharacterDashComponents;
using SkillBox.Course.CharacterMoveComponents;
using SkillBox.Course.PlayerComponentsSystems;
using SkillBox.Course.PlayerInputComponents;
using SkillBox.Course.PlayerInputJoystick;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SkillBox.Course.PlayerInputSystems
{
    public partial class PlayerInputSystembase : SystemBase
    {
        private PlayerInput _input;
        private InputAction _move;
        private InputAction _sprint;

        protected override void OnCreate()
        {
            _input = new PlayerInput();
            _input.Player.Enable();

            _move = _input.Player.Move;
            _sprint = _input.Player.Sprint;
        }

        protected override void OnUpdate()
        {

            // тут два ввода для удобства (хотя и нарушает SRP), но вообще можно было бы отдельную систему создать

            foreach (var data in SystemAPI.Query<RefRW<PlayerInputData>>())
            {
                data.ValueRW.DirectionInput = _move.ReadValue<Vector2>();
                data.ValueRW.Sprint = _sprint.ReadValue<float>();
            }

            foreach (var data in SystemAPI.Query<RefRW<PlayerInputData>>())
            {
                data.ValueRW.DirectionInput += (float2)FixedJoysrickInputSingleton.Instance.JoystickInput.Direction;
                data.ValueRW.Sprint += FixedJoysrickInputSingleton.Instance.SprintBtn;

                data.ValueRW.Sprint = math.clamp(data.ValueRW.Sprint, 0, 1);

                if (math.length(data.ValueRW.DirectionInput) > 0.01f)
                    data.ValueRW.DirectionInput = math.normalize(data.ValueRW.DirectionInput);
            }
        }

        protected override void OnDestroy()
        {
            _input.Dispose();
        }
    }

    [UpdateBefore(typeof(PhysicsSimulationGroup))]
    public partial struct PlayerInputToMoveSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (move, input) in SystemAPI.Query<RefRW<CharacterMoveComponent>, RefRO<PlayerInputData>>())
            {
                move.ValueRW.Direction = new float3(input.ValueRO.DirectionInput.x, 0, input.ValueRO.DirectionInput.y);
            }
        }
    }


    [UpdateBefore(typeof(PhysicsSimulationGroup))]
    [UpdateAfter(typeof(PlayerInputToMoveSystem))]
    public partial struct PlayerInputToDashSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var dashTimerLookup = SystemAPI.GetComponentLookup<CharacterDashEnabledTimer>();
            var moveLookup = SystemAPI.GetComponentLookup<CharacterMoveComponent>();

            var elapsedTime = SystemAPI.Time.ElapsedTime;

            foreach (var (sprint, input, entity) in SystemAPI.Query<RefRW<CharacterDashComponent>, RefRO<PlayerInputData>>().WithDisabled<CharacterDashEnabledTimer>().WithEntityAccess())
            {
                if (input.ValueRO.Sprint <= 0f)
                    continue;

                dashTimerLookup.SetComponentEnabled(entity, true);
                dashTimerLookup.GetRefRW(entity).ValueRW.DashStartTime = elapsedTime;


                sprint.ValueRW.Direction = moveLookup[entity].Direction;
            }
        }
    }
}
