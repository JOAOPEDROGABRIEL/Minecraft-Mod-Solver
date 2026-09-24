using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Minecraft_Mod_Solver.ViewModels;

namespace Minecraft_Mod_Solver.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void ProcurarPasta_Click(object sender, RoutedEventArgs e)
    {
        // Obtém o controle da janela principal para chamar o sistema operacional
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        // Abre a janela padrão de seleção de pastas
        var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Selecione a pasta onde estão os mods",
            AllowMultiple = false
        });

        // Se o usuário selecionou uma pasta e clicou em "OK"
        if (folders.Count > 0)
        {
            // Passa o caminho selecionado para a ViewModel atualizar o campo de texto
            if (DataContext is MainViewModel vm)
            {
                vm.SelectedFolderPath = folders[0].Path.LocalPath;
            }
        }
    }
}