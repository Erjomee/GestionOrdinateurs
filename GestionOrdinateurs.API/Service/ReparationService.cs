using GestionOrdinateurs.Shared;

using Microsoft.EntityFrameworkCore;

namespace GestionOrdinateurs.API;


public class ReparationService
{
    private readonly ParcInformatiqueContext _context;
    private readonly JournalService _journal;
    private static readonly object _lock = new object();


    public ReparationService(ParcInformatiqueContext context,JournalService journal)
    {
        _context = context;
        _journal = journal;
    }

    public async Task SimulerReparationAsync(
    Ordinateur ordinateur)
    {
        // Création object reparation 
        var reparation = new Reparation
        {
            UnOrdinateur = ordinateur,
            TypeReparation = "Maintenance",
            DateDebut = DateTime.Now,
            Statut = "En cours",
            Technicien = "Jerome",
        };
        lock (_lock)
            {
                _context.Reparations.Add(reparation);
                _context.SaveChanges();
            }

            await Task.Delay(2000);

        lock (_lock)
        {
            reparation.Statut = "Terminée";
            _context.SaveChanges();
        }

        _journal.Ecrire(
            $"{ordinateur.Nom} réparé",
            "Reparations/reparations.log");

        Console.WriteLine($"{ordinateur.Nom} réparé");
    }

    public async Task<List<Reparation>> GetAll()
    {
        return await _context.Reparations
            .Include(r => r.UnOrdinateur)
            .ToListAsync();
    }

    public async Task<List<HistoriqueReparationRecord>> GetHistoriqueReparations()
    {
        return await _context.Reparations
            .Include(reparation => reparation.UnOrdinateur)
            .Where(reparation => reparation.UnOrdinateur != null)
            .Select(reparation => new HistoriqueReparationRecord(
                reparation.UnOrdinateur!.Id,
                reparation.UnOrdinateur.Nom,
                reparation.Technicien,
                reparation.TypeReparation,
                reparation.DateDebut))
            .ToListAsync();
    }
}