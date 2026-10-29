namespace GestionOrdinateurs.Shared;

public class InventaireOrdinateurs
{
    public List<Ordinateur> Ordinateurs { get; set; } = new();

    // Recherche par nom
    public Ordinateur? this[string reference]
    {
        get => Ordinateurs.FirstOrDefault(o => o.Nom == reference);
    }

    // Recherche par position
    public Ordinateur this[int index]
    {
        get => Ordinateurs[index];
        set => Ordinateurs[index] = value;
    }
}