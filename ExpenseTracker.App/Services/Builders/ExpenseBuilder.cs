using ExpenseTracker.App.Interfaces.Builders;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.App.Services.Facades;
using ExpenseTracker.App.Models.Dto;
using ExpenseTracker.Data.Entities;
using ExpenseTracker.Data;
using ExpenseTracker.App.Models.Request;
using NuGet.Common;

namespace ExpenseTracker.App.Services.Builders
{
    public class ExpenseBuilder : BuilderFacadeBasedService, IExpenseBuilder
	{

		public ExpenseBuilder(BuilderFacade facade) : base(facade)
		{
		}

		private ExpenseDto? _expenseDto;

		public Expense Expense
		{
			get
			{
				if (_expenseDto?.Expense == null)
				{
					throw new ArgumentNullException("Error in expense");
				}

				return _expenseDto.Expense;
			}
			private set
			{
				_expenseDto.Expense = value;
			}
		}

		public ExpenseDto Dto
		{
			get
			{
				if (_expenseDto == null)
				{
					throw new ArgumentNullException("Error in DTO");
				}

				return _expenseDto;
			}
			set
			{
				_expenseDto = value;
			}
		}


		public Expense CreateValidExpense(ExpenseRequest screenRequest)
		{
			Mapper.Map(screenRequest, Dto);

			MapNewExpense();

			Validate();

			return Expense;

		}


		public Expense GetValidExpense(ExpenseUpdateRequest request)
		{
            Dto.PostedAt ??= DateTime.Now;

			Mapper.Map(request, Dto);

            Expense.Name = Dto.Name;
            Expense.Amount = Dto.Amount;
            Expense.PostedAt = Dto.PostedAt.Value!;
            Expense.CategoryId = Dto.CategoryId;

            Validate();

			Dto.IsChanged = true;

			return Expense;
		}


        private void Validate()
		{
            Category? category = Context.Categories.Find(Dto.CategoryId);

            if (category == null)
            {
                throw new ApplicationException("Category was not found");
            }

            // check other validations
        }

		private void MapNewExpense()
		{
            Dto.PostedAt ??= DateTime.Now;

			Expense = Mapper.Map<Expense>(Dto);
		}

    }
}
