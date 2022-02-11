namespace WWB.NEST.Repository.Entity
{
    public interface IElasticEntity<TKey>
    {
        public TKey Id { get; set; }
    }
}