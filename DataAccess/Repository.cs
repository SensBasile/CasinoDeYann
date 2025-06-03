using AutoMapper;

using Microsoft.EntityFrameworkCore;

namespace CasinoDeYann.DataAccess;

public class Repository<DBEntity, ModelEntity> : IRepository<DBEntity, ModelEntity>
      where DBEntity : class, EfModels.IObjectWithId, new()
      where ModelEntity : class, Dbo.IObjectWithId, new()

    {
        private DbSet<DBEntity> _set;
        protected EfModels.CasinoDbContext _context;
        protected ILogger _logger;
        protected readonly IMapper _mapper;
        public Repository(EfModels.CasinoDbContext context, ILogger logger, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
            _logger = logger;
            _set = _context.Set<DBEntity>();
        }


        public virtual async Task<IEnumerable<ModelEntity>> Get(string includeTables = "")
        {
            try
            {
                List<DBEntity> query = null;
                if (String.IsNullOrEmpty(includeTables))
                {
                    query = await _set.AsNoTracking().ToListAsync();
                }
                else
                {
                    query = await _set.Include(includeTables).AsNoTracking().ToListAsync();
                }

                return _mapper.Map<ModelEntity[]>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError("error on db", ex);
                return null;
            }
        }
        
        public virtual ModelEntity GetOneById(long id, string includeTables = "")
        {
            try
            {
                DBEntity query = null;
                if (String.IsNullOrEmpty(includeTables))
                {
                    query = _set.AsNoTracking().First(x => x.Id == id);
                }
                else
                {
                    query = _set.Include(includeTables).AsNoTracking().First(x => x.Id == id);
                }

                return _mapper.Map<ModelEntity>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError("error on db", ex);
                return null;
            }
        }

        public virtual async Task<ModelEntity> Insert(ModelEntity entity)
        {
            DBEntity dbEntity = _mapper.Map<DBEntity>(entity);
            _set.Add(dbEntity);
            try
            {
                await _context.SaveChangesAsync();
                ModelEntity newEntity = _mapper.Map<ModelEntity>(dbEntity);
                return newEntity;
            }
            catch (Exception ex)
            {
                _logger.LogError("error on db", ex);
                return null;
            }

        }

        public virtual async Task<ModelEntity> Update(ModelEntity entity)
        {
            DBEntity dbEntity = _set.Find(entity.Id);


            if (dbEntity == null)
            {
                return null;
            }
            _mapper.Map(entity, dbEntity);
            if (!_context.ChangeTracker.HasChanges())
            {
                return entity;
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("error on db", ex);

                return null;
            }
            return _mapper.Map<ModelEntity>(dbEntity);

        }

        public virtual async Task<bool> Delete(long idEntity)
        {
            DBEntity dbEntity = _set.Find(idEntity);


            if (dbEntity == null)
            {
                return false;
            }
            _set.Remove(dbEntity);
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("error on db", ex);
                return false;
            }
        }
    }

