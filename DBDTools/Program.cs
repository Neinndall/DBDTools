using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using Serilog;
using System.Text.RegularExpressions;

class Program
{
    static void Main(string[] args)
    {
        // Mostrar el arte ASCII de bienvenida
        UI.PrintWelcomeMessage();

        // Configura Serilog
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {Message}{NewLine}{Exception}")
            .CreateLogger();

        // Directorios
        string jsonDir = @"Files\JSON";
        string wemDir = @"Files\WEM";
        string outputDir = @"Output"; // Directorio en la raíz

        // Crear los directorios JSON y WEM si no existen
        Directory.CreateDirectory(jsonDir);
        Directory.CreateDirectory(wemDir);
        Log.Information("Directories: {jsonDir}, {wemDir} have been created.", jsonDir, wemDir);

        // Mensaje para el usuario
        Log.Information("Please place the WEM and JSON files in their corresponding directories.");
        Log.Information("Press ENTER when you are ready to begin...");

        // Esperar a que el usuario presione ENTER
        Console.ReadLine();

        // Diccionario de archivos
        Dictionary<string, string> fileDict = new Dictionary<string, string>();

        // Expresión regular para eliminar la parte después del último guion bajo
        var regex = new Regex(@"_(?!.*_).*");

        // Iterar sobre todos los archivos JSON en el directorio
        foreach (string jsonFilePath in Directory.GetFiles(jsonDir, "*.json"))
        {
            string jsonFileName = Path.GetFileName(jsonFilePath);
            try
            {
                // Cargar el archivo JSON
                string jsonContent = File.ReadAllText(jsonFilePath);
                var data = JObject.Parse(jsonContent);

                Log.Information("File: '{jsonFileName}' loaded.", jsonFileName);

                // Verificar si la clave 'SoundBanksInfo' existe en los datos cargados
                if (data["SoundBanksInfo"] != null)
                {
                    var soundbanksInfo = data["SoundBanksInfo"];

                    // Verificar si la clave 'SoundBanks' existe dentro de 'SoundBanksInfo'
                    if (soundbanksInfo["SoundBanks"] != null)
                    {
                        foreach (var soundbank in soundbanksInfo["SoundBanks"])
                        {
                            // Verificar si la clave 'Media' existe en cada SoundBank
                            if (soundbank["Media"] != null)
                            {
                                foreach (var media in soundbank["Media"])
                                {
                                    string fileId = media["Id"].ToString();
                                    string filePath = media["CachePath"].ToString();

                                    // Limpiar el CachePath usando regex
                                    string cleanedFilePath = regex.Replace(filePath, ".wem");

                                    fileDict[fileId] = cleanedFilePath;
                                }
                            }
                        }
                    }
                    else
                    {
                        Log.Warning("The key 'SoundBanks' is not found in 'SoundBanksInfo'.");
                    }
                }
                else
                {
                    Log.Warning("The key 'SoundBanksInfo' is not found in the JSON file.");
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error loading JSON file '{jsonFileName}': {Message}", jsonFileName, e.Message);
                continue;
            }
        }

        // Crear el directorio de salida si no existe
        Directory.CreateDirectory(outputDir);

        // Iterar sobre cada archivo WEM en la carpeta correspondiente
        foreach (string wemFilePath in Directory.GetFiles(wemDir, "*.wem"))
        {
            string wemFileName = Path.GetFileName(wemFilePath);
            string wemId = Path.GetFileNameWithoutExtension(wemFileName);

            if (fileDict.TryGetValue(wemId, out string filePath))
            {
                // Crear el directorio si no existe y mover/sobrescribir el archivo
                string folderPath = Path.Combine(outputDir, Path.GetDirectoryName(filePath));
                string fileName = Path.GetFileNameWithoutExtension(filePath);

                Directory.CreateDirectory(folderPath);
                string destination = Path.Combine(folderPath, fileName + ".wem");

                // Mover el archivo (sobrescribiendo si ya existe)
                File.Move(wemFilePath, destination, true);

                Log.Information("Moving: {wemFileName} to {destination}", wemFileName, destination);
            }
            else
            {
                // Mover el archivo a la carpeta de archivos desconocidos
                string unknownFolder = Path.Combine(outputDir, "Unknown Files");
                Directory.CreateDirectory(unknownFolder);
                string destination = Path.Combine(unknownFolder, wemFileName);

                // Mover el archivo (sobrescribiendo si ya existe)
                File.Move(wemFilePath, destination, true);
                Log.Information("File: {wemFileName} not found in JSON, moved to folder Unknown Files", wemFileName);
            }
        }

        // Mensaje de finalización
        Log.Information("Done. Press any key to close.");

        // Esperar a que el usuario presione una tecla antes de cerrar
        Console.ReadKey();

        // Cierra el logger
        Log.CloseAndFlush();
    }
}
