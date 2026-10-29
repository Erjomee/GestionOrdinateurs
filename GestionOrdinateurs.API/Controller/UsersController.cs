
using GestionOrdinateurs.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionOrdinateurs.API
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly ParcInformatiqueContext _db;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            ParcInformatiqueContext db,
            ILogger<UsersController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // GET /users
        // Récupère tous les utilisateurs
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            _logger.LogInformation(
                "Récupération de tous les utilisateurs");

            var users = await _db.Users.ToListAsync();

            return Ok(users);
        }

        // GET /users/{login}
        // Recherche un utilisateur par son login
        [HttpGet("{login}")]
        public async Task<IActionResult> GetUserByLogin(string login)
        {
            _logger.LogInformation(
                "Recherche de l'utilisateur {Login}", login);

            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Login == login);

            if (user == null)
            {
                _logger.LogWarning(
                    "Utilisateur {Login} introuvable", login);

                return NotFound();
            }

            return Ok(user);
        }
    }
}
