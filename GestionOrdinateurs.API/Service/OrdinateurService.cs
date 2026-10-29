using GestionOrdinateurs.Shared;
using Microsoft.EntityFrameworkCore;

namespace GestionOrdinateurs.API;

public class OrdinateurService : IOrdinateurService
{
    // Contexte permettant d'accéder à la base de données
    private readonly ParcInformatiqueContext _context;

    // Événements déclenchés après les opérations
    public event EventHandler<OrdinateurAjouteEventArgs>? OrdinateurAjoute;
    public event EventHandler<OrdinateurSupprimeEventArgs>? OrdinateurSupprime;
    public event EventHandler<OrdinateurMisAJourEventArgs>? OrdinateurMisAJour;

    // Constructeur avec injection du contexte
    public OrdinateurService(ParcInformatiqueContext context)
    {
        _context = context;
    }

    // Récupère tous les ordinateurs
    public async Task<List<Ordinateur>> GetAll()
    {
        return await _context.Ordinateurs.ToListAsync();
    }

    // Recherche un ordinateur par son ID avec LINQ
    public async Task<Ordinateur?> GetById(int id)
    {
        return await _context.Ordinateurs
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    // Ajoute un ordinateur dans la base
    public async Task Ajouter(Ordinateur ordinateur)
    {
        await _context.Ordinateurs.AddAsync(ordinateur);
        await _context.SaveChangesAsync();

        // Déclenche l'événement après l'ajout
        OrdinateurAjoute?.Invoke(this, new OrdinateurAjouteEventArgs
        {
            Nom = ordinateur.Nom
        });
    }

    // Modifie un ordinateur existant
    public async Task Modifier(int id, Ordinateur ordinateur)
    {
        var ancien = await GetById(id);

        // Vérifie que l'ordinateur existe
        if (ancien != null)
        {
            // Met à jour les informations générales
            ancien.Nom = ordinateur.Nom;
            ancien.Stock = ordinateur.Stock;
            ancien.Prix = ordinateur.Prix;
            ancien.Etat = ordinateur.Etat;

            // Met à jour les caractéristiques techniques
            ancien.Caracteristiques.RamGo = ordinateur.Caracteristiques.RamGo;
            ancien.Caracteristiques.TypeOrdi = ordinateur.Caracteristiques.TypeOrdi;
            ancien.Caracteristiques.StockageGo = ordinateur.Caracteristiques.StockageGo;

            // Enregistre les modifications dans la base
            await _context.SaveChangesAsync();

            // Déclenche l'événement après la modification
            OrdinateurMisAJour?.Invoke(this, new OrdinateurMisAJourEventArgs
            {
                Nom = ancien.Nom
            });
        }
    }

    // Supprime un ordinateur existant
    public async Task Supprimer(int id)
    {
        var ordinateur = await GetById(id);

        if (ordinateur != null)
        {
            _context.Ordinateurs.Remove(ordinateur);
            await _context.SaveChangesAsync();

            // Déclenche l'événement après la suppression
            OrdinateurSupprime?.Invoke(this, new OrdinateurSupprimeEventArgs
            {
                Nom = ordinateur.Nom
            });
        }
    }

    // Récupère les ordinateurs disponibles en stock
    public async Task<List<Ordinateur>> GetOrdinateursDisponibles()
    {
        return await _context.Ordinateurs
            .Where(o => o.Stock > 0)
            .ToListAsync();
    }

    // Recherche les ordinateurs selon leur type
    public async Task<List<Ordinateur>> GetByType(TypeOrdinateur type)
    {
        return await _context.Ordinateurs
            .Where(o => o.Caracteristiques.TypeOrdi == type)
            .ToListAsync();
    }

    // Récupère les ordinateurs du plus cher au moins cher
    public async Task<List<Ordinateur>> GetOrdinateursParPrixDecroissant()
    {
        return await _context.Ordinateurs
            .OrderByDescending(o => o.Prix)
            .ToListAsync();
    }

    
    public string Diagnostiquer(Ordinateur ordinateur)
    {
        return ordinateur switch
        {
            { Etat: EtatOrdinateur.HorsService } => "Remplacement",

            {
                Etat: EtatOrdinateur.Lent,
                Caracteristiques: { RamGo: < 8 }
            } => "Ajouter RAM",

            _ => "Aucune action"
        };
    }

    public async Task<(int TotalOrdinateur, int OrdiDisponibles, decimal ValeurStock, int NbReparations)> CalculerStatistiques()
    {
        // Nombre total d'ordinateurs enregistrés
        var total = await _context.Ordinateurs.CountAsync();

        var valeurStock = await _context.Ordinateurs
            .SumAsync(o => o.Stock * o.Prix);

        var disponibles = await _context.Ordinateurs
            .CountAsync(o => o.Stock > 0);

        // Nombre total de réparations
        var reparations = await _context.Reparations.CountAsync();

        return (total, disponibles, valeurStock, reparations);
    }


}
