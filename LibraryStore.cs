using System.IO;
using System.Text.Json;
using MathAnimator.Model;

namespace MathAnimator
{
    public static class LibraryStore
    {
        private const string FileName = "library.json";

        public static LibraryData Load()
        {
            if (!File.Exists(FileName))
            {
                return new LibraryData
                {
                    Folders =
                    {
                        new LibraryFolder { Name = "Standard" }
                    }
                };
            }

            try
            {
                return JsonSerializer.Deserialize<LibraryData>(
                    File.ReadAllText(FileName)
                )!;
            }
            catch
            {
                // Falls Datei kaputt ist
                return new LibraryData
                {
                    Folders =
                    {
                        new LibraryFolder { Name = "Standard" }
                    }
                };
            }
        }

        public static void Save(LibraryData data)
        {
            File.WriteAllText(
                FileName,
                JsonSerializer.Serialize(
                    data,
                    new JsonSerializerOptions { WriteIndented = true }
                )
            );
        }
    }
}