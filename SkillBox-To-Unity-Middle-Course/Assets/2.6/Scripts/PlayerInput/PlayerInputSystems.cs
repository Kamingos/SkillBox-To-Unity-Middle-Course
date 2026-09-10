using SkillBox.Course.CharacterMoveComponents;
using SkillBox.Course.PlayerInputComponents;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

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
            Debug.Log($"{_move.ReadValue<Vector2>().ToString()} | {_sprint.ReadValue<float>().ToString()}");

            foreach (var data in SystemAPI.Query<RefRW<PlayerInputData>>())
            {
                data.ValueRW.DirectionInput = _move.ReadValue<Vector2>();
                data.ValueRW.Sprint = _sprint.ReadValue<float>();
            }
        }

        protected override void OnDestroy()
        {
            _input.Dispose();
        }
    }

    public partial struct PlayerMoveSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (move, input) in SystemAPI.Query<RefRW<CharacterMoveComponent>, RefRO<PlayerInputData>>())
            {
                move.ValueRW.Direction = new float3(input.ValueRO.DirectionInput.x, 0, input.ValueRO.DirectionInput.y);
            }

            foreach (var (sprint, input) in SystemAPI.Query<RefRW<CharacterSprintComponent>, RefRO<PlayerInputData>>())
            {
                sprint.ValueRW.Value = input.ValueRO.Sprint;
            }
        }
    }
}
