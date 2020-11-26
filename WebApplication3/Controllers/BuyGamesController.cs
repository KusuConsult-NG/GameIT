using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Data;
using WebApplication3.Models.Games;
using Algorand.Algod.Api;
using Algorand.Algod.Model;
using Algorand.Client;
using Account = Algorand.Account;
using Algorand;

namespace WebApplication3.Controllers
{
    public class BuyGamesController : Controller
    {
        private readonly ApplicationDbContext _context;
        AlgodApi algodApiInstance = new AlgodApi("https://testnet-algorand.api.purestake.io/ps1", "B3SU4KcVKi94Jap2VXkK83xx38bsv95K5UZm2lab");
        public BuyGamesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BuyGames
        public async Task<IActionResult> Index()
        {
            return View(await _context.BuyGame.ToListAsync());
        }

        // GET: BuyGames/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var buyGame = await _context.BuyGame
                .SingleOrDefaultAsync(m => m.Id == id);
            AlgodApi algodApiInstance = new AlgodApi("https://testnet-algorand.api.purestake.io/ps1", "B3SU4KcVKi94Jap2VXkK83xx38bsv95K5UZm2lab");
            //BuyerMnemonnic
            Account src = new Account("ordinary certain payment annual plate slot squeeze recycle neck whip transfer cat photo error bus frost magnet arrange filter used soon medal liquid abstract father");
            //Console.WriteLine(account3.Address.ToString());
            //var c = "AJ7GLWNUT6TEJYUGNYAZQ7342TK7DBZLWBMIMK3NBN6NZCWJHLNYCRYQ7I";
            var buyerAddress = "36DTG5ITNWLAVZBSZ6BXPWP7UTSPEOL3K4HWIEUYML7RKUCBZQP5XBR7XY";
            var b = "EUFD3MONZCV6P65OHVHDTDY2B7OYALLNDORL2GJPB4RATJB4K2O3CUUVOU";
            var c = "OQIM6O74DXB454SCLT7KH7GJ5HJJE6DQDPPW6JNPVSRZ6RU4GVQMGKTH2Q";
            //Console.WriteLine(account1.Address.ToString());
            //Console.WriteLine(account2.Address.ToString());
            var accountInfo = algodApiInstance.AccountInformation(c.ToString());
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
            var amount = Utils.AlgosToMicroalgos(1);
            var tx = Utils.GetPaymentTransaction(new Address(buyerAddress), new Address(b), amount, "pay message", transParams);
            var signedTx = src.SignTransaction(tx);

            Console.WriteLine("Signed transaction with txid: " + signedTx.transactionID);

            // send the transaction to the network
            try
            {
                var ids = Utils.SubmitTransaction(algodApiInstance, signedTx);
                Console.WriteLine("Successfully sent tx with id: " + ids.TxId);
                Console.WriteLine(Utils.WaitTransactionToComplete(algodApiInstance, ids.TxId));
            }
            catch (ApiException e)
            {
                // This is generally expected, but should give us an informative error message.
                Console.WriteLine("Exception when calling algod#rawTransaction: " + e.Message);
            }

            Console.WriteLine("You have successefully arrived the end of this test, please press and key to exist.");
            Console.ReadKey();
            if (buyGame == null)
            {
                return NotFound();
            }

            return View(buyGame);
        }

        // GET: BuyGames/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BuyGames/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,GameName,AmountPaid")] BuyGame buyGame)
        {
            if (ModelState.IsValid)
            {
                _context.Add(buyGame);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(buyGame);
        }

        // GET: BuyGames/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var buyGame = await _context.BuyGame.SingleOrDefaultAsync(m => m.Id == id);
            if (buyGame == null)
            {
                return NotFound();
            }
            return View(buyGame);
        }

        // POST: BuyGames/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,GameName,AmountPaid")] BuyGame buyGame)
        {
            if (id != buyGame.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(buyGame);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BuyGameExists(buyGame.Id))
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
            return View(buyGame);
        }

        // GET: BuyGames/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var buyGame = await _context.BuyGame
                .SingleOrDefaultAsync(m => m.Id == id);
            if (buyGame == null)
            {
                return NotFound();
            }

            return View(buyGame);
        }

        // POST: BuyGames/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var buyGame = await _context.BuyGame.SingleOrDefaultAsync(m => m.Id == id);
            _context.BuyGame.Remove(buyGame);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BuyGameExists(int id)
        {
            return _context.BuyGame.Any(e => e.Id == id);
        }
        private static string BuildPost(string BuyerAddress, string SellerAddress, string GameToBuy, string Amount)
        {
            AlgodApi algodApiInstance = new AlgodApi("https://testnet-algorand.api.purestake.io/ps1", "B3SU4KcVKi94Jap2VXkK83xx38bsv95K5UZm2lab");
            Account src = new Account("ordinary certain payment annual plate slot squeeze recycle neck whip transfer cat photo error bus frost magnet arrange filter used soon medal liquid abstract father");
            //Console.WriteLine(account3.Address.ToString());
            //var c = "AJ7GLWNUT6TEJYUGNYAZQ7342TK7DBZLWBMIMK3NBN6NZCWJHLNYCRYQ7I";
            var a = "36DTG5ITNWLAVZBSZ6BXPWP7UTSPEOL3K4HWIEUYML7RKUCBZQP5XBR7XY";
            var b = "EUFD3MONZCV6P65OHVHDTDY2B7OYALLNDORL2GJPB4RATJB4K2O3CUUVOU";
            var c = "OQIM6O74DXB454SCLT7KH7GJ5HJJE6DQDPPW6JNPVSRZ6RU4GVQMGKTH2Q";
            //Console.WriteLine(account1.Address.ToString());
            //Console.WriteLine(account2.Address.ToString());
            var accountInfo = algodApiInstance.AccountInformation(c.ToString());
            Console.WriteLine(string.Format($"Account Balance: {0} microAlgos", accountInfo.Amount));
            var acts = algodApiInstance.AccountInformation(a.ToString());
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
            var amount = Utils.AlgosToMicroalgos(1);
            var tx = Utils.GetPaymentTransaction(new Address(a), new Address(b), amount, "pay message", transParams);
            var signedTx = src.SignTransaction(tx);

            Console.WriteLine("Signed transaction with txid: " + signedTx.transactionID);

            // send the transaction to the network
            try
            {
                var id = Utils.SubmitTransaction(algodApiInstance, signedTx);
                Console.WriteLine("Successfully sent tx with id: " + id.TxId);
                Console.WriteLine(Utils.WaitTransactionToComplete(algodApiInstance, id.TxId));
            }
            catch (ApiException e)
            {
                // This is generally expected, but should give us an informative error message.
                Console.WriteLine("Exception when calling algod#rawTransaction: " + e.Message);
            }

            Console.WriteLine("You have successefully arrived the end of this test, please press and key to exist.");
            Console.ReadKey();
            return "a";
        }
    }
}
