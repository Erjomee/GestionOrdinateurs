using GestionOrdinateurs.API;
using GestionOrdinateurs.Shared;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Xunit;

namespace GestionOrdinateurs.Tests;

public class OrdinateurServiceTest
{
    // Crée une base de données en mémoire différente pour chaque test
    private ParcInformatiqueContext CreerContexte()
    {
        var options = new DbContextOptionsBuilder<ParcInformatiqueContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ParcInformatiqueContext(options);
    }

    // Crée un ordinateur avec le nouveau modèle
    private static Ordinateur CreerOrdinateur(
        int id,
        string nom,
        TypeOrdinateur type,
        int ramGo,
        int stockageGo,
        decimal prix = 0,
        EtatOrdinateur etat = EtatOrdinateur.EnService,
        int stock = 0)
    {
        return new Ordinateur
        {
            Id = id,
            Nom = nom,
            Etat = etat,
            Prix = prix,
            Stock = stock,
            Caracteristiques = new Caracteristiques(
                ramGo,
                stockageGo,
                type)
        };
    }

    // Vérifie qu'un ordinateur peut être ajouté
    [Fact]
    public async Task AjouterOrdinateur()
    {
        using var context = CreerContexte();
        var service = new OrdinateurService(context);

        var ordinateur = CreerOrdinateur(
            1, "PC-Test", TypeOrdinateur.Portable,
            16, 512, 800, stock: 5);

        await service.Ajouter(ordinateur);

        var ordinateurs = await service.GetAll();
        ordinateurs.Should().ContainSingle();

        var resultat = await service.GetById(1);
        resultat.Should().NotBeNull();
        resultat!.Nom.Should().Be("PC-Test");
    }

    // Vérifie la suppression d'un ordinateur existant
    [Fact]
    public async Task SupprimerOrdinateurExistant()
    {
        using var context = CreerContexte();

        context.Ordinateurs.Add(CreerOrdinateur(
            1, "PC-Test", TypeOrdinateur.Portable,
            16, 512, 800, stock: 5));

        await context.SaveChangesAsync();

        var service = new OrdinateurService(context);
        await service.Supprimer(1);

        var resultat = await service.GetAll();
        resultat.Should().BeEmpty();
    }

    // Vérifie la suppression d'un ordinateur inexistant
    [Fact]
    public async Task SupprimerOrdinateurInexistant()
    {
        using var context = CreerContexte();
        var service = new OrdinateurService(context);

        await service.Supprimer(99);

        var resultat = await service.GetAll();
        resultat.Should().BeEmpty();
    }

    // Vérifie la modification d'un ordinateur
    [Fact]
    public async Task ModifierOrdinateur()
    {
        using var context = CreerContexte();

        context.Ordinateurs.Add(CreerOrdinateur(
            1, "PC-Test", TypeOrdinateur.Portable,
            16, 512, 800, stock: 5));

        await context.SaveChangesAsync();

        var service = new OrdinateurService(context);

        var modification = CreerOrdinateur(
            1, "PC-Modifie", TypeOrdinateur.Fixe,
            32, 1000, 1200,
            EtatOrdinateur.EnPanne, stock: 0);

        await service.Modifier(1, modification);

        var resultat = await service.GetById(1);

        resultat.Should().NotBeNull();
        resultat!.Nom.Should().Be("PC-Modifie");
        resultat.Prix.Should().Be(1200);
        resultat.Stock.Should().Be(0);
        resultat.Etat.Should().Be(EtatOrdinateur.EnPanne);
        resultat.Caracteristiques.RamGo.Should().Be(32);
        resultat.Caracteristiques.StockageGo.Should().Be(1000);
        resultat.Caracteristiques.TypeOrdi.Should().Be(TypeOrdinateur.Fixe);
    }

    // Vérifie la recherche par type d'ordinateur
    [Fact]
    public async Task GetByType_RetourneLesOrdinateursDuType()
    {
        using var context = CreerContexte();

        context.Ordinateurs.AddRange(
            CreerOrdinateur(1, "PC-1", TypeOrdinateur.Portable, 16, 512),
            CreerOrdinateur(2, "PC-2", TypeOrdinateur.Fixe, 8, 256),
            CreerOrdinateur(3, "PC-3", TypeOrdinateur.Portable, 32, 1000)
        );

        await context.SaveChangesAsync();

        var service = new OrdinateurService(context);

        var resultat = await service.GetByType(TypeOrdinateur.Portable);

        resultat.Should().HaveCount(2);
        resultat.Should().OnlyContain(
            o => o.Caracteristiques.TypeOrdi == TypeOrdinateur.Portable);
    }

    // Vérifie que seuls les ordinateurs ayant un stock positif sont retournés
    [Fact]
    public async Task GetOrdinateursDisponibles_RetourneLeStockPositif()
    {
        using var context = CreerContexte();

        context.Ordinateurs.AddRange(
            CreerOrdinateur(
                1, "PC-1", TypeOrdinateur.Portable,
                16, 512, stock: 3),
            CreerOrdinateur(
                2, "PC-2", TypeOrdinateur.Fixe,
                8, 256, stock: 0)
        );

        await context.SaveChangesAsync();

        var service = new OrdinateurService(context);

        var resultat = await service.GetOrdinateursDisponibles();

        resultat.Should().ContainSingle();
        resultat[0].Stock.Should().BeGreaterThan(0);
    }

    // Vérifie le tri des ordinateurs du plus cher au moins cher
    [Fact]
    public async Task GetOrdinateursParPrixDecroissant_TrieLesPrix()
    {
        using var context = CreerContexte();

        context.Ordinateurs.AddRange(
            CreerOrdinateur(
                1, "PC-1", TypeOrdinateur.Portable, 16, 512, 500),
            CreerOrdinateur(
                2, "PC-2", TypeOrdinateur.Fixe, 16, 1000, 1200),
            CreerOrdinateur(
                3, "PC-3", TypeOrdinateur.Server, 32, 2000, 800)
        );

        await context.SaveChangesAsync();

        var service = new OrdinateurService(context);

        var resultat = await service.GetOrdinateursParPrixDecroissant();

        resultat.Select(o => o.Prix)
            .Should()
            .ContainInOrder(1200m, 800m, 500m);
    }


}