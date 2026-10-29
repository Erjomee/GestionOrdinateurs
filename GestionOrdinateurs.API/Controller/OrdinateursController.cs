using GestionOrdinateurs.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GestionOrdinateurs.API
{
    [Authorize]
    [ApiController]
    [Route("ordinateurs")]
    public class OrdinateursController : ControllerBase
    {
        private readonly IOrdinateurService service;
        private readonly ILogger<OrdinateursController> _logger;

        private readonly IConfiguration _configuration;
        private readonly JournalService _journal;

        // Constructeur avec injection des dépendances
        public OrdinateursController(
            IOrdinateurService service,
            ILogger<OrdinateursController> logger,
            IConfiguration configuration,
            JournalService journal
            )
        {
            this.service = service;
            _logger = logger;
            _configuration = configuration;
            _journal = journal;

            service.OrdinateurAjoute += AfficherMessageAjout;
            service.OrdinateurSupprime += AfficherMessageSuppression;
            service.OrdinateurMisAJour += AfficherMessageMiseAJour;
        }

        // GET /ordinateurs
        // Recupere la liste de tous les ordinateurs
        [HttpGet]
        public async Task<IActionResult> GetOrdinateurs()
        {
            _logger.LogInformation("Récupération de tous les ordinateurs");

            return Ok(await service.GetAll());
        }

        // GET /ordinateurs/{id}
        // Recupere un ordinateur à partir de son identifiant
        [HttpGet("{id}")]
        public async Task<IActionResult> GetComputerById(int id)
        {
            _logger.LogInformation(
                "Recherche de l'ordinateur avec l'ID {Id}", id);

            var ordinateur = await service.GetById(id);

            // Vérifie si l'ordinateur existe
            if (ordinateur == null)
            {
                _logger.LogWarning(
                    "Ordinateur avec l'ID {Id} introuvable", id);

                return NotFound();
            }

            return Ok(ordinateur);
        }

        // POST /ordinateurs
        // Ajoute un nouvel ordinateur dans la base de donnée
        [HttpPost]
        public async Task<IActionResult> CreateOrdinateur(Ordinateur ordinateur)
        {
            _logger.LogInformation(
                "Ajout de l'ordinateur {Nom}", ordinateur.Nom);

            await service.Ajouter(ordinateur);

            // on retourne l'ordinateur créé avec son URL
            return Created($"/ordinateurs/{ordinateur.Id}", ordinateur);
        }

        // PUT /ordinateurs/{id}
        // Modifie un ordinateur existant
        [HttpPut("{id}")]
        public async Task<IActionResult> ModifierOrdinateur(
            int id,
            Ordinateur ordinateur)
        {
            _logger.LogInformation(
                "Modification de l'ordinateur avec l'ID {Id}", id);

            var existant = await service.GetById(id);

            // Vérifie si l'ordinateur existe
            if (existant == null)
            {
                _logger.LogWarning(
                    "Impossible de modifier : ordinateur {Id} introuvable", id);

                return NotFound();
            }

            await service.Modifier(id, ordinateur);

            return Ok(service.GetById(id));
        }

        // DELETE /ordinateurs/{id}
        // Supprime un ordinateur à partir de son identifiant
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrdinateur(int id)
        {
            _logger.LogInformation(
                "Suppression de l'ordinateur avec l'ID {Id}", id);

            var existant = await service.GetById(id);

            if (existant == null)
            {
                _logger.LogWarning(
                    "Impossible de supprimer : ordinateur {Id} introuvable", id);

                return NotFound();
            }

            await service.Supprimer(id);
            // Retourne le statut HTTP 204 indiquant la suppression
            return NoContent();
        }

        // GET /api/ordinateurs/info
        // Retourne les informations de l'application
        [HttpGet("info")]
        public async Task<IActionResult> GetInfoApp()
        {
            // On les informations dans des variables depuis appsettings.json
            var nom = _configuration["Application:Name"];
            var version = _configuration["Application:Version"];

            _logger.LogInformation("Informations de l'application récupérées");

            return Ok(new
            {
                Nom = nom,
                Version = version,
            });
        }

        // GET /ordinateurs/disponibles
        // Retourne uniquement les ordinateurs ayant un stock supérieur à zéro
        [HttpGet("disponibles")]
        public async Task<IActionResult> GetOrdinateursDisponibles()
        {
            _logger.LogInformation(
                "Récupération des ordinateurs disponibles en stock");

            var ordinateurs = await service.GetOrdinateursDisponibles();

            return Ok(ordinateurs);
        }

        // Recherche les ordinateurs correspondant à une marque
        [HttpGet("type/{type}")]
        public async Task<IActionResult> GetByType(TypeOrdinateur type)
        {
            _logger.LogInformation(
                "Recherche des ordinateurs de type {Type}", type);
            var ordinateurs = await service.GetByType(type);
            return Ok(ordinateurs);
        }

        // GET /ordinateurs/prix/decroissant
        // Retourne les ordinateurs dans l'ordre des prix décroissant
        [HttpGet("prix/decroissant")]
        public async Task<IActionResult> GetOrdinateursParPrixDecroissant()
        {
            _logger.LogInformation(
                "Récupération des ordinateurs triés par prix décroissant");

            var ordinateurs = await service.GetOrdinateursParPrixDecroissant();

            return Ok(ordinateurs);
        }

        [HttpGet("{id}/diagnostic")]
        public async Task<IActionResult> Diagnostiquer(int id)
        {
            var ordinateur = await service.GetById(id);

            if (ordinateur == null)
            {
                return NotFound();
            }

            var diagnostic = service.Diagnostiquer(ordinateur);

            return Ok(new
            {
                OrdinateurId = id,
                Nom = ordinateur.Nom,
                Diagnostic = diagnostic
            });
        }

        // GET /ordinateurs/statistiques
        [HttpGet("statistiques")]
        public async Task<IActionResult> GetStatistiques()
        {
            var (total, disponibles, valeurStock, reparations)
                = await service.CalculerStatistiques();

            return Ok(new
            {
                TotalOrdinateurs = total,
                OrdinateursDisponibles = disponibles,
                ValeurStock = valeurStock,
                NombreReparations = reparations
            });
        }

        // GET /ordinateurs/inventaire/{nom}
        [HttpGet("inventaire/{nom}")]
        public async Task<IActionResult> Chercher(string nom)
        {
            var inventaire = new InventaireOrdinateurs();

            inventaire.Ordinateurs = await service.GetAll();

            var ordinateur = inventaire[nom];

            if (ordinateur == null)
            {
                return NotFound();
            }

            return Ok(ordinateur);
        }

        // Event: Affiche un message lorsqu'un ordinateur est ajouté
        private void AfficherMessageAjout(
            object? sender, OrdinateurAjouteEventArgs e)
        {
            _logger.LogInformation("Nouvel ordinateur ajouté : {Nom}", e.Nom);

            _journal.Ecrire(
                $"{e.Nom} ajouté",
                "Ordinateurs/ordinateurs.log");
        }

        private void AfficherMessageSuppression(
            object? sender, OrdinateurSupprimeEventArgs e)
        {
            _logger.LogInformation("Ordinateur supprimé : {Nom}", e.Nom);

            _journal.Ecrire(
                $"{e.Nom} supprimé",
                "Ordinateurs/ordinateurs.log");
        }

        private void AfficherMessageMiseAJour(
            object? sender, OrdinateurMisAJourEventArgs e)
        {
            _logger.LogInformation("Ordinateur mis à jour : {Nom}", e.Nom);

            _journal.Ecrire(
                $"{e.Nom} modifié",
                "Ordinateurs/ordinateurs.log");
        }
        
    }
}