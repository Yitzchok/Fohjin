using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;

namespace Fohjin.DDD.Reporting.Infrastructure
{
    public class NHibernateReportingRepository : IReportingRepository
    {
        public ISessionFactory DatabaseSession { get; set; }
        readonly ISession session;

        public NHibernateReportingRepository(ISessionFactory databaseSession)
        {
            DatabaseSession = databaseSession;
            session = databaseSession.OpenSession();
        }

        public IEnumerable<TDto> GetByExample<TDto>(object example) where TDto : class
        {
            return session
                  .CreateCriteria<TDto>().Add(Example.Create(example)).List<TDto>();
        }

        public void Save<TDto>(TDto dto) where TDto : class
        {
            session.Save(dto);
        }

        public void Update<TDto>(object update, object where) where TDto : class
        {
            session.Update(update, where);
        }

        public void Delete<TDto>(object example) where TDto : class
        {
            session.Delete(example);
        }
    }
}