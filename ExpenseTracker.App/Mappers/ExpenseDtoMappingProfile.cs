using AutoMapper;
using ExpenseTracker.App.Models.Request;
using ExpenseTracker.App.Models.Dto;
using ExpenseTracker.Data.Entities;

namespace ExpenseTracker.App.Mappers
{
    public class ExpenseDtoMappingProfile : Profile
	{
		public ExpenseDtoMappingProfile()
		{

            CreateMap<ExpenseRequest, ExpenseDto>();

            CreateMap<ExpenseUpdateRequest, ExpenseDto>();

            CreateMap<ExpenseDto, Expense>();

			CreateMap<Expense, ExpenseDto>();

		}
	}
}
