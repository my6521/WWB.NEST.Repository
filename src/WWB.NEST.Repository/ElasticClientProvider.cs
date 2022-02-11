using System;
using Microsoft.Extensions.Options;
using Nest;

namespace WWB.NEST.Repository
{
    public class ElasticClientProvider : IElasticClientProvider
    {
        public ElasticClientProvider(ElasticOptions options)
        {
            Options = options;
        }

        public ElasticOptions Options { get; }

        public ElasticClient GetClient(string indexName)
        {
            return GetClient(Options.Url, indexName);
        }

        private ElasticClient GetClient(string url, string indexName)
        {
            if (indexName == null) throw new ArgumentNullException(nameof(indexName));
       

            var uri = new Uri(url);
            var connectionSetting = new ConnectionSettings(uri);
            if (!string.IsNullOrWhiteSpace(indexName))
            {
                connectionSetting.DefaultIndex(indexName);
            }

            connectionSetting.BasicAuthentication(Options.UserName, Options.Password);
            return new ElasticClient(connectionSetting);
        }
    }
}