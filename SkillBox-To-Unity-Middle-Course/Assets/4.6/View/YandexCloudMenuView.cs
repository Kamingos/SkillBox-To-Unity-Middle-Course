using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SkillBox.Course
{
    public class YandexCloudMenuView : MonoBehaviour
    {
        [SerializeField] private Button uploadDataBtn;
        [SerializeField] private TMP_InputField nameField;
        [SerializeField] private TMP_InputField ageField;
        [SerializeField] private TMP_InputField cityField;
        [SerializeField] private TMP_Text responseText;
        [SerializeField] private TMP_Text actualDataText;
        [SerializeField] private Button readDataBtn;

        [SerializeField] private List<TMP_InputField> extraFields = new List<TMP_InputField>();
        [SerializeField] private Transform profileListContent;
        [SerializeField] private GameObject profileRowPrefab;
        [SerializeField] private GameObject[] initialProfileRows;

        public event Action UploadRequested;
        public event Action DownloadRequested;
        private readonly List<GameObject> createdProfileRows = new List<GameObject>();

        private void Awake()
        {
            SetField(nameField, "Герой", "");
            SetField(ageField, "Класс героя", "");
            SetField(cityField, "Скорость героя", "");
            cityField.contentType = TMP_InputField.ContentType.DecimalNumber;

            SetField(extraFields[0], "Длительность рывка", "");
            SetField(extraFields[1], "Скорость рывка", "");
            SetField(extraFields[2], "Перезарядка рывка", "");
            SetField(extraFields[3], "Сила атаки", "");
            extraFields[0].contentType = TMP_InputField.ContentType.DecimalNumber;
            extraFields[1].contentType = TMP_InputField.ContentType.DecimalNumber;
            extraFields[2].contentType = TMP_InputField.ContentType.DecimalNumber;
            extraFields[3].contentType = TMP_InputField.ContentType.IntegerNumber;
            actualDataText.text = JsonUtility.ToJson(
                new YCObjectModel("Воин", "Воин", 5f, 0.2f, 10f, 1.5f, 25), true);
            SetResponse("Измените параметры героя и нажмите «Отправить».");

            for (int i = 0; i < initialProfileRows.Length; i++)
                initialProfileRows[i].SetActive(false);
        }

        private void OnEnable()
        {
            uploadDataBtn.onClick.AddListener(HandleUploadClicked);
            readDataBtn.onClick.AddListener(HandleDownloadClicked);
        }

        private void OnDisable()
        {
            uploadDataBtn.onClick.RemoveListener(HandleUploadClicked);
            readDataBtn.onClick.RemoveListener(HandleDownloadClicked);
        }

        public bool TryCreateModel(out YCObjectModel data, out string error)
        {
            float speed;
            float dashDuration;
            float dashSpeed;
            float dashReload;
            int attackPower;

            if (string.IsNullOrWhiteSpace(nameField.text) ||
                string.IsNullOrWhiteSpace(ageField.text) ||
                !TryFloat(cityField.text, out speed) ||
                !TryFloat(extraFields[0].text, out dashDuration) ||
                !TryFloat(extraFields[1].text, out dashSpeed) ||
                !TryFloat(extraFields[2].text, out dashReload) ||
                !int.TryParse(extraFields[3].text, NumberStyles.Integer, CultureInfo.InvariantCulture, out attackPower))
            {
                data = default(YCObjectModel);
                error = "Заполните поля модели героя. Числовые поля должны быть корректными числами.";
                return false;
            }

            data = new YCObjectModel(
                nameField.text.Trim(),
                ageField.text.Trim(),
                speed,
                dashDuration,
                dashSpeed,
                dashReload,
                attackPower);
            error = null;
            return true;
        }

        public void SetResponse(string message)
        {
            responseText.text = message;
        }

        public void ShowData(YCObjectModel data)
        {
            nameField.text = data.heroName;
            ageField.text = data.heroClass;
            cityField.text = data.heroSpeed.ToString(CultureInfo.InvariantCulture);
            extraFields[0].text = data.heroDashDuration.ToString(CultureInfo.InvariantCulture);
            extraFields[1].text = data.heroDashSpeed.ToString(CultureInfo.InvariantCulture);
            extraFields[2].text = data.heroDashReloadDuration.ToString(CultureInfo.InvariantCulture);
            extraFields[3].text = data.heroAttackPower.ToString(CultureInfo.InvariantCulture);
            actualDataText.text = JsonUtility.ToJson(data, true);
        }

        public void ShowProfiles(IList<string> profiles)
        {
            ClearProfileRows();
            for (int i = 0; i < profiles.Count; i++)
            {
                int separator = profiles[i].IndexOf('\n');
                if (separator >= 0)
                    AddProfile(profiles[i].Substring(0, separator), profiles[i].Substring(separator + 1));
            }
        }

        public void AddProfile(string key, string json)
        {
            GameObject row = Instantiate(profileRowPrefab, profileListContent);
            row.SetActive(true);
            row.name = key;
            TMP_Text text = row.GetComponentInChildren<TMP_Text>(true);
            text.text = key + "\n" + json;
            createdProfileRows.Add(row);
        }

        private void ClearProfileRows()
        {
            for (int i = 0; i < createdProfileRows.Count; i++)
            {
                createdProfileRows[i].SetActive(false);
                Destroy(createdProfileRows[i]);
            }
            createdProfileRows.Clear();
        }

        private static void SetField(TMP_InputField field, string label, string value)
        {
            field.placeholder.GetComponent<TMP_Text>().text = label;
            field.text = value;
        }

        private static bool TryFloat(string value, out float result)
        {
            return float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
        }

        private void HandleUploadClicked()
        {
            UploadRequested?.Invoke();
        }

        private void HandleDownloadClicked()
        {
            DownloadRequested?.Invoke();
        }
    }
}
