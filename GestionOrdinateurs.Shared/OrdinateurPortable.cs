namespace GestionOrdinateurs.Shared;

public class OrdinateurPortable : Ordinateur
{
    public double TailleEcran { get; set; }

    public int AutonomieBatterie { get; set; }
    
     public void AfficherAutonomie()
    {
        Console.WriteLine(
            $"Autonomie : {AutonomieBatterie} heures"
        );
    }

    public bool EstAutonome()
    {
        return AutonomieBatterie >= 5;
    }

    
}