using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Nest;
using WWB.NEST.Repository.Entity;

namespace WWB.NEST.Repository
{
    public abstract class BaseElasticRepository<TEntity, TKey> where TEntity : class, IElasticEntity<TKey>, new()
    {
        protected ElasticClient Client { get; }
        protected string Index { get; }

        protected BaseElasticRepository(IElasticClientProvider elasticClientProvider, string index = null)
        {
            Index = index ?? typeof(TEntity).Name.ToLower();

            if (!string.IsNullOrWhiteSpace(elasticClientProvider.Options.Prefix))
            {
                Index = elasticClientProvider.Options.Prefix + Index;
            }

            Index = Index.ToLower();

            Client = elasticClientProvider.GetClient(Index);

            if (!Client.Indices.Exists(Index).Exists)
            {
                Client.CreateIndex<TEntity>(Index);
            }
        }

        public async Task<bool> IndexExistsAsync()
        {
            return (await Client.Indices.ExistsAsync(Index)).Exists;
        }

        public async Task<TEntity> GetAsync(TKey id)
        {
            var result = await Client.GetAsync<TEntity>(id.ToString());
            if (result.Found)
            {
                return result.Source;
            }

            return null;
        }

        public async Task InsertAsync(TEntity entity)
        {
            //这里可判断是否存在
            var response = await Client.IndexAsync(entity, s => s.Index(Index));

            if (!response.IsValid)
                throw new Exception("新增数据失败:" + response.OriginalException.Message);
        }

        public async Task UpdateAsync(TEntity entity)
        {
            var response = await Client.UpdateAsync<TEntity>(entity.Id.ToString(), x => x.Index(Index).Doc(entity));
            if (!response.IsValid)
                throw new Exception("更新失败:" + response.OriginalException.Message);
        }

        public async Task DeleteAsync(TKey id)
        {
            await Client.DeleteAsync<TEntity>(id.ToString(), x => x.Index(Index));
        }

        public async Task RemoveIndex()
        {
            var exists = await IndexExistsAsync();
            if (!exists) return;
            var response = await Client.Indices.DeleteAsync(Index);

            if (!response.IsValid)
                throw new Exception("删除index失败:" + response.OriginalException.Message);
        }

        public async Task<Tuple<long, List<TEntity>>> SearchAsync(int from, int size, QueryContainer query,
            List<ISort> sorts)
        {
            var searchRequest = new SearchRequest<TEntity>(Indices.Index(Index))
            {
                From = from,
                Size = size,
                Query = query,
                Sort = sorts,
            };


            var result = await Client.SearchAsync<TEntity>(searchRequest);

            return new Tuple<long, List<TEntity>>(result.Total, result.Documents.ToList());
        }
    }
}