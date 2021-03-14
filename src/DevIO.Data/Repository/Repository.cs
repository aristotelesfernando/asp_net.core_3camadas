using DevIO.Business.Interfaces;
using DevIO.Business.Models;
using DevIO.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DevIO.Data.Repository
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity, new() //este argumento ao final que se possa instanciar TEntity
    {
        protected readonly MyDbContext Db;
        protected readonly DbSet<TEntity> DbSet;        

        public Repository(MyDbContext db)
        {
            Db = db;  // Injetando o DbContext para ficar acessível aos repositórios especializados
            DbSet = db.Set<TEntity>(); // Atalho para o DBSET
        }

        public async Task<IEnumerable<TEntity>> Buscar(Expression<Func<TEntity, bool>> predicade)
        {
            return await DbSet.AsNoTracking().Where(predicade).ToListAsync();
        }

        public virtual async Task<TEntity> ObterPorId(Guid id)
        {
            return await DbSet.FindAsync(id);
        }

        public virtual async Task<List<TEntity>> ObterTodos()
        {
            return await DbSet.ToListAsync();
        }

        public virtual async Task Adicionar(TEntity entity)
        {
            //Db.Set<TEntity>().Add(entity); ou
            DbSet.Add(entity);
            await SaveChanges();
        }

        public virtual async Task Atualizar(TEntity obj)
        {
            DbSet.Update(obj);
            await SaveChanges();
        }

        public virtual async Task Remover(Guid id)
        {
            //Pode ser feito assim...
            //DbSet.Remove(await DbSet.FindAsync(id));

            //Ou dessa forma... mais elegante
            var entity = new TEntity { Id = id };
            DbSet.Remove(entity);
            await SaveChanges();
        }

        public async Task<int> SaveChanges()
        {
            return await Db.SaveChangesAsync();
        }

        public void Dispose()
        {
            // Db? onde o ? quer dizer, se existir, faça o DISPOSE senão não faça nada... evita nullReference reference
            Db?.Dispose();
        }
    }
}
