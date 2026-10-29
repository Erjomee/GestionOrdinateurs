namespace GestionOrdinateurs.Shared;

public class Reparation
{
    public int Id { get; set; }

    public string Description { get; set; } = string.Empty;

    public string TypeReparation { get; set; } = string.Empty;

    public DateTime DateDebut { get; set; }

    public string Statut { get; set; } = string.Empty;

    public int UnOrdinateurId { get; set; }

    public Ordinateur? UnOrdinateur { get; set; }

    public string Technicien { get; set; } = string.Empty;
}