using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Minecraft_Mod_Solver.Models;

namespace Minecraft_Mod_Solver.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _selectedFolderPath = "Nenhuma pasta selecionada";

    [ObservableProperty]
    private string _curseForgeApiKey = string.Empty;

    [ObservableProperty]
    private bool _isProcessing = false;

    // Lista reativa que aparecerá na tela
    public ObservableCollection<ModFileModel> ModFiles { get; } = new();

    [RelayCommand]
    private async Task SelectFolderAsync()
    {
        // TODO: Implementaremos a abertura da janela do Windows para escolher a pasta no próximo passo
        SelectedFolderPath = "Caminho/Ficticio/Para/Testar/Interface";
        
        // Exemplo de como um mod aparecerá na lista
        ModFiles.Add(new ModFileModel { FileName = "jei-1.20.1.jar", Status = "Pendente" });
    }

    [RelayCommand]
    private async Task VerifyModsAsync()
    {
        IsProcessing = true;
        // TODO: Aqui chamaremos os Services (Modrinth/CurseForge)
        await Task.Delay(2000); // Simula um tempo de carregamento
        IsProcessing = false;
    }
}