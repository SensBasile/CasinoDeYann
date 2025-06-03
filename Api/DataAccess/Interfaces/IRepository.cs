namespace CasinoDeYann.Api.DataAccess.Interfaces;

public interface IRepository<DBEntity, ModelEntity>
{
    Task<IEnumerable<ModelEntity>> Get(string includeTables = "");
    
    ModelEntity GetOneById(long idEntity, string includeTables = "");
    Task<ModelEntity> Insert(ModelEntity entity);
    Task<ModelEntity> Update(ModelEntity entity);
    Task<bool> Delete(long idEntity);
}