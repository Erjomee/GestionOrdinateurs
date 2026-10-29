
namespace GestionOrdinateurs.API;

public class JournalService
{
    private readonly string _dossierLogs;

    public JournalService(IWebHostEnvironment environment)
    {
        _dossierLogs = Path.Combine(
            environment.ContentRootPath,
            "Logs"
        );
    }

    public void Ecrire(string message, string cheminRelatif)
    {
        string chemin = Path.Combine(_dossierLogs, cheminRelatif);

        Directory.CreateDirectory(Path.GetDirectoryName(chemin)!);

        string ligne = $"{DateTime.Now:dd/MM/yyyy} - {message}";

        using var writer = new StreamWriter(chemin, append: true);
        writer.WriteLine(ligne);
    }
}
