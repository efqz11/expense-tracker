using AutoMapper;
using ExpenseTracker.App.Interfaces.Services;
using ExpenseTracker.Data;

namespace ExpenseTracker.App.Services.Facades
{
    public class MainFacade<T>
    {
        private readonly ExpenseTrackerDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<T> _logger;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _configuration;

        public MainFacade(
            ExpenseTrackerDbContext context,
            IMapper mapper,
            ILogger<T> logger,
            IConfiguration configuration,
            IPermissionService permissionService
            )
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _permissionService = permissionService;
            _configuration = configuration;
        }

        public ExpenseTrackerDbContext GetDbContext()
        {
            return _context;
        }

        public IMapper GetMapper()
        {
            return _mapper;
        }

        public IPermissionService GetPermissionService()
        {
            return _permissionService;
        }

        public ILogger<T> GetLogger()
        {
            return _logger;
        }

        public IConfiguration GetConfiguration()
        {
            return _configuration;
        }
	}
}
