using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
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

        // https://stackoverflow.com/questions/273313/randomize-a-listt
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Shared.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }
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

        public static bool IsValidAudioExtension(string path)
        {
            string[] validExtensions = { ".mp3", ".ogg", ".wav" };
            string ext = Path.GetExtension(path).ToLower();
            foreach (var e in validExtensions)
            {
                if (ext == e)
                    return true;
            }
            return false;
        }

        public static bool ValidateJson<T>(string path, out List<T> result)
        {
            result = default;
            string content;

            if (!Godot.FileAccess.FileExists(path))
            {
                GD.PushError($"{path} not found!");
                return false;
            }

            try
            {
                content = LoadFromFile(path);
            }
            catch (Exception e)
            {
                GD.PushError($"Failed to read file: {path}\n{e.Message}");
                return false;
            }

            try
            {
                result = JsonSerializer.Deserialize<List<T>>(content);

                var assembly = typeof(JsonSerializerOptions).Assembly;
                var updateHandlerType = assembly.GetType("System.Text.Json.JsonSerializerOptionsUpdateHandler");
                var clearCacheMethod = updateHandlerType?.GetMethod("ClearCache", BindingFlags.Static | BindingFlags.Public);
                clearCacheMethod?.Invoke(null, [null]);

                return result != null;
            }
            catch (JsonException e)
            {
                GD.PushError($"JSON format error: {path}\n{e.Message}");
                return false;
            }
            catch (Exception e)
            {
                GD.PushError($"Unexpected error: {path}\n{e.Message}");
                return false;
            }
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

        public static double WeightDecay(int count, double baseWeight = 1f, double decayRate = 1f)
        {
            return baseWeight / (1 + count * decayRate);
        }
    }
}