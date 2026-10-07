using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace SkillBox.Course
{
    public static class LocalJsonStorage
    {
        private const string SaveFolderName = "YandexCloud";

        public static string GetPath(string objectKey)
        {
            string[] segments = GetValidatedSegments(objectKey);
            string path = Path.Combine(Application.persistentDataPath, SaveFolderName);

            for (int i = 0; i < segments.Length; i++)
                path = Path.Combine(path, segments[i]);

            return path;
        }

        public static void Save(string objectKey, string json)
        {
            string path = GetPath(objectKey);
            string directory = Path.GetDirectoryName(path);

            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            File.WriteAllText(path, json, new UTF8Encoding(false));
        }

        public static string Load(string objectKey)
        {
            string path = GetPath(objectKey);

            if (!File.Exists(path))
                throw new FileNotFoundException("Локальная копия данных не найдена.", path);

            return File.ReadAllText(path, Encoding.UTF8);
        }

        public static List<string> LoadAll(string objectPrefix)
        {
            string directory = Path.Combine(Application.persistentDataPath, SaveFolderName);
            List<string> result = new List<string>();
            if (!Directory.Exists(directory))
                return result;

            string[] files = Directory.GetFiles(directory, objectPrefix + "*.json");
            for (int i = 0; i < files.Length; i++)
            {
                string key = Path.GetFileName(files[i]);
                string json = File.ReadAllText(files[i], Encoding.UTF8);
                result.Add(key + "\n" + json);
            }

            return result;
        }

        private static string[] GetValidatedSegments(string objectKey)
        {
            if (string.IsNullOrWhiteSpace(objectKey))
                throw new ArgumentException("Ключ объекта не задан.", nameof(objectKey));

            string[] segments = objectKey.Split('/');
            char[] invalidChars = Path.GetInvalidFileNameChars();

            for (int i = 0; i < segments.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(segments[i]) || segments[i] == "." || segments[i] == "..")
                    throw new ArgumentException("Ключ объекта содержит недопустимый путь.", nameof(objectKey));

                if (segments[i].IndexOfAny(invalidChars) >= 0)
                    throw new ArgumentException("Ключ объекта содержит недопустимые символы имени файла.", nameof(objectKey));
            }

            return segments;
        }
    }
}
