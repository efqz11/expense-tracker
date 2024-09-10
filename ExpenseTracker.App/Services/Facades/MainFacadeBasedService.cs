using AutoMapper;
using ExpenseTracker.App.Interfaces.Services;
using ExpenseTracker.Data;

namespace ExpenseTracker.App.Services.Facades
{
    public abstract class MainFacadeBasedService<T>
    {

        private readonly MainFacade<T> _facade;

        public MainFacadeBasedService(MainFacade<T> facade)
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

                throw new ArgumentNullException("Facade not initiated");

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

                throw new ArgumentNullException("Facade not initiated");

            }
        }

        public IPermissionService PermissionService
        {
            get
            {
                if (_facade != null)
                {
                    return _facade.GetPermissionService();
                }

                throw new ArgumentNullException("Facade not initiated");

            }
        }

        public ILogger<T> Logger
        {
            get
            {
                if (_facade != null)
                {
                    return _facade.GetLogger();
                }

                throw new ArgumentNullException("Facade not initiated");

            }
        }

        public string GetValidUserIdToAccess(string? id = null)
        {
            return PermissionService.GetValidUserIdToAccess(id);
        }

        public IConfiguration Configuration
        {
            get
            {
                if (_facade != null)
                {
                    return _facade.GetConfiguration();
                }

                throw new ArgumentNullException("Facade not initiated");

            }
        }

    }
}
