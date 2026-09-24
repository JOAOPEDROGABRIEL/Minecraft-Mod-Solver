using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Minecraft_Mod_Solver.Models;
using Minecraft_Mod_Solver.Services;

namespace Minecraft_Mod_Solver.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _selectedFolderPath = string.Empty;

    [ObservableProperty]
    private string _curseForgeApiKey = string.Empty;

    [ObservableProperty]
    private bool _isProcessing = false;

    // Novas propriedades visuais
    [ObservableProperty]
    private string _currentStatusLabel = "Aguardando inicialização...";

    [ObservableProperty]
    private string _apiStatusLabel = "APIs: Não testadas";

    public ObservableCollection<ModFileModel> ModFiles { get; } = new();

    public MainViewModel()
    {
        // Carrega a chave do AppData assim que o app abre
        CurseForgeApiKey = ConfigService.Load().CurseForgeApiKey;
    }

    // Dispara automaticamente quando a pasta é selecionada
    partial void OnSelectedFolderPathChanged(string value)
    {
        if (Directory.Exists(value))
        {
            ModFiles.Clear();
            var files = Directory.GetFiles(value, "*.jar");
            foreach (var f in files)
            {
                ModFiles.Add(new ModFileModel { FileName = Path.GetFileName(f), FilePath = f, Status = "Aguardando", ApiSource = "-" });
            }
            CurrentStatusLabel = $"{ModFiles.Count} mods encontrados e prontos para verificação.";
        }
    }

    [RelayCommand]
    private void SaveConfig()
    {
        ConfigService.Save(CurseForgeApiKey);
        ApiStatusLabel = "Chave salva com sucesso!";
    }

    [RelayCommand]
    private async Task TestApiAsync()
    {
        ApiStatusLabel = "Testando conexões...";
        var service = new ModVerifierService(CurseForgeApiKey);
        
        bool cfOk = await service.TestCurseForgeConnectionAsync();
        
        if (cfOk) ApiStatusLabel = "APIs: Modrinth (OK) | CurseForge (OK)";
        else ApiStatusLabel = "APIs: Modrinth (OK) | CurseForge (Falha/Sem Chave)";
    }

    [RelayCommand]
    private async Task VerifyModsAsync()
    {
        if (string.IsNullOrWhiteSpace(SelectedFolderPath) || !Directory.Exists(SelectedFolderPath)) return; 

        IsProcessing = true;
        
        string verifierDir = Path.Combine(SelectedFolderPath, "modVerifier Files");
        string clientDir = Path.Combine(verifierDir, "client side");
        string serverDir = Path.Combine(verifierDir, "server side");
        string unverifiedDir = Path.Combine(verifierDir, "non verified");

        Directory.CreateDirectory(clientDir);
        Directory.CreateDirectory(serverDir);
        Directory.CreateDirectory(unverifiedDir);

        var verifierService = new ModVerifierService(CurseForgeApiKey);

        foreach (var mod in ModFiles)
        {
            // Atualiza a interface em tempo real mostrando qual mod está sendo processado
            CurrentStatusLabel = $"Verificando: {mod.FileName}...";
            mod.Status = "Buscando...";

            string? side = await verifierService.CheckModrinthAsync(mod.FilePath);
            string apiSource = "Modrinth";

            if (side == null)
            {
                side = await verifierService.CheckCurseForgeAsync(mod.FilePath);
                if (side != null) apiSource = "CurseForge";
            }

            string targetDir = unverifiedDir;

            if (side is "required" or "optional")
            {
                mod.Status = "Server-Side";
                mod.ApiSource = apiSource;
                targetDir = serverDir;
            }
            else if (side == "unsupported" || side == "client")
            {
                mod.Status = "Client-Only";
                mod.ApiSource = apiSource;
                targetDir = clientDir;
            }
            else
            {
                mod.Status = "Não Reconhecido";
                mod.ApiSource = "-";
                targetDir = unverifiedDir;
            }

            try
            {
                string destFile = Path.Combine(targetDir, mod.FileName);
                File.Copy(mod.FilePath, destFile, overwrite: true);
            }
            catch
            {
                mod.Status = "Erro de Cópia";
            }
        }

        CurrentStatusLabel = "Gerando arquivo de manifesto JSON...";
        
        // Exportação dos dados para JSON
        string jsonExport = JsonSerializer.Serialize(ModFiles, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(Path.Combine(verifierDir, "modslist.json"), jsonExport);

        CurrentStatusLabel = "Verificação e organização concluídas com sucesso!";
        IsProcessing = false;
    }
}