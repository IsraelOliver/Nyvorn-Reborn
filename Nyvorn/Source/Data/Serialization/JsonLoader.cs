using System;
using System.IO;
using System.Text.Json;

namespace Nyvorn.Source.Data.Serialization
{
    public static class JsonLoader
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            IncludeFields = true
        };

        public static T LoadFromFile<T>(string relativePath) where T : new()
        {
            string fullPath = Path.Combine(AppContext.BaseDirectory, relativePath);
            if (!File.Exists(fullPath))
                return new T();

            string json = File.ReadAllText(fullPath);
            T data = JsonSerializer.Deserialize<T>(json, Options);
            return data ?? new T();
        }
    }
}
