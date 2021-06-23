using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.RepositoryInterfaces;
using ApplicationCore.ServiceInterfaces;
using ApplicationCore.Models;
using System.Dynamic;
using Microsoft.AspNetCore.Authorization;
using ApplicationCore.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace BudgetTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IExpendRepository _expendRepository;
        private readonly IIncomeRepository _incomeRepository;
        private double balance = 0;
        public dynamic mymodel = new ExpandoObject();
        public UserController( IUserService userService, ICurrentUserService currentUserService, IExpendRepository expendRepository, IIncomeRepository incomeRepository)
        {
            
            _userService = userService;
            _currentUserService = currentUserService;
            _expendRepository = expendRepository;
             _incomeRepository = incomeRepository;
        }
        private void initBalance([FromQuery]List<ExpenditureResponseModel> expenses, [FromQuery]List<IncomeResponseModel> incomes)
        {

            foreach (var income in incomes)
            {
                this.balance += income.Amount;
            }
            foreach (var expense in expenses)
            {
                this.balance -= expense.Amount;
            }
            if (balance < 0) throw new Exception("balance is negative");
        }

        // ../user/id/Details/
        [Authorize]
        [HttpGet]
        [Route("{id:int}/Details")]
        public async Task<IActionResult> Details(int id)
        {
            List<ExpenditureResponseModel> exmodel = await this.GetExpense(id);
            List<IncomeResponseModel> inmodel = await this.GetIncome(id);
            initBalance(exmodel, inmodel);
            var userResponse = await _userService.GetUserDetails(id);
            var responseModel = new UserLoginResponseModel
            {
                FullName = userResponse.FullName,
                Email = userResponse.Email,
                Id = userResponse.Id,
            };

            mymodel.expense = exmodel;
            mymodel.income = inmodel;
            mymodel.responseModel = responseModel;
            return Ok(mymodel);

        }
        private async Task<List<ExpenditureResponseModel>> GetExpense(int id)
        {

            var userResponse = await _userService.GetUserDetails(id);
            //String email = userResponse.Email;
            //var user = await _userService.GetUser(email);
            var expense_test = await _expendRepository.GetByIdAsync(1);
            var expenses = await _expendRepository.GetAllExpense(id, 30, 1);
            List<ExpenditureResponseModel> exmodel = new List<ExpenditureResponseModel>();
            foreach (var expense in expenses)
            {
                exmodel.Add(new ExpenditureResponseModel
                {

                    Id = expense.Id,
                    Amount = expense.Amount,
                    Description = expense.Description,
                    Remarks = expense.Remarks,
                    ExpDate = expense.ExpDate,
                    UserId = expense.UserId
                });
            }

            return exmodel;

        }
        private async Task<List<IncomeResponseModel>> GetIncome(int id)
        {
            var incomes = await _incomeRepository.GetAllIncomes(id, 30, 1);
            List<IncomeResponseModel> inmodel = new List<IncomeResponseModel>();
            foreach (var income in incomes)
            {
                inmodel.Add(new IncomeResponseModel
                {
                    Id = income.Id,
                    Amount = income.Amount,
                    Description = income.Description,
                    Remarks = income.Remarks,
                    IncomeDate = income.IncomeDate,
                    UserId = income.UserId
                });
            }
            return inmodel;

        }

        // ../user/{userId}/Income
        [HttpGet("{userId:int}/Income")]
        public async Task<ActionResult> Income( int userId)
        {
            List<IncomeResponseModel> incomes = await this.GetIncome(userId);
            IncomeRequestModel InRequest = new IncomeRequestModel
            {

                UserId = userId,
                Description = "",
                Remarks = "",
                IncomeDate = DateTime.Now

            };
            mymodel.createInRequest = InRequest;
            mymodel.income = incomes;
            
            return Ok(mymodel);
        }


        // ...user/{userId}/Expense
        [HttpGet("{userId:int}/Expense")]
        public async Task<IActionResult> Expense(int userId)
        {
            List<ExpenditureResponseModel> expenses = await this.GetExpense(userId);
            ExpenditureRequestModel ExRequest = new ExpenditureRequestModel
            {

                UserId = userId,
                Description = "",
                Remarks = "",
                ExpDate = DateTime.Now

            };
            mymodel.createExRequest = ExRequest;
            mymodel.expense = expenses;
       
            return Ok(mymodel);
        }
        
        [Authorize]
        [HttpGet]
        [Route("{userId:int}/Expense/{id:int}")]
        public async Task<IActionResult> UpdateExpense(int userId, int id)
        {

            var expense = await _expendRepository.GetByIdAsync(id);
            ExpenditureResponseModel model = new ExpenditureResponseModel
            {
                Id = expense.Id,
                UserId = expense.UserId,
                Amount = expense.Amount,
                ExpDate = expense.ExpDate,
                Remarks = expense.Remarks,
                Description = expense.Description
            };
            mymodel.updateExpense = model;
            mymodel.exudpateRequest = new ExpenditureRequestModel
            {
                Id = expense.Id,
                UserId = expense.UserId,
                Amount = expense.Amount,
                ExpDate = expense.ExpDate,
                Remarks = expense.Remarks,
                Description = expense.Description
            };
            return Ok(mymodel);
        }

   
        [Authorize]
        [HttpPut]
        [Route("Expense")]
        public async Task<IActionResult> UpdateExpense(ExpenditureRequestModel expense)
        {
            // get model and update it in db and return to expense page
            Expenditure expensee = new Expenditure
            {
                Id = expense.Id,
                UserId = expense.UserId,
                Amount = expense.Amount,
                ExpDate = (DateTime)expense.ExpDate,
                Remarks = expense.Remarks,
                Description = expense.Description
            };

            await _expendRepository.UpdateAsync(expensee);
            return Ok();
        }


        [Authorize]
        [HttpPost("Expense")]
        public async Task<IActionResult> CreateExpense([FromBody]ExpenditureResponseModel expendModel)
        {
            Expenditure expense = new Expenditure
            {
                Id = expendModel.Id,
                UserId = expendModel.UserId,
                Amount = expendModel.Amount,
                ExpDate = (DateTime)expendModel.ExpDate,
                Description = expendModel.Description,
                Remarks = expendModel.Remarks

            };
            await _expendRepository.AddAsync(expense);
            balance -= expense.Amount;
            List<ExpenditureResponseModel> response = new List<ExpenditureResponseModel>();
            var expenses = await _expendRepository.GetAllExpense(expense.UserId);
            foreach (var incomee in expenses)
            {
                response.Add(new ExpenditureResponseModel
                {
                    Id = incomee.Id,
                    UserId = incomee.UserId,
                    Amount = incomee.Amount,
                    Description = incomee.Description,
                    Remarks = incomee.Remarks,
                    ExpDate = (DateTime)incomee.ExpDate
                });
            }
            mymodel.expense = response;
            return Ok();
        }

        [Authorize]
        [HttpPost("Income")]
        public async Task<IActionResult> CreateIncome([FromBody]IncomeRequestModel request)
        {
            Income income = new Income
            {
                Id = request.Id,
                UserId = request.UserId,
                Amount = request.Amount,
                Description = request.Description,
                Remarks = request.Remarks,
                IncomeDate = (DateTime)request.IncomeDate
            };

            await _incomeRepository.AddAsync(income);
            List<IncomeResponseModel> response = new List<IncomeResponseModel>();
            var incomes = await _incomeRepository.GetAllIncomes(income.UserId);
            foreach (var incomee in incomes)
            {
                response.Add(new IncomeResponseModel
                {
                    Id = incomee.Id,
                    UserId = incomee.UserId,
                    Amount = incomee.Amount,
                    Description = incomee.Description,
                    Remarks = incomee.Remarks,
                    IncomeDate = incomee.IncomeDate
                });
            }
            mymodel.income = response;
            return Ok( );
        }


        [Authorize]
        [HttpPut("Income")]
        public async Task<IActionResult> UpdateIncome(IncomeRequestModel request)
        {
            // get model and update it in db and return to expense page
            Income income = new Income
            {
                Id = request.Id,
                UserId = request.UserId,
                Amount = request.Amount,
                Description = request.Description,
                Remarks = request.Remarks,
                IncomeDate = (DateTime)request.IncomeDate
            };

            await _incomeRepository.UpdateAsync(income);
            return Ok();
        }

        [Authorize]
        [HttpDelete("{userId:int}/Expense/{id:int}")]
        public async Task<IActionResult> DeleteExpense(int userId, int id)
        {

            Expenditure expense = await _expendRepository.GetByIdAsync(id);
            if (expense != null)
            {
                await _expendRepository.DeleteAsync(expense);
                balance += expense.Amount;
            }
            var expenses = await _expendRepository.GetAllExpense(expense.UserId);
            List<ExpenditureResponseModel> response = new List<ExpenditureResponseModel>();
            foreach (var ex in expenses)
            {
                response.Add(new ExpenditureResponseModel
                {
                    Id = ex.Id,
                    UserId = ex.UserId,
                    Amount = ex.Amount,
                    Description = ex.Description,
                    ExpDate = ex.ExpDate,
                    Remarks = ex.Remarks

                });
            }
            mymodel.expense = response;

            return NoContent();

        }

        [Authorize]
        [HttpDelete("{userId:int}/Income/{id:int}")]
        public async Task<IActionResult> DeleteIncome(int userId, int id)
        {

            var income = await _incomeRepository.GetByIdAsync(id);
            if (income != null)
            {
                await _incomeRepository.DeleteAsync(income);
                balance -= income.Amount;
            }
            var incomes = await _incomeRepository.GetAllIncomes(income.UserId);
            List<IncomeResponseModel> response = new List<IncomeResponseModel>();
            foreach (var incomee in incomes)
            {
                response.Add(new IncomeResponseModel
                {
                    Id = incomee.Id,
                    UserId = incomee.UserId,
                    Amount = incomee.Amount,
                    Description = incomee.Description,
                    IncomeDate = incomee.IncomeDate,
                    Remarks = incomee.Remarks

                });
            }
            mymodel.income = response;
            return NoContent();

        }

    }
}
