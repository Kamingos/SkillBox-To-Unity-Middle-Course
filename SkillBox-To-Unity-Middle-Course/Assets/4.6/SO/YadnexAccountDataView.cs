using UnityEngine;

namespace SkillBox.Course
{
    [CreateAssetMenu(fileName = "YadnexAccountDataView", menuName = "Scriptable Objects/YadnexAccountDataView")]
    public class YadnexAccountDataView : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string serviceAccountId;
        [SerializeField] private string createdAt; // пример: "2026-10-01T03:51:48.756220235Z"
        [SerializeField] private string bucketName;
        [SerializeField] private string keyIdd;
        [SerializeField] private string secret;

        public string Id => id;
        public string ServiceAccountId => serviceAccountId;
        public string CreatedAt => createdAt;
        public string BucketName => bucketName;
        public string KeyId => keyIdd;
        public string Secret => secret;

    }
}
