using AutoMapper;
using ExpenseTracker.Data;

namespace ExpenseTracker.App.Services.Facades
{
    public abstract class BuilderFacadeBasedService
    {

        private readonly BuilderFacade _facade;

        public BuilderFacadeBasedService(BuilderFacade facade)
        {
            _facade = facade;
        }

        public ExpenseTrackerDbContext Context
        {
            get
            {
                if (_facade != null)
                {
                    return _facade.GetDbContext();
                }

                throw new ArgumentNullException("No connection to the database");

            }
        }

        public IMapper Mapper
        {
            get
            {
                if (_facade != null)
                {
                    return _facade.GetMapper();
                }

                throw new ArgumentNullException("No initiated builder facade");

            }
        }
    }
}
