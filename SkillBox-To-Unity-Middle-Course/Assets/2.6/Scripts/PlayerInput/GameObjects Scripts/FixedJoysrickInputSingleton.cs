using SkillBox.Course.GameObjectScripts;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace SkillBox.Course.PlayerInputJoystick
{
    internal class FixedJoysrickInputSingleton : MonoBehaviour
    {
        [SerializeField] private FixedJoystick joystick;
        public FixedJoystick JoystickInput => joystick;

        // мне лень делать второй класс, это будет своего рода View-шкой
        [SerializeField] private Button sprintBtn;
        public float SprintBtn => sprintBtn.IsPressed() ? 1 : 0;


        public static FixedJoysrickInputSingleton Instance;


        public void Awake()
        {
            if (Instance == null)
                Instance = this;

            else
                Destroy(gameObject);
        }

        public void OnValidate()
        {
            if (joystick == null)
                Debug.Log("[FixedJoysrickInput] joystick == null");

            if (sprintBtn == null)
                Debug.Log("[FixedJoysrickInput] sprintBtn == null");
        }
    }
}
