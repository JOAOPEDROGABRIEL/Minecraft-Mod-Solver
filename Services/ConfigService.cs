using System;
using System.IO;
using System.Text.Json;

namespace Minecraft_Mod_Solver.Services;

public class ConfigService
{
    // Caminho: C:\Users\Usuario\AppData\Roaming\MinecraftModSolver\config.json
    private static readonly string ConfigPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
        "MinecraftModSolver", 
        "config.json"
    );

    public class AppConfig 
    { 
        public string CurseForgeApiKey { get; set; } = string.Empty; 
    }

    public static AppConfig Load()
    {
        if (!File.Exists(ConfigPath)) return new AppConfig();
        
        try
        {
            string json = File.ReadAllText(ConfigPath);
            return JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
        }
        catch { return new AppConfig(); }
    }

    public static void Save(string apiKey)
    {
        var fileInfo = new FileInfo(ConfigPath);
        fileInfo.Directory?.Create(); // Cria a pasta MinecraftModSolver se não existir
        
        string json = JsonSerializer.Serialize(new AppConfig { CurseForgeApiKey = apiKey });
        File.WriteAllText(ConfigPath, json);
    }
}