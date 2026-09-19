using System.Collections;
using UnityEngine;

namespace SkillBox.Course.GameObjectScripts
{
    public class PlayerRefsSingleton : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private Animator _animator;

        public Rigidbody Rigidbody => _rigidbody;
        public Animator Animator => _animator;

        public static PlayerRefsSingleton Instance;

        public void Awake()
        {
            if (Instance == null)
                Instance = this;

            else
                Destroy(gameObject);
        }

        public void OnValidate()
        {
            if (_rigidbody == null)
                Debug.Log("[PlayerRefsSingleton] _rigidbody == null");

            if (_animator == null)
                Debug.Log("[PlayerRefsSingleton] _animator == null");
        }
    }
}