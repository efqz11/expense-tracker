using AutoMapper;
using ExpenseTracker.App.Models.Request;
using ExpenseTracker.App.Models.Response;
using ExpenseTracker.Data.Entities;

namespace ExpenseTracker.App.Mappers
{
    public class SimpleExpenseProfile : Profile
    {
        public SimpleExpenseProfile()
        {
            CreateMap<Expense, SimpleExpense>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.ToString("N2")))
                .ForMember(dest => dest.PostedAt, opt => opt.MapFrom(src => src.PostedAt.ToString("dd-MMM-yyyy HH:mm")));


            CreateMap<Expense, ExpenseUpdateRequest>();
        }
    }
}
