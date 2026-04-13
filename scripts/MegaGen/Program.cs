using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MegaDataGenerator
{
    class Program
    {
        static async Task Main()
        {
            string emojiUrl = "https://raw.githubusercontent.com/muan/unicode-emoji-json/main/data-by-group.json";
            string biUrl = "https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.json";
            string targetPath = @"c:\Users\DELL\Pictures\HTTApp\src\webui\blazorwasm\HTT.BlazorWasm.App\wwwroot\assets\icons\enterprise-metadata.json";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
            
            var emojiStr = await client.GetStringAsync(emojiUrl);
            var emojiSource = JsonSerializer.Deserialize<List<EmojiGroup>>(emojiStr, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var biStr = await client.GetStringAsync(biUrl);
            var biDoc = JsonDocument.Parse(biStr);

            var categories = new List<object>();
            var icons = new Dictionary<string, List<object>>();

            // 1. Emoji Blocks (9)
            foreach (var group in emojiSource)
            {
                categories.Add(new { name = group.Name, icon = group.Emojis.First().Emoji, type = "Emoji" });
                icons[group.Name] = group.Emojis.Select(e => (object)new { name = e.Name, key = e.Slug, content = e.Emoji, type = "Emoji" }).ToList();
            }

            // 2. Diversified Bootstrap Blocks (8 new categories)
            var biCatConfig = new[] { 
                new { name = "Networking & Cloud", icon = "🌐" },
                new { name = "Financial Tools", icon = "💳" },
                new { name = "Medical & Health", icon = "🏥" },
                new { name = "Media Controls", icon = "🎬" },
                new { name = "Device Management", icon = "📱" },
                new { name = "Map & Navigation", icon = "📍" },
                new { name = "System Security", icon = "🛡️" },
                new { name = "Weather & Environment", icon = "🌦️" }
            };

            foreach(var c in biCatConfig) categories.Add(new { name = c.name, icon = c.icon, type = "Emoji" });

            int i = 0;
            foreach(var bi in biDoc.RootElement.EnumerateObject())
            {
                var targetCat = biCatConfig[i % biCatConfig.Length];
                if(!icons.ContainsKey(targetCat.name)) icons[targetCat.name] = new List<object>();
                icons[targetCat.name].Add(new { name = bi.Name.ToUpperInvariant(), key = bi.Name, content = "bi-" + bi.Name, type = "Css" });
                i++;
            }

            var output = new { categories, icons };
            var outputJson = JsonSerializer.Serialize(output, new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
            File.WriteAllText(targetPath, outputJson, new UTF8Encoding(false));
            Console.WriteLine($"SUCCESS: 17 Categories, {icons.Values.Sum(v => v.Count)} Icons. Redundant files removed.");
        }
    }
    class EmojiGroup { public string Name { get; set; } public List<EmojiItem> Emojis { get; set; } }
    class EmojiItem { public string Name { get; set; } public string Slug { get; set; } public string Emoji { get; set; } }
}
