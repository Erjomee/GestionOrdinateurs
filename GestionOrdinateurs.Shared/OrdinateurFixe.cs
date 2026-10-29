namespace GestionOrdinateurs.Shared;

public class OrdinateurFixe : Ordinateur
{
    public bool PossedeEcran { get; set; }

    public string TypeBoitier { get; set; } = string.Empty;

    public void AfficherTypeBoitier()
    {
        Console.WriteLine($"Type de boîtier : {TypeBoitier}");
    }


    public bool EstComplet()
    {
        return PossedeEcran;
    }
}