using ApplicationCore.Entities;
using ApplicationCore.Models;
using ApplicationCore.RepositoryInterfaces;
using ApplicationCore.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace BudgetTracker.MVC.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IExpendRepository _expendRepository;

        private readonly IIncomeRepository _incomeRepository;
        private double balance = 0;
        public dynamic mymodel = new ExpandoObject();

        public UserController(IUserService userService, ICurrentUserService currentUserService, IExpendRepository expendRepository, IIncomeRepository incomeRepository)
        {
            _userService = userService;
            _currentUserService = currentUserService;
            _expendRepository = expendRepository;
            // _incomeService = incomeService;
            _incomeRepository = incomeRepository;
            mymodel.responseModel = null;
            mymodel.income = null;
            mymodel.expense = null;
            mymodel.createInRequest = null;
            mymodel.createExRequest = null;

        }

        public void initBalance(List<ExpenditureResponseModel> expenses, List<IncomeResponseModel> incomes)
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


        [HttpGet]
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
            return View(mymodel);

        }
        public async Task<List<ExpenditureResponseModel>> GetExpense(int id)
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
        public async Task<List<IncomeResponseModel>> GetIncome(int id)
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


        [HttpGet]
        public async Task<IActionResult> Expense(int id)
        {
            List<ExpenditureResponseModel> expenses = await this.GetExpense(id);
            ExpenditureRequestModel ExRequest = new ExpenditureRequestModel
            {

                UserId = id,
                Description = "",
                Remarks = "",
                ExpDate = DateTime.Now

            };
            mymodel.createExRequest = ExRequest;
            mymodel.expense = expenses;
            ViewData["id"] = id;
            return View(mymodel);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UpdateExpense(int id)
        {
            // get the expense to be updated and go to a form page
            // return responsemodel
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
            return View("UpdateExpense", mymodel);
        }

        [Authorize]
        [HttpPost]
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
            ExpenditureResponseModel model = new ExpenditureResponseModel
            {
                Id = expensee.Id,
                UserId = expensee.UserId,
                Amount = expensee.Amount,
                ExpDate = (DateTime)expensee.ExpDate,
                Remarks = expensee.Remarks,
                Description = expensee.Description
            };
            mymodel.updateExpense = model;

            return View("Expense", mymodel);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DeleteExpense(int id)
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

            return View("Expense", mymodel);

        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateExpense(ExpenditureResponseModel expendModel)
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
            return View("Expense", mymodel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateIncome(IncomeRequestModel request)
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
            balance += income.Amount;
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
            return View("Income", mymodel);
        }

        [HttpGet]
        public async Task<IActionResult> Income(int id)
        {
            List<IncomeResponseModel> incomes = await this.GetIncome(id);
            IncomeRequestModel InRequest = new IncomeRequestModel
            {

                UserId = id,
                Description = "",
                Remarks = "",
                IncomeDate = DateTime.Now

            };
            mymodel.createInRequest = InRequest;
            mymodel.income = incomes;
            return View(mymodel);
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UpdateIncome(int id)
        {
            // get the expense to be updated and go to a form page
            // return responsemodel
            var income = await _incomeRepository.GetByIdAsync(id);
            IncomeResponseModel model = new IncomeResponseModel
            {
                Id = income.Id,
                UserId = income.UserId,
                Amount = income.Amount,
                IncomeDate = income.IncomeDate,
                Remarks = income.Remarks,
                Description = income.Description
            };
            mymodel.updateIncome = model;
            mymodel.udpateRequest = new IncomeRequestModel
            {
                Id = income.Id,
                UserId = income.UserId,
                Amount = income.Amount,
                IncomeDate = income.IncomeDate,
                Remarks = income.Remarks,
                Description = income.Description
            };
            return View("UpdateIncome", mymodel);
        }
        [Authorize]
        [HttpPost]
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
            return View("Income", mymodel);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DeleteIncome(int id)
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
            return View("Income", mymodel);

        }




    }
}
