using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace SkillBox.Course
{
    public class YandexCloudMenuPresenter : MonoBehaviour
    {
        [SerializeField] private YandexCloudMenuView view;
        [SerializeField] private YadnexAccountDataView accountData;
        [SerializeField] private string objectKeyPrefix = "hero_";
        [SerializeField] private string objectListPrefix = string.Empty;

        private readonly YandexObjectStorageClient storage = new YandexObjectStorageClient();
        private bool busy;

        private void OnEnable()
        {
            if (view == null)
            {
                Debug.LogError("YandexCloudMenuPresenter: не назначен View.", this);
                return;
            }

            view.UploadRequested += Upload;
            view.DownloadRequested += Download;
        }

        private void OnDisable()
        {
            if (view == null)
                return;

            view.UploadRequested -= Upload;
            view.DownloadRequested -= Download;
        }

        private void Upload()
        {
            if (busy)
                return;

            YCObjectModel data;
            string error;
            if (!view.TryCreateModel(out data, out error))
            {
                view.SetResponse(error);
                return;
            }

            if (CheckSettings())
                StartCoroutine(UploadJson(data, CreateObjectKey(data.heroName)));
        }

        private void Download()
        {
            if (!busy && CheckSettings())
                StartCoroutine(DownloadProfiles());
        }

        private IEnumerator UploadJson(YCObjectModel data, string objectKey)
        {
            busy = true;
            string json = JsonUtility.ToJson(data, true);
            string localError = SaveLocal(objectKey, json);
            view.SetResponse("Отправка JSON в Object Storage...");

            yield return storage.Upload(
                accountData.BucketName, objectKey, accountData.KeyId, accountData.Secret,
                Encoding.UTF8.GetBytes(json),
                message =>
                {
                    view.AddProfile(objectKey, json);
                    view.SetResponse(message + " Файл: " + objectKey + "." + localError);
                },
                message => view.SetResponse(message + localError));

            busy = false;
        }

        private IEnumerator DownloadProfiles()
        {
            busy = true;
            string[] keys = null;
            string listError = null;

            view.SetResponse("Получение списка героев из Object Storage...");
            yield return storage.ListObjects(
                accountData.BucketName, objectListPrefix, accountData.KeyId, accountData.Secret,
                result => keys = result,
                error => listError = error);

            if (keys == null)
            {
                try
                {
                    List<string> localProfiles = LocalJsonStorage.LoadAll(objectListPrefix);
                    view.ShowProfiles(localProfiles);
                    view.SetResponse(listError + " Показаны локальные данные: " + localProfiles.Count + ".");
                }
                catch (IOException exception)
                {
                    view.SetResponse(listError + " Не удалось прочитать локальные данные: " + exception.Message);
                }
                catch (System.UnauthorizedAccessException exception)
                {
                    view.SetResponse(listError + " Нет доступа к локальным данным: " + exception.Message);
                }

                busy = false;
                yield break;
            }

            List<string> profiles = new List<string>();
            int failedDownloads = 0;
            for (int i = 0; i < keys.Length; i++)
            {
                if (!keys[i].EndsWith(".json", System.StringComparison.OrdinalIgnoreCase))
                    continue;

                byte[] bytes = null;
                string downloadError = null;
                yield return storage.Download(
                    accountData.BucketName, keys[i], accountData.KeyId, accountData.Secret,
                    result => bytes = result,
                    error => downloadError = error);

                if (bytes == null)
                {
                    failedDownloads++;
                    Debug.LogWarning("Не удалось скачать " + keys[i] + ": " + downloadError, this);
                    continue;
                }

                string json = Encoding.UTF8.GetString(bytes).TrimStart('\uFEFF');
                profiles.Add(keys[i] + "\n" + json);
                string saveError = SaveLocal(keys[i], json);
                if (!string.IsNullOrEmpty(saveError))
                    Debug.LogWarning("Локальная копия " + keys[i] + " не сохранена: " + saveError, this);
            }

            view.ShowProfiles(profiles);
            view.SetResponse("В списке " + profiles.Count + " JSON-файлов. Не скачано: " + failedDownloads + ".");
            busy = false;
        }

        private string SaveLocal(string objectKey, string json)
        {
            try
            {
                LocalJsonStorage.Save(objectKey, json);
                return " Офлайн-копия сохранена.";
            }
            catch (IOException exception)
            {
                return " Не удалось сохранить офлайн-копию: " + exception.Message;
            }
            catch (System.UnauthorizedAccessException exception)
            {
                return " Нет доступа к офлайн-копии: " + exception.Message;
            }
        }

        private string CreateObjectKey(string heroName)
        {
            StringBuilder safeName = new StringBuilder();
            for (int i = 0; i < heroName.Length; i++)
            {
                char character = heroName[i];
                if (char.IsLetterOrDigit(character) || character == '-' || character == '_')
                    safeName.Append(character);
                else if (safeName.Length > 0 && safeName[safeName.Length - 1] != '_')
                    safeName.Append('_');
            }

            string name = safeName.ToString().Trim('_');
            if (string.IsNullOrEmpty(name))
                name = System.Guid.NewGuid().ToString("N");

            return objectKeyPrefix + name + "_" + System.Guid.NewGuid().ToString("N") + ".json";
        }

        private bool CheckSettings()
        {
            if (accountData != null &&
                !string.IsNullOrWhiteSpace(accountData.BucketName) &&
                !string.IsNullOrWhiteSpace(accountData.KeyId) &&
                !string.IsNullOrWhiteSpace(accountData.Secret) &&
                !string.IsNullOrWhiteSpace(objectKeyPrefix))
                return true;

            view.SetResponse("Заполните бакет и ключи доступа в YadnexAccountDataView SO.");
            return false;
        }
    }
}
