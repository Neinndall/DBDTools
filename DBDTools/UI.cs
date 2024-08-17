using System;

public static class UI
{
    public static void PrintWelcomeMessage()
    {
        // Configuración de la consola
        Console.Title = Config.AppName;

        // Arte ASCII estilizado para "WEM AUDIO RENAMER"
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(@"
 _   _   ___   __ __      __    _  _   __    _    __      ___   ___   __  _    __    __ __   ___   ___  
| | | | | __| |  V  |    /  \  | || | | _\  | |  /__\    | _ \ | __| |  \| |  /  \  |  V  | | __| | _ \ 
| 'V' | | _|  | \_/ |   | /\ | | \/ | | v | | | | \/ |   | v / | _|  | | ' | | /\ | | \_/ | | _|  | v / 
!_/ \_! |___| |_| |_|   |_||_|  \__/  |__/  |_|  \__/    |_|_\ |___| |_|\__| |_||_| |_| |_| |___| |_|_\");
        
        // Imprimir la información adicional en una línea
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine();
        Console.Write("                                    by: ");

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"{Config.Company}");

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write(" | Version: ");

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"v.{Config.Version}");

        Console.WriteLine(); // Salto de línea después de la información

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"                        {Config.Description}");
        Console.WriteLine();
        
        Console.ResetColor();
    }
}
