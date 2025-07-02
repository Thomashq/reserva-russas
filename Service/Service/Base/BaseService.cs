using RR.Core.Repositories.Base;
using System;


namespace RR.Core.Services.Base
{
    public abstract class BaseService<TEntity, TKey> : IBaseService<TEntity, TKey>
         where TEntity : class
    {
        protected readonly IBaseRepository<TEntity, TKey> _repository;

        protected BaseService(IBaseRepository<TEntity, TKey> repository)
        {
            _repository = repository;
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public virtual async Task<TEntity?> GetByIdAsync(TKey id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public virtual async Task<TEntity> CreateAsync(TEntity entity)
        {
            await ValidateForCreate(entity);
            var result = await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return result;
        }

        public virtual async Task<TEntity?> UpdateAsync(TKey id, TEntity entity)
        {
            var existingEntity = await _repository.GetByIdAsync(id);
            if (existingEntity == null)
                return null;

            await ValidateForUpdate(id, entity);
            UpdateEntity(existingEntity, entity);
            var result = await _repository.UpdateAsync(existingEntity);
            await _repository.SaveChangesAsync();
            return result;
        }

        public virtual async Task<bool> DeleteAsync(TKey id)
        {
            var exists = await _repository.ExistsAsync(id);
            if (!exists)
                return false;

            await ValidateForDelete(id);
            await _repository.DeleteAsync(id);
            await _repository.SaveChangesAsync();
            return true;
        }

        public virtual async Task<bool> ExistsAsync(TKey id)
        {
            return await _repository.ExistsAsync(id);
        }

        // Métodos virtuais para validações e customizações
        protected virtual Task ValidateForCreate(TEntity entity)
        {
            return Task.CompletedTask;
        }

        protected virtual Task ValidateForUpdate(TKey id, TEntity entity)
        {
            return Task.CompletedTask;
        }

        protected virtual Task ValidateForDelete(TKey id)
        {
            return Task.CompletedTask;
        }

        protected virtual void UpdateEntity(TEntity existingEntity, TEntity newEntity)
        {
            // Override em classes derivadas para implementar lógica específica de atualização
        }
    }
}