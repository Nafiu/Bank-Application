using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bitministry_Task.Models;
using System.Web.Security;
using System.Transactions;
using PagedList;
using PagedList.Mvc;

namespace Bitministry_Task.Controllers
{
    [Authorize]
    [SessionExpireFilter]
    public class HomeController : Controller
    {
        public ActionResult MyProfile()
        {
            using (BankAccountDb b = new BankAccountDb())
            {
                string us = User.Identity.Name;
                BankAccount balance = b.Generaltable.Where(a => a.Username.Equals(us)).FirstOrDefault();
                List<BankAccount> ban = new List<BankAccount>();
                ban.Add(balance);
                return View(ban);
            }
        }


        [HttpGet]
        public ActionResult Transfer()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Transfer(tranferamount ta)
        {
            bool debitmoney = false;
            bool creditmoney = false;

            if (ModelState.IsValid)
            {
                try
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        using (BankAccountDb b = new BankAccountDb())
                        {
                            var from = b.Generaltable.Where(a => a.Username.Equals(ta.Username)).FirstOrDefault();
                            var to = b.Generaltable.Where(a => a.Username.Equals(ta.To)).FirstOrDefault();
                            if (from.AccountBalance >= ta.Amount && to != null)
                            {
                                BankAccount bafrom = from;
                                bafrom.AccountBalance = bafrom.AccountBalance - ta.Amount;

                                BankAccount bato = to;
                                bato.AccountBalance = bato.AccountBalance + ta.Amount;

                                Transactionsdetail tddebit = new Transactionsdetail();
                                tddebit.Username = User.Identity.Name;
                                tddebit.Tousername = ta.To;
                                tddebit.Datetimes = DateTime.Now;
                                tddebit.Debited = ta.Amount;
                                tddebit.Credited = null;
                                tddebit.Balance = bafrom.AccountBalance;
                                tddebit.Typeofaction = "Transaction";
                                b.transdetails.Add(tddebit);

                                Transactionsdetail tdcredit = new Transactionsdetail();
                                tdcredit.Username = ta.To;
                                tdcredit.Tousername = User.Identity.Name;
                                tdcredit.Datetimes = DateTime.Now;
                                tdcredit.Debited = null;
                                tdcredit.Credited = ta.Amount;
                                tdcredit.Typeofaction = "Transaction";
                                tdcredit.Balance = bato.AccountBalance;
                                b.transdetails.Add(tdcredit);
                                b.SaveChanges();
                                ModelState.Remove("Amount");
                                ModelState.Remove("To");
                                debitmoney = true;
                                creditmoney = true;
                            }
                            if (debitmoney && creditmoney)
                            {
                                ts.Complete();
                                ViewBag.cla = "panel-success";
                                ViewBag.headi = "Transaction";
                                ViewBag.transfer = " Transaction completed Sucessfully";
                                return View("Partialviews");
                            }
                        }
                    }
                }
                catch
                {
                    ModelState.Remove("Amount");
                    ModelState.Remove("To");
                    ViewBag.cla = "panel-warning";
                    ViewBag.headi = "Transaction";
                    ViewBag.transfer = " Transaction Failed";
                    return View("Partialviews");
                }
                ViewBag.cla = "panel-warning";
                ViewBag.headi = "Transaction";
                ViewBag.transfer = " Transaction Failed due to receiver account is not available (or) you not having the balance";
                ModelState.Remove("Amount");
                ModelState.Remove("To");
                return View("Partialviews");
            }
            return View();
        }


        [HttpGet]
        public ActionResult Deposite()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Deposite(tranferamount ta)
        {
            if (ModelState.IsValid)
            {
                using (BankAccountDb b = new BankAccountDb())
                {
                    var userdeposite = b.Generaltable.Where(a => a.Username.Equals(ta.Username)).FirstOrDefault();
                    if (userdeposite != null)
                    {
                        BankAccount ba = userdeposite;
                        ba.AccountBalance = ba.AccountBalance + ta.Amount;

                        Transactionsdetail td = new Transactionsdetail();
                        td.Username = ta.Username;
                        td.Typeofaction = "Deposit";
                        td.Tousername = ta.To;
                        td.Credited = ta.Amount;
                        td.Datetimes = DateTime.Now;
                        td.Balance = ba.AccountBalance;
                        b.transdetails.Add(td);
                        b.SaveChanges();
                        ViewBag.cla = "panel-success";
                        ViewBag.headi = "Deposite";
                        ViewBag.transfer = "Deposited sucessfully";
                        return View("Partialviews");
                    }
                    ViewBag.cla = "panel-warning";
                    ViewBag.headi = "Deposite";
                    ViewBag.transfer = "Deposited Failed ckeck your username";
                    return View("Partialviews");
                }
            }
            return View();
        }


        [HttpGet]
        public ActionResult Withdraw()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Withdraw(tranferamount ta)
        {
            ta.Username = User.Identity.Name;
            if (ModelState.IsValid)
            {
                using (BankAccountDb b = new BankAccountDb())
                {
                    var userwithdraw = b.Generaltable.Where(a => a.Username.Equals(ta.Username)).FirstOrDefault();
                    if (userwithdraw != null && userwithdraw.AccountBalance >= ta.Amount)
                    {
                        BankAccount ba = userwithdraw;
                        ba.AccountBalance = ba.AccountBalance - ta.Amount;

                        Transactionsdetail td = new Transactionsdetail();
                        td.Username = ta.Username;
                        td.Tousername = ta.To;
                        td.Typeofaction = "Withdraw";
                        td.Balance = ba.AccountBalance;
                        td.Debited = ta.Amount;
                        td.Datetimes = DateTime.Now;
                        b.transdetails.Add(td);
                        b.SaveChanges();
                        ViewBag.cla = "panel-success";
                        ViewBag.headi = "Withdraw";
                        ViewBag.transfer = "Withdraw sucessfully";
                        return View("Partialviews");
                    }
                    ViewBag.cla = "panel-warning";
                    ViewBag.headi = "Withdraw";
                    ViewBag.transfer = "Withdraw Failed Check your account name and your balance";
                    return View("Partialviews");
                }
            }
            return View();
        }

        public ActionResult Statement(string fromd, string tod, int? page)
        {
            BankAccountDb bae = new BankAccountDb();
            DateTime f = Convert.ToDateTime(fromd,System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
            DateTime t = Convert.ToDateTime(tod, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
            ViewBag.fd = fromd; ViewBag.td = tod;
            var ss = bae.transdetails.Where(a => a.Username.Equals(User.Identity.Name) &&
            (a.Datetimes >= f && a.Datetimes <= t)).ToList().ToPagedList(page ?? 1, 5);
            if ((ss.Count == 0 && fromd != null) && f >= t)
            {
                ViewBag.er = "No Transaction Occured";
            }
            return View(ss);
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(BankAccount ba)
        {
            string encryptpassword = FormsAuthentication.HashPasswordForStoringInConfigFile(ba.password, "SHA1");
            BankAccountDb db = new BankAccountDb();
            BankAccount b = new BankAccount();
            b.Username = ba.Username;
            b.AccountBalance = ba.AccountBalance;
            b.password = encryptpassword;
            db.Generaltable.Add(b);
            db.SaveChanges();

            Transactionsdetail t = new Transactionsdetail();
            t.Username = ba.Username;
            t.Tousername = ba.Username;
            t.Datetimes = DateTime.Now;
            t.Credited = 18658;
            t.Balance = ba.AccountBalance;
            t.Typeofaction = "Deposited";
            db.transdetails.Add(t);
            db.SaveChanges();
            return View();
        }

    }
}