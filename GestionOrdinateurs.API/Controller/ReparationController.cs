using Microsoft.AspNetCore.Mvc;
using GestionOrdinateurs.Shared;
using Microsoft.AspNetCore.Authorization;

namespace GestionOrdinateurs.API;

[Authorize]
[ApiController]
[Route("reparations")]
public class ReparationController : ControllerBase
{
    private readonly ReparationService _reparService;
    private readonly OrdinateurService _ordinateurService;
    private readonly StockService _stockService;


    public ReparationController(
        ReparationService service,
        OrdinateurService ordinateurService,
        StockService stockService)
    {
        _reparService = service;
        _ordinateurService = ordinateurService;
        _stockService = stockService;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("SimulerReparation")]
    public async Task<IActionResult> SimulerReparation()
    {
        var ordinateurs = await _ordinateurService.GetAll();

        if (ordinateurs.Count < 5)
            return BadRequest("Il faut au moins 5 ordinateurs.");

        await Task.WhenAll(
            ordinateurs.Take(5).Select(o =>
                _reparService.SimulerReparationAsync(o))
        );

        return Ok("Les 5 réparations sont terminées.");
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var reparations = await _reparService.GetAll();
        return Ok(reparations);
    }

    [HttpGet("GetStockSSD")]
    public int GetStockSSD()
    {
        Parallel.For(0, 10, i => _stockService.ConsommerSSD().Wait());

        return _stockService.StockSSD;
    }

    [HttpGet("historique")]
    public async Task<ActionResult<List<HistoriqueReparationRecord>>> GetHistorique()
    {
        var historique = await _reparService.GetHistoriqueReparations();
        return Ok(historique);
    }
}