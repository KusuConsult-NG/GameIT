using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Models;
using Algorand.Algod.Api;
using Algorand.Algod.Model;
using Algorand.Client;
using Account = Algorand.Account;
using Algorand;

namespace WebApplication3.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public HomeController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> About()
        {
            var userIt = await _userManager.FindByNameAsync(User.Identity.Name);
            AlgodApi algodApiInstance = new AlgodApi("https://testnet-algorand.api.purestake.io/ps1", "B3SU4KcVKi94Jap2VXkK83xx38bsv95K5UZm2lab");
            //BuyerMnemonnic
            Account src = new Account(userIt.MnnemonicKey);
            var buyerAddress = userIt.AccountAddress;
            //var c = "OQIM6O74DXB454SCLT7KH7GJ5HJJE6DQDPPW6JNPVSRZ6RU4GVQMGKTH2Q";
            //Console.WriteLine(account1.Address.ToString());
            //Console.WriteLine(account2.Address.ToString());
            var accountInfo = algodApiInstance.AccountInformation(buyerAddress.ToString());
            //Console.WriteLine(string.Format($"Account Balance: {0} microAlgos", accountInfo.Amount));
            ViewData["acc"] = $"Account Balance: {accountInfo.Amount} microAlgos";
            var acts = algodApiInstance.AccountInformation(buyerAddress.ToString());
            var befores = "Account 1 balance before: " + acts.Amount.ToString();
            //Console.WriteLine(acts);
            //Console.WriteLine(befores);
            ViewData["address"] = $"{userIt.AccountAddress}";
            ViewData["acts"] = $"{acts}";
            ViewData["befores"] = $"{befores}";
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
