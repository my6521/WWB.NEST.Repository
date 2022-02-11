using Nest;

namespace WWB.NEST.Repository
{
    public interface IElasticClientProvider
    {
        ElasticOptions Options { get; }
        ElasticClient GetClient(string indexName);
    }
}