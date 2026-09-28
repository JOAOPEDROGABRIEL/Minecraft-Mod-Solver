// Copyright (C) 2026 João Pedro Gabriel
// 
// Este programa é um software livre; você pode redistribuí-lo e/ou 
// modificá-lo sob os termos da Licença Pública Geral GNU como 
// publicada pela Free Software Foundation; na versão 3 da Licença.


using Avalonia;
using System;
using Avalonia.Diagnostics;

namespace Minecraft_Mod_Solver;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
#if DEBUG
            .LogToTrace()
#endif
            .WithInterFont();
}
