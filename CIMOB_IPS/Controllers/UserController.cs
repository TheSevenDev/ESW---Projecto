﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CIMOB_IPS.Controllers
{
    public class UserController : Controller
    {
        [HttpGet]
        public IActionResult Register()
        {

            return View();
        }

        public IActionResult Login()
        {

            return View();
        }

    }
}