namespace GestionOrdinateurs.API;

public class OrdinateurMisAJourEventArgs : EventArgs
{
    public string Nom { get; set; } = string.Empty;
}