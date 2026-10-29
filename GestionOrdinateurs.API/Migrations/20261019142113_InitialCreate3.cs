using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionOrdinateurs.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reparations_Ordinateurs_UnOrdinateurID",
                table: "Reparations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ordinateurs",
                table: "Ordinateurs");

            migrationBuilder.DropColumn(
                name: "Marque",
                table: "Ordinateurs");

            migrationBuilder.DropColumn(
                name: "Processeur",
                table: "Ordinateurs");

            migrationBuilder.RenameTable(
                name: "Ordinateurs",
                newName: "Ordinateur");

            migrationBuilder.RenameColumn(
                name: "UnOrdinateurID",
                table: "Reparations",
                newName: "UnOrdinateurId");

            migrationBuilder.RenameIndex(
                name: "IX_Reparations_UnOrdinateurID",
                table: "Reparations",
                newName: "IX_Reparations_UnOrdinateurId");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Ordinateur",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "RAM",
                table: "Ordinateur",
                newName: "Etat");

            migrationBuilder.RenameColumn(
                name: "EtatOrdinateur",
                table: "Ordinateur",
                newName: "Caracteristiques_TypeOrdi");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Reparations",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Caracteristiques_RamGo",
                table: "Ordinateur",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Caracteristiques_StockageGo",
                table: "Ordinateur",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ordinateur",
                table: "Ordinateur",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reparations_Ordinateur_UnOrdinateurId",
                table: "Reparations",
                column: "UnOrdinateurId",
                principalTable: "Ordinateur",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reparations_Ordinateur_UnOrdinateurId",
                table: "Reparations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ordinateur",
                table: "Ordinateur");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Reparations");

            migrationBuilder.DropColumn(
                name: "Caracteristiques_RamGo",
                table: "Ordinateur");

            migrationBuilder.DropColumn(
                name: "Caracteristiques_StockageGo",
                table: "Ordinateur");

            migrationBuilder.RenameTable(
                name: "Ordinateur",
                newName: "Ordinateurs");

            migrationBuilder.RenameColumn(
                name: "UnOrdinateurId",
                table: "Reparations",
                newName: "UnOrdinateurID");

            migrationBuilder.RenameIndex(
                name: "IX_Reparations_UnOrdinateurId",
                table: "Reparations",
                newName: "IX_Reparations_UnOrdinateurID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Ordinateurs",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "Etat",
                table: "Ordinateurs",
                newName: "RAM");

            migrationBuilder.RenameColumn(
                name: "Caracteristiques_TypeOrdi",
                table: "Ordinateurs",
                newName: "EtatOrdinateur");

            migrationBuilder.AddColumn<string>(
                name: "Marque",
                table: "Ordinateurs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Processeur",
                table: "Ordinateurs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ordinateurs",
                table: "Ordinateurs",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Reparations_Ordinateurs_UnOrdinateurID",
                table: "Reparations",
                column: "UnOrdinateurID",
                principalTable: "Ordinateurs",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
