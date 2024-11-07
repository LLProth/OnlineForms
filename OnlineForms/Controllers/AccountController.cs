using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Management;
using System.Web.Mvc;
using OnlineForms.ViewModels;
using OnlineForms.Models.SFN18795;
using OnlineForms.Models;
using System.Data;
using Microsoft.SharePoint.Client;
using Microsoft.VisualBasic;
using SP = Microsoft.SharePoint.Client;
using FormCollection = System.Web.Mvc.FormCollection;
using System.Security;
using System.Security.Principal;
using System.Net.Http;
using OnlineForms.Models.SFN61579;
using System.Net.Mail;
using System.Diagnostics;
using OnlineForms.Helper;
using System.Configuration;
using System.Web.Security;
using OnlineForms.Models.LogOn;

namespace OnlineForms.Controllers
{
 [AllowAnonymous]
    public class AccountController : Controller
    {
        private ModelDAL dal = new ModelDAL();

        // GET: Forms
        public ActionResult LogOn()
        {
            return View("~/Views/Account/LogOn.cshtml");
        }

        [HttpGet]
        public ActionResult SignIn(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(LogOnViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.LogOnModel.UserName, model.LogOnModel.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.LogOnModel.UserName, model.LogOnModel.RememberMe);
                    try
                    {
                        return Redirect(returnUrl);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect");
                }
            }

            // if we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("LogOn", "Account");
        }
    }
}