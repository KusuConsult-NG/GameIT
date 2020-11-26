using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Data;
using WebApplication3.Models;
using WebApplication3.Models.Games;
using Algorand.Algod.Api;
using Algorand.Algod.Model;
using Algorand.Client;
using Account = Algorand.Account;
using Algorand;

namespace WebApplication3.Controllers
{
    public class GameITModelsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GameITModelsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: GameITModels
        public async Task<IActionResult> Index()
        {
            return View(await _context.GameITModel.ToListAsync());
        }

        // GET: GameITModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var userIt = await _userManager.FindByNameAsync(User.Identity.Name);
            var gameITModel = await _context.GameITModel
                .SingleOrDefaultAsync(m => m.Id == id);
            AlgodApi algodApiInstance = new AlgodApi("https://testnet-algorand.api.purestake.io/ps1", "B3SU4KcVKi94Jap2VXkK83xx38bsv95K5UZm2lab");
            //BuyerMnemonnic
            Account src = new Account(userIt.MnnemonicKey);
            var buyerAddress = userIt.AccountAddress;
            var sellerAddress = gameITModel.GameAddressOwner;
            //var c = "OQIM6O74DXB454SCLT7KH7GJ5HJJE6DQDPPW6JNPVSRZ6RU4GVQMGKTH2Q";
            //Console.WriteLine(account1.Address.ToString());
            //Console.WriteLine(account2.Address.ToString());
            var accountInfo = algodApiInstance.AccountInformation(buyerAddress.ToString());
            Console.WriteLine(string.Format($"Account Balance: {0} microAlgos", accountInfo.Amount));
            var acts = algodApiInstance.AccountInformation(buyerAddress.ToString());
            var befores = "Account 1 balance before: " + acts.Amount.ToString();
            Console.WriteLine(acts);
            Console.WriteLine(befores);
            TransactionParams transParams = null;
            try
            {
                transParams = algodApiInstance.TransactionParams();
            }
            catch (ApiException err)
            {
                throw new Exception("Could not get params", err);
            }
            var amount = Utils.AlgosToMicroalgos(Convert.ToDouble(gameITModel.Price));
            var tx = Utils.GetPaymentTransaction(new Address(buyerAddress), new Address(buyerAddress), amount, "pay message", transParams);
            var signedTx = src.SignTransaction(tx);
            ViewBag.Tran = $"Signed transaction with txid: {signedTx.transactionID}";
            //Console.WriteLine("Signed transaction with txid: " + signedTx.transactionID);

            // send the transaction to the network
            try
            {
                var ids = Utils.SubmitTransaction(algodApiInstance, signedTx);
                ViewBag.Sent = $"Successfully sent tx with id: {ids.TxId}";
                //Console.WriteLine("Successfully sent tx with id: " + ids.TxId);
                ViewBag.WaitTran = Utils.WaitTransactionToComplete(algodApiInstance, ids.TxId);
                //Console.WriteLine(Utils.WaitTransactionToComplete(algodApiInstance, ids.TxId));
            }
            catch (ApiException e)
            {
                // This is generally expected, but should give us an informative error message.
                ViewBag.Exc = $"Exception when calling algod#rawTransaction: {e.Message}";
                //Console.WriteLine("Exception when calling algod#rawTransaction: " + e.Message);
            }
            ViewBag.Success = $"You have successefully arrived the end of this test, please press and key to exist.";
            //Console.WriteLine("You have successefully arrived the end of this test, please press and key to exist.");
            //Console.ReadKey();
            if (gameITModel == null)
            {
                return NotFound();
            }

            return View(gameITModel);
        }

        // GET: GameITModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GameITModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,GameName,Price,GameOwner,GameAddressOwner,GameAddressOwnerKey")] GameITModel gameITModel)
        {
            if (ModelState.IsValid)
            {
                var userIt = await _userManager.FindByNameAsync(User.Identity.Name);
                gameITModel.GameAddressOwner = userIt.AccountAddress;
                gameITModel.GameAddressOwnerKey = userIt.MnnemonicKey;
                gameITModel.GameOwner = User.Identity.Name;
                _context.Add(gameITModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gameITModel);
        }

        // GET: GameITModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gameITModel = await _context.GameITModel.SingleOrDefaultAsync(m => m.Id == id);
            if (gameITModel == null)
            {
                return NotFound();
            }
            return View(gameITModel);
        }

        // POST: GameITModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,GameName,Price,GameOwner,GameAddressOwner,GameAddressOwnerKey")] GameITModel gameITModel)
        {
            if (id != gameITModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userIt = await _userManager.FindByNameAsync(User.Identity.Name);
                    ApplicationUser user = new ApplicationUser();
                    gameITModel.GameAddressOwner = user.AccountAddress;
                    gameITModel.GameAddressOwnerKey = user.MnnemonicKey;
                    gameITModel.GameOwner = User.Identity.Name;
                    _context.Update(gameITModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameITModelExists(gameITModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(gameITModel);
        }

        // GET: GameITModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gameITModel = await _context.GameITModel
                .SingleOrDefaultAsync(m => m.Id == id);
            if (gameITModel == null)
            {
                return NotFound();
            }

            return View(gameITModel);
        }

        // POST: GameITModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gameITModel = await _context.GameITModel.SingleOrDefaultAsync(m => m.Id == id);
            _context.GameITModel.Remove(gameITModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> BuyIT(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gameITModel = await _context.GameITModel
                .SingleOrDefaultAsync(m => m.Id == id);
            var userIt = await _userManager.FindByNameAsync(User.Identity.Name);
            AlgodApi algodApiInstance = new AlgodApi("https://testnet-algorand.api.purestake.io/ps1", "B3SU4KcVKi94Jap2VXkK83xx38bsv95K5UZm2lab");
            //BuyerMnemonnic
            Account src = new Account(userIt.MnnemonicKey);
            var buyerAddress = userIt.AccountAddress;
            var sellerAddress = gameITModel.GameAddressOwner;
            //var c = "OQIM6O74DXB454SCLT7KH7GJ5HJJE6DQDPPW6JNPVSRZ6RU4GVQMGKTH2Q";
            //Console.WriteLine(account1.Address.ToString());
            //Console.WriteLine(account2.Address.ToString());
            var accountInfo = algodApiInstance.AccountInformation(buyerAddress.ToString());
            Console.WriteLine(string.Format($"Account Balance: {0} microAlgos", accountInfo.Amount));
            var acts = algodApiInstance.AccountInformation(buyerAddress.ToString());
            var befores = "Account 1 balance before: " + acts.Amount.ToString();
            Console.WriteLine(acts);
            Console.WriteLine(befores);
            TransactionParams transParams = null;
            try
            {
                transParams = algodApiInstance.TransactionParams();
            }
            catch (ApiException err)
            {
                throw new Exception("Could not get params", err);
            }
            var amount = Utils.AlgosToMicroalgos(Convert.ToDouble(gameITModel.Price));
            var tx = Utils.GetPaymentTransaction(new Address(buyerAddress), new Address(gameITModel.GameAddressOwner), amount, "pay message", transParams);
            var signedTx = src.SignTransaction(tx);
            ViewBag.Tran = $"Signed transaction with txid: {signedTx.transactionID}";
            //Console.WriteLine("Signed transaction with txid: " + signedTx.transactionID);

            // send the transaction to the network
            try
            {
                var ids = Utils.SubmitTransaction(algodApiInstance, signedTx);
                ViewBag.Sent = $"Successfully sent tx with id: {ids.TxId}";
                //Console.WriteLine("Successfully sent tx with id: " + ids.TxId);
                ViewBag.WaitTran = Utils.WaitTransactionToComplete(algodApiInstance, ids.TxId);
                //Console.WriteLine(Utils.WaitTransactionToComplete(algodApiInstance, ids.TxId));
            }
            catch (ApiException e)
            {
                // This is generally expected, but should give us an informative error message.
                ViewBag.Exc = $"Exception when calling algod#rawTransaction: {e.Message}";
                //Console.WriteLine("Exception when calling algod#rawTransaction: " + e.Message);
            }
            ViewBag.Success = $"You have successefully arrived the end of this test, please press and key to exist.";
            //Console.WriteLine("You have successefully arrived the end of this test, please press and key to exist.");
            //Console.ReadKey();
            if (gameITModel == null)
            {
                return NotFound();
            }

            return View(gameITModel);
        }

        // POST: GameITModels/Delete/5
        [HttpPost, ActionName("BuyIT")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuyIT(int id)
        {
            var gameITModel = await _context.GameITModel.SingleOrDefaultAsync(m => m.Id == id);
            var userIt = await _userManager.FindByNameAsync(User.Identity.Name);
            AlgodApi algodApiInstance = new AlgodApi("https://testnet-algorand.api.purestake.io/ps1", "B3SU4KcVKi94Jap2VXkK83xx38bsv95K5UZm2lab");
            //BuyerMnemonnic
            Account src = new Account(userIt.MnnemonicKey);
            var buyerAddress = userIt.AccountAddress;
            var sellerAddress = gameITModel.GameAddressOwner;
            //var c = "OQIM6O74DXB454SCLT7KH7GJ5HJJE6DQDPPW6JNPVSRZ6RU4GVQMGKTH2Q";
            //Console.WriteLine(account1.Address.ToString());
            //Console.WriteLine(account2.Address.ToString());
            var accountInfo = algodApiInstance.AccountInformation(buyerAddress.ToString());
            Console.WriteLine(string.Format($"Account Balance: {0} microAlgos", accountInfo.Amount));
            var acts = algodApiInstance.AccountInformation(buyerAddress.ToString());
            var befores = "Account 1 balance before: " + acts.Amount.ToString();
            Console.WriteLine(acts);
            Console.WriteLine(befores);
            TransactionParams transParams = null;
            try
            {
                transParams = algodApiInstance.TransactionParams();
            }
            catch (ApiException err)
            {
                throw new Exception("Could not get params", err);
            }
            var amount = Utils.AlgosToMicroalgos(Convert.ToDouble(gameITModel.Price));
            var tx = Utils.GetPaymentTransaction(new Address(buyerAddress), new Address(gameITModel.GameAddressOwner), amount, "pay message", transParams);
            var signedTx = src.SignTransaction(tx);
            ViewBag.Tran = $"Signed transaction with txid: {signedTx.transactionID}";
            //Console.WriteLine("Signed transaction with txid: " + signedTx.transactionID);

            // send the transaction to the network
            try
            {
                var ids = Utils.SubmitTransaction(algodApiInstance, signedTx);
                ViewBag.Sent = $"Successfully sent tx with id: {ids.TxId}";
                //Console.WriteLine("Successfully sent tx with id: " + ids.TxId);
                ViewBag.WaitTran = Utils.WaitTransactionToComplete(algodApiInstance, ids.TxId);
                //Console.WriteLine(Utils.WaitTransactionToComplete(algodApiInstance, ids.TxId));
            }
            catch (ApiException e)
            {
                // This is generally expected, but should give us an informative error message.
                ViewBag.Exc = $"Exception when calling algod#rawTransaction: {e.Message}";
                //Console.WriteLine("Exception when calling algod#rawTransaction: " + e.Message);
            }
            ViewBag.Success = $"You have successefully arrived the end of this test, please press and key to exist.";
            //Console.WriteLine("You have successefully arrived the end of this test, please press and key to exist.");
            //Console.ReadKey();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult BuyGame(int id, BuyGame gameITModel)
        {
            GameITModel postReply = _context.GameITModel.Find(id);
            if (postReply == null)
            {
                return NotFound();
            }
            if (postReply != null)
            {
                TempData["Message"] = string.Format("Updated successfully");
                
                _context.BuyGame.Add(gameITModel);
                _context.SaveChanges();
            }
            return RedirectToAction("Index", "Home");
        }
        private bool GameITModelExists(int id)
        {
            return _context.GameITModel.Any(e => e.Id == id);
        }
    }
}
