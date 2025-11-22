using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace GameJam
{
    [Tool]
    public static class Utils
    {
        public static string LoadFromFile(string path)
        {
            using var file = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Read);
            string content = file.GetAsText();
            return content;
        }

        public static bool IsValidImageExtension(string path)
        {
            string[] validExtensions = { ".png", ".jpg", ".jpeg", ".bmp", ".tiff" };
            string ext = Path.GetExtension(path).ToLower();
            foreach (var e in validExtensions)
            {
                if (ext == e)
                    return true;
            }
            return false;
        }

        public static T ParseJson<T>(string path)
        {
            var content = LoadFromFile(path);
            var parsed = JsonSerializer.Deserialize<T>(content);

            /*
			Trying to fix assembly unloading errors, code from:
				https://github.com/godotengine/godot/issues/78513
				https://github.com/dotnet/runtime/issues/65323
			*/
            var assembly = typeof(JsonSerializerOptions).Assembly;
            var updateHandlerType = assembly.GetType("System.Text.Json.JsonSerializerOptionsUpdateHandler");
            var clearCacheMethod = updateHandlerType?.GetMethod("ClearCache", BindingFlags.Static | BindingFlags.Public);
            clearCacheMethod?.Invoke(null, [null]);

            return parsed;
        }

        public static List<T> ParseJsonList<T>(string path)
        {
            var content = LoadFromFile(path);
            var parsed = JsonSerializer.Deserialize<List<T>>(content);

            /*
			Trying to fix assembly unloading errors, code from:
				https://github.com/godotengine/godot/issues/78513
				https://github.com/dotnet/runtime/issues/65323
			*/
            var assembly = typeof(JsonSerializerOptions).Assembly;
            var updateHandlerType = assembly.GetType("System.Text.Json.JsonSerializerOptionsUpdateHandler");
            var clearCacheMethod = updateHandlerType?.GetMethod("ClearCache", BindingFlags.Static | BindingFlags.Public);
            clearCacheMethod?.Invoke(null, [null]);

            return parsed;
        }
    }
}