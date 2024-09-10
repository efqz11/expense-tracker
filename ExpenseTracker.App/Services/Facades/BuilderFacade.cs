using AutoMapper;
using ExpenseTracker.Data;

namespace ExpenseTracker.App.Services.Facades
{
    public class BuilderFacade
    {

        private readonly ExpenseTrackerDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<BuilderFacade> _logger;

        public ExpenseTrackerDbContext Context
        {
            get
            {
                if (_context != null)
                {
                    return _context;
                }

                throw new ArgumentNullException("No connection to the database");

            }
        }

        public BuilderFacade(
            ExpenseTrackerDbContext context,
            IMapper mapper,
            ILogger<BuilderFacade> logger
            )
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public ExpenseTrackerDbContext GetDbContext()
        {
            return _context;
        }

        public IMapper GetMapper()
        {
            return _mapper;
        }


    }
}
