using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWeather.EF.Extensions
{
    public static class EFExtensions
    {
        public static EntityEntry<T> AddOrUpdate<T>(this DbContext context, T entity, params object[] keyValues) where T : class
        {
            var existingEntity = context.Find<T>(keyValues);

            if (existingEntity == null)
            {
                return context.Add(entity);
            }
            else
            {
                var entry = context.Entry(existingEntity);
                entry.CurrentValues.SetValues(entity);
                entry.State = EntityState.Modified;
                return entry;
            }
        }

        public static async Task<EntityEntry<T>> AddOrUpdateAsync<T>(this DbContext context, T entity, params object[] keyValues) where T : class
        {
            var existingEntity = await context.FindAsync<T>(keyValues);

            if (existingEntity == null)
            {
                return await context.AddAsync(entity);
            }
            else
            {
                var entry = context.Entry(existingEntity);
                entry.CurrentValues.SetValues(entity);
                entry.State = EntityState.Modified;
                return entry;
            }
        }
    }
}