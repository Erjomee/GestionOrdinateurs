using GestionOrdinateurs.Shared;

namespace GestionOrdinateurs.API;

public interface IOrdinateurService
{
    // Événements
    event EventHandler<OrdinateurAjouteEventArgs>? OrdinateurAjoute;

    event EventHandler<OrdinateurSupprimeEventArgs>? OrdinateurSupprime;

    event EventHandler<OrdinateurMisAJourEventArgs>? OrdinateurMisAJour;

    // Méthodes du service
    Task<List<Ordinateur>> GetAll();

    Task<Ordinateur?> GetById(int id);

    Task Ajouter(Ordinateur ordinateur);

    Task Modifier(int id, Ordinateur ordinateur);

    Task Supprimer(int id);

    Task<List<Ordinateur>> GetOrdinateursDisponibles();
    Task<List<Ordinateur>> GetByType(TypeOrdinateur type);
    Task<List<Ordinateur>> GetOrdinateursParPrixDecroissant();
    string Diagnostiquer(Ordinateur ordinateur);

    Task<(int TotalOrdinateur, int OrdiDisponibles, decimal ValeurStock, int NbReparations)> CalculerStatistiques();
}