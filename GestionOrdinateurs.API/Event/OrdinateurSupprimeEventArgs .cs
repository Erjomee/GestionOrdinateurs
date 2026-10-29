namespace GestionOrdinateurs.API;

public class OrdinateurSupprimeEventArgs : EventArgs
{
    public string Nom { get; set; } = string.Empty;
}