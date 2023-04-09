using System;
using System.Text.Json;
using System.IO;

namespace Airborn.web.Models
{
    public static class BookNumbersJsonHelper
    {
        public static JsonFile LoadJson(this BookDistances bookNumbers, string path)
        {
            JsonFile jsonFile = JsonSerializer.Deserialize<JsonFile>(File.ReadAllText(path));

            return jsonFile;
        }
    }
}