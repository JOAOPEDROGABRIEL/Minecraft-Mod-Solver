using CommunityToolkit.Mvvm.ComponentModel;

namespace Minecraft_Mod_Solver.Models;

// A classe precisa ser 'partial' para o CommunityToolkit gerar o código por baixo dos panos
public partial class ModFileModel : ObservableObject
{
    [ObservableProperty]
    private string _fileName = string.Empty;

    [ObservableProperty]
    private string _filePath = string.Empty;

    [ObservableProperty]
    private string _status = "Aguardando...";

    [ObservableProperty]
    private string _apiSource = "-";
}