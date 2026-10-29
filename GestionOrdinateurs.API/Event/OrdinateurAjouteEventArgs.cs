namespace GestionOrdinateurs.API;

public class OrdinateurAjouteEventArgs : EventArgs
{
    public string Nom { get; set; } = string.Empty;
}