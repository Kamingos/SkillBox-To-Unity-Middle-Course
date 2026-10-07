using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SkillBox.Course
{
    public class YandexCloudMenuView : MonoBehaviour
    {
        #region input

        [SerializeField] private Button uploadDataBtn;
        [SerializeField] private TMP_InputField nameField;
        [SerializeField] private TMP_InputField ageField;
        [SerializeField] private TMP_InputField cityField;
        [SerializeField] private TMP_Text responseText;
        #endregion

        #region Actual

        [SerializeField] private TMP_Text actualDataText;
        #endregion

        #region readData

        [SerializeField] private Button readDataBtn;
        #endregion
    }
}
