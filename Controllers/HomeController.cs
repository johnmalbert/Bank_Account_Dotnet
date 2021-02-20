using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Bank_Accounts.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace Bank_Accounts.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context;

        public HomeController(MyContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("NewUser")]
        public IActionResult NewUser(User newUser)
        {
            //create a new user
            if(ModelState.IsValid)
            {
                if(_context.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("Index");
                }
                //encrypt password
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                //add user to db
                _context.Add(newUser);
                _context.SaveChanges();
                var ThisUserId = _context.Users.FirstOrDefault(u => u.Email == newUser.Email).UserId;

                HttpContext.Session.SetInt32("UserId", ThisUserId);
                return Redirect($"accounts/{ThisUserId}");
            }
            else
            {
                return View("Index");
            }
        }

        [HttpPost("login-attempt")]
        public IActionResult LoginAttempt(Login newLogin)
        {
            if(ModelState.IsValid)
            {
                var userInDb = _context.Users.FirstOrDefault(u => u.Email == newLogin.Email);
                if(userInDb == null)
                {
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Login");
                }
                var hasher = new PasswordHasher<Login>();
                
                // verify provided password against hash stored in db
                var result = hasher.VerifyHashedPassword(newLogin, userInDb.Password, newLogin.Password);

                if(result == 0)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Login");
                }
                //set session to userid
                HttpContext.Session.SetInt32("UserId", userInDb.UserId);
                int? UserId = HttpContext.Session.GetInt32("UserId");
                return Redirect($"accounts/{UserId}");
            }
            else
            {
                return View("Login");
            }
        }
        
        [HttpGet("accounts/{UserId}")]
        public IActionResult Accounts(int UserId)
        {
            if(UserId != HttpContext.Session.GetInt32("UserId"))
            {
                return RedirectToAction("Index");
            }
            User CurrentUser = _context.Users.FirstOrDefault(u => u.UserId == UserId);

            ViewBag.UserTransactions = _context.Transactions.Where(t => t.UserId == UserId).OrderByDescending(u => u.CreatedAt).ToList();
            ViewBag.UserName = CurrentUser.FirstName;
            var AccountValue = 0;
            foreach (var item in ViewBag.UserTransactions)
            {
                AccountValue += item.Amount;
            }
            ViewBag.AccountValue = AccountValue.ToString("C2");
            return View();
        }

        [HttpPost("transaction")]
        public IActionResult Transaction(Transaction newTransaction)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            var CurrentUser = _context.Users.FirstOrDefault(u => u.UserId == (int)UserId);
            ViewBag.UserTransactions = _context.Transactions.Where(t => t.Creator == CurrentUser);            
            decimal AccountValue = 0;
            foreach (var item in ViewBag.UserTransactions)
            {
                AccountValue += item.Amount;
            }            
            if(ModelState.IsValid)
            {
                //add to db:
                if(AccountValue + newTransaction.Amount > 0)
                {
                    newTransaction.Creator = CurrentUser;
                    newTransaction.UserId = CurrentUser.UserId;
                    _context.Add(newTransaction);
                    _context.SaveChanges();
                }
                else
                {
                    ModelState.AddModelError("Amount", "Not enough funds!");
                    return View("Accounts");
                }
            }
            else
            {
                return View("Accounts");
            }
            return Redirect($"accounts/{UserId}");
        }
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
