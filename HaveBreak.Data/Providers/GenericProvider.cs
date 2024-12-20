﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaveBreak.Data.Providers
{
    internal class GenericProvider<TEntity> where TEntity : class, IAuditedEntity
    {
        protected HaveBreakDbContext DbContext;

        /// <summary>
        /// IQueryable of entities where IsDeleted != true
        /// </summary>
        protected virtual IQueryable<TEntity> ActiveDbSet => DbContext.Set<TEntity>().Where(x => x.IsDeleted != true);

        protected async Task SoftDeleteListAsync(List<TEntity> list)
        {
            if (list.Any())
            {
                list.ForEach(x => x.IsDeleted = true);

                await DbContext.SaveChangesAsync();
            }
        }

        protected async Task SoftDeleteAsync(TEntity entity)
        {
            entity.IsDeleted = true;

            await DbContext.SaveChangesAsync();
        }
    }
}
