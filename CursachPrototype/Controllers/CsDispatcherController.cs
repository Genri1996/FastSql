﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CursachPrototype.Models.Accounting;
using CursachPrototype.ViewModels;
using DataProxy;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using CursachPrototype.ExtensionMethods;
using DataProxy.DbManangment;

namespace CursachPrototype.Controllers
{
    /// <summary>
    /// Handles databases actions
    /// </summary>
    public class CsDispatcherController : Controller
    {
        /// <summary>
        /// Reference to current user.
        /// </summary>
        private AppUserManager _userManager => System.Web.HttpContext.Current.GetOwinContext()
            .GetUserManager<AppUserManager>();

        /// <summary>
        /// Returns menu for creating database in some selected DBMS
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize]
        public ActionResult CreatePublicDb()
        {
            DbVm vm = new DbVm
            {
                AvailableServers = GetAvailableDbmsAsListString()
            };
            return View(vm);
        }

        /// <summary>
        /// Returns menu for creating database in some selected DBMS
        /// </summary>
        /// <returns></returns>
        [HttpGet, Authorize]
        public ActionResult CreateProtectedDb()
        {
            ProtectedDbVm vm = new ProtectedDbVm
            {
                AvailableServers = GetAvailableDbmsAsListString()
            };
            return View(vm);
        }

        /// <summary>
        /// /// <summary>
        /// Returns menu for creating anonymous database in some selected DBMS
        /// </summary>
        /// <returns></returns>
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CreateAnonymousDb()
        {
            AnonymousDbVm vm = new AnonymousDbVm
            {
                AvailableServers = GetAvailableDbmsAsListString()
            };
            return View(vm);
        }


        [HttpPost]
        public ActionResult CreatePublicDb(DbVm vm)
        {
            vm.AvailableServers = GetAvailableDbmsAsListString();

            if (!ModelState.IsValid)
                return View(vm);

            //Receives info
            AppUser user = _userManager.FindById(User.Identity.GetUserId());
            var tempObj = vm.ToCreateDatabaseObject(user);

            //Check Db Exists
            DbmsType selectedDbmsType = (DbmsType)Enum.Parse(typeof(DbmsType), vm.SelectedServer);
            if (DataService.CheckDataBaseExists(selectedDbmsType, tempObj.Name))
            {
                string errorMessage = "База данных с таким именем уже существует в этой СУБД.";
                ModelState.AddModelError("err", errorMessage);
                return View(vm);
            }

            //Try to create database
            String connectionString;
            try
            {
                connectionString = DataService.CreateDatabase(tempObj);
            }
            catch (Exception)
            {
                string errorMessage = "Не удалось создать базу данных.";
                return View("CustomError", (object)errorMessage);
            }

            tempObj.ConnectionString = connectionString;
            //Save action to database
            //Add new info to user. No ID and foreighn Key (!)
            DataBasesManager.AddDbInfo(tempObj);
            _userManager.Update(user);

            return View("ShowConnectionString", (object)connectionString);
        }

        /// <summary>
        /// Creates Database according to user`s vm selection
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost, Authorize]
        public ActionResult CreateProtectedDb(ProtectedDbVm vm)
        {
            vm.AvailableServers = GetAvailableDbmsAsListString();

            if (!ModelState.IsValid)
                return View(vm);

            //Receives info
            AppUser user = _userManager.FindById(User.Identity.GetUserId());
            var tempObj = vm.ToCreateDatabaseObject(user);

            //Check Db Exists
            DbmsType selectedDbmsType = (DbmsType)Enum.Parse(typeof(DbmsType), vm.SelectedServer);
            if (DataService.CheckDataBaseExists(selectedDbmsType, tempObj.Name))
            {
                string errorMessage = "База данных с таким именем уже существует в этой СУБД.";
                ModelState.AddModelError("err", errorMessage);
                return View(vm);
            }

            //Try to create database
            String connectionString;
            try
            {
                connectionString = DataService.CreateDatabase(tempObj, user.UserNickName, vm.DataBasePassword);
            }
            catch (Exception)
            {
                string errorMessage = "Не удалось создать базу данных.";
                return View("CustomError", (object)errorMessage);
            }

            tempObj.ConnectionString = connectionString;
            //Save action to database
            //Add new info to user. No ID and foreighn Key (!)
            DataBasesManager.AddDbInfo(tempObj);
            _userManager.Update(user);

            return View("ShowConnectionString", (object)connectionString);
        }

        [HttpPost]
        public ActionResult CreateAnonymousDb(AnonymousDbVm vm)
        {
            vm.AvailableServers = GetAvailableDbmsAsListString();

            if (!ModelState.IsValid)
                return View(vm);

            var tempObj = vm.ToCreateDatabaseObject();
            //Check Db Exists
            DbmsType selectedDbmsType = (DbmsType)Enum.Parse(typeof(DbmsType), vm.SelectedServer);
            if (DataService.CheckDataBaseExists(selectedDbmsType, tempObj.Name))
            {
                string errorMessage = "База данных с таким именем уже существует в этой СУБД.";
                ModelState.AddModelError("err", errorMessage);
                return View(vm);
            }

            //Try to create database
            String connectionString;
            try
            {
                connectionString = DataService.CreateDatabase(tempObj);
            }
            catch (Exception)
            {
                string errorMessage = "Не удалось создать базу данных.";
                return View("CustomError", (object)errorMessage);
            }
            tempObj.ConnectionString = connectionString;
            DataBasesManager.AddAnonymousDbInfo(tempObj);

            return View("ShowConnectionString", (object)connectionString);
        }

        //TODO: Use Enum List instead
        /// <summary>
        /// Returns all available Dbms As String List.
        /// </summary>
        /// <returns></returns>
        private List<string> GetAvailableDbmsAsListString()
        {
            return DataService.AvailableServers.Select(dbmsType => dbmsType.ToString()).ToList();
        }
    }
}