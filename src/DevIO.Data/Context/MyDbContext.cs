using DevIO.Business.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevIO.Data.Context
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }
        public DbSet<Fornecedor> Fornecedores{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Definindo valor de tipo e tamanho padrão para alguma propriedade que tenha esquecido de ter sido mapeada
            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetProperties()
                .Where(p => p.ClrType == typeof(string))))
            {
                property.Relational().ColumnType = "varchar(100)";
            }

            // Aqui: aplica as validações criadas nas classes de mapeamento diretamente do Assembly onde elas foram definidas
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyDbContext).Assembly);

            // Impedindo que ocorra o cascade quando um registro seja apagado quando ocorrer DELETE entre tabelas relacionadas.
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}
