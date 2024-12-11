using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using CledAcademy.Core.Domain;
using CledAcademy.Core.Models;
using CledAcademy.Core.Models.HesabAz;
using CledAcademy.Core.Models.StaticFiles;
using CledAcademy.Repository.UOW;
using CledAcademy.Web.Models;
using CledAcademy.Web.Models.ViewModels;
using CledAcademy.Web.Models.ViewModels.AccountViewModels;
using CledAcademy.Web.Services.Abstract;


namespace CledAcademy.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IEmailSender _emailSender;
        private readonly IDataDictionaryService _dataDictionaryService;
        private readonly ILogger _logger;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
                                 IUnitOfWorkManager unitOfWorkManager, IEmailSender emailSender, ILoggerFactory loggerFactory,
                                 IDataDictionaryService dataDictionaryService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWorkManager = unitOfWorkManager;
            _emailSender = emailSender;
            _dataDictionaryService = dataDictionaryService;
            _logger = loggerFactory.CreateLogger<AccountController>();
        }

        //
        // GET: /Account/Index
        [HttpGet]
        public IActionResult Index()
        {
            var student = this.GetStudent(_userManager, _unitOfWorkManager.Repository<Student>());
            var accountTopUps =
                _unitOfWorkManager.Repository<AccountTopUp>().FindBy(at => at.StudentId == student.Id && at.StatusCode != null)
                                  .ToList();
            var studentOrders =
                _unitOfWorkManager.Repository<StudentOrder>().FindBy(so => so.StudentId == student.Id).Select(
                    so =>
                        new StudentOrderViewModel
                        {
                            Id = so.Id,
                            OrderId = so.OrderId,
                            OrderType = so.OrderType,
                            Price = so.Fee,
                            DateTime = so.DateTime
                        }).ToList();

            foreach(var item in studentOrders)
                switch(item.OrderType)
                {
                    case OrderType.Lesson:
                        var lesson = _unitOfWorkManager.Repository<Lesson>().GetSingle(item.OrderId, l => l.Section.Course);
                        item.Name = lesson.Section.Course.Name + " / " + lesson.Section.Name + " / " + lesson.Name;
                        break;
                    case OrderType.Section:
                        var section = _unitOfWorkManager.Repository<Section>().GetSingle(item.OrderId, s => s.Course);
                        item.Name = section.Course.Name + " / " + section.Name;
                        break;
                    case OrderType.Course:
                        var course = _unitOfWorkManager.Repository<Course>().GetSingle(item.OrderId);
                        item.Name = course.Name;
                        break;
                }

            var accountViewModel = new AccountViewModel { StudentOrders = studentOrders, AccountTopUps = accountTopUps };
            ViewBag.Student = student;
            return View(accountViewModel);
        }

        //
        // GET: /Account/Login
        [HttpGet]
        public IActionResult ShoppingCard(string result)
        {
            if(result != null) ViewBag.Message = this.GetMessage(result);

            var student = this.GetStudent(_userManager, _unitOfWorkManager.Repository<Student>());

            var items =
                _unitOfWorkManager.Repository<ShoppingCard>().FindBy(sc => sc.StudentId == student.Id).Select(
                    sc =>
                        new ShoppingCardViewModel
                        {
                            Id = sc.Id,
                            OrderId = sc.OrderId,
                            StudentId = sc.StudentId,
                            OrderType = sc.OrderType
                        }).ToList();

            var courseBuyDiscount = _dataDictionaryService.GetSingle("CourseBuyDiscount");
            var sectionBuyDiscount = _dataDictionaryService.GetSingle("SectionBuyDiscount");

            foreach(var item in items)
                switch(item.OrderType)
                {
                    case OrderType.Lesson:
                        var lesson = _unitOfWorkManager.Repository<Lesson>().GetSingle(item.OrderId, l => l.Section.Course);
                        item.Name = lesson.Section.Course.Name + " / " + lesson.Section.Name + " / " + lesson.Name;
                        item.Price = lesson.Price;
                        break;
                    case OrderType.Section:
                        var section = _unitOfWorkManager.Repository<Section>().GetSingle(item.OrderId, s => s.Course);
                        item.Name = section.Course.Name + " / " + section.Name;
                        item.Price =
                            _unitOfWorkManager.Repository<Lesson>().FindBy(l => l.SectionId == section.Id).Sum(l => l.Price) *
                            double.Parse(sectionBuyDiscount.Value);
                        break;
                    case OrderType.Course:
                        var course = _unitOfWorkManager.Repository<Course>().GetSingle(item.OrderId);
                        item.Name = course.Name;
                        item.Price =
                            _unitOfWorkManager.Repository<Lesson>().FindBy(l => l.Section.CourseId == course.Id).Sum(l => l.Price) *
                            double.Parse(courseBuyDiscount.Value);
                        break;
                }

            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CheckOut()
        {
            var student = this.GetStudent(_userManager, _unitOfWorkManager.Repository<Student>());
            var items =
                _unitOfWorkManager.Repository<ShoppingCard>().FindBy(sc => sc.StudentId == student.Id).Select(
                    sc =>
                        new ShoppingCardCheckoutModel
                        {
                            Id = sc.Id,
                            OrderId = sc.OrderId,
                            StudentId = sc.StudentId,
                            OrderType = sc.OrderType
                        }).ToList();

            var courseBuyDiscount = _dataDictionaryService.GetSingle("CourseBuyDiscount");
            var sectionBuyDiscount = _dataDictionaryService.GetSingle("SectionBuyDiscount");

            foreach(var item in items)
                switch(item.OrderType)
                {
                    case OrderType.Lesson:
                        var lesson = _unitOfWorkManager.Repository<Lesson>().GetSingle(item.OrderId, l => l.Section.Course);
                        item.Lesson = lesson;
                        item.Name = lesson.Section.Course.Name + " / " + lesson.Section.Name + " / " + lesson.Name;
                        item.Price = lesson.Price;
                        break;
                    case OrderType.Section:
                        var section = _unitOfWorkManager.Repository<Section>().GetSingle(item.OrderId, s => s.Course);
                        item.Section = section;
                        item.Name = section.Course.Name + " / " + section.Name;
                        item.Price =
                            _unitOfWorkManager.Repository<Lesson>().FindBy(l => l.SectionId == section.Id).Sum(l => l.Price) *
                            double.Parse(sectionBuyDiscount.Value);
                        break;
                    case OrderType.Course:
                        var course = _unitOfWorkManager.Repository<Course>().GetSingle(item.OrderId);
                        item.Course = course;
                        item.Name = course.Name;
                        item.Price =
                            _unitOfWorkManager.Repository<Lesson>().FindBy(l => l.Section.CourseId == course.Id).Sum(l => l.Price) *
                            double.Parse(courseBuyDiscount.Value);
                        break;
                }

            CheckoutResult checkOutResult;

            var totalPrice = items.Sum(i => i.Price);
            if(totalPrice > student.Balance) checkOutResult = CheckoutResult.InSufficentBalance;
            else
                try
                {
                    foreach(var item in items)
                        switch(item.OrderType)
                        {
                            case OrderType.Lesson:
                                _unitOfWorkManager.Repository<StudentOrder>().Add(new StudentOrder
                                {
                                    DateTime = DateTime.Now,
                                    Fee = -item.Price,
                                    OrderId = item.OrderId,
                                    OrderType = OrderType.Lesson,
                                    StudentId = student.Id
                                });
                                _unitOfWorkManager.Repository<StudentLesson>().Add(new StudentLesson
                                {
                                    StudentId = student.Id,
                                    StartDate = DateTime.Now,
                                    LessonId = item.OrderId
                                });
                                break;
                            case OrderType.Section:
                                _unitOfWorkManager.Repository<StudentOrder>().Add(new StudentOrder
                                {
                                    DateTime = DateTime.Now,
                                    Fee = -item.Price,
                                    OrderId = item.OrderId,
                                    OrderType = OrderType.Section,
                                    StudentId = student.Id
                                });
                                var sectionLessons =
                                    _unitOfWorkManager.Repository<Lesson>().FindBy(l => l.SectionId == item.OrderId).ToList();
                                foreach(var lesson in sectionLessons)
                                    _unitOfWorkManager.Repository<StudentLesson>().Add(new StudentLesson
                                    {
                                        StudentId = student.Id,
                                        StartDate = DateTime.Now,
                                        LessonId = lesson.Id
                                    });
                                break;
                            case OrderType.Course:
                                _unitOfWorkManager.Repository<StudentOrder>().Add(new StudentOrder
                                {
                                    DateTime = DateTime.Now,
                                    Fee = -item.Price,
                                    OrderId = item.OrderId,
                                    OrderType = OrderType.Course,
                                    StudentId = student.Id
                                });
                                var courseLessons =
                                    _unitOfWorkManager.Repository<Lesson>().FindBy(l => l.Section.CourseId == item.OrderId).ToList
                                        ();
                                foreach(var lesson in courseLessons)
                                    _unitOfWorkManager.Repository<StudentLesson>().Add(new StudentLesson
                                    {
                                        StudentId = student.Id,
                                        StartDate = DateTime.Now,
                                        LessonId = lesson.Id
                                    });
                                break;
                        }

                    student.Balance -= totalPrice;
                    _unitOfWorkManager.Repository<ShoppingCard>().DeleteWhere(sc => true);
                    _unitOfWorkManager.Repository<Student>().Update(student);
                    _unitOfWorkManager.Commit();
                    checkOutResult = CheckoutResult.Success;
                }
                catch(Exception)
                {
                    checkOutResult = CheckoutResult.Failure;
                }

            return RedirectToAction("ShoppingCard", new { result = checkOutResult.ToString() });
        }

        public IActionResult Pay()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(PayViewModel model)
        {
            double amount;
            if(!double.TryParse(model.Amount, out amount))
            {
                ModelState.AddModelError("InvalidAmount", "Məbləğ düzgün daxil edilməyib. Düzgün format 00,00");
                return View(model);
            }

            if(amount <= 0)
            {
                ModelState.AddModelError("InvalidAmount", "Məbləğ 0 ola bilməz.");
                return View(model);
            }

            var paymentItem = new HesabAzPaymentItem
            {
                MerchantName = HesabAzProperties.MerchantName,
                Amount = (int)(amount * 100),
                CardType = model.CardType,
                Lang = "lv",
                Description = "cledacademy " + amount + " AZN payment"
            };
            paymentItem.HashCode =
                HesabAzProperties.GetMd5HashCode(HesabAzProperties.AuthKey + HesabAzProperties.MerchantName + model.CardType +
                                                 paymentItem.Amount + paymentItem.Description);

            var jsonValues = JsonConvert.SerializeObject(paymentItem,
                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            var request = (HttpWebRequest)WebRequest.Create(HesabAzProperties.RequestToServerUrlGetPaymentKey);
            request.ContentType = "application/json; charset=utf-8";
            request.Method = "POST";
            request.Accept = "application/json";
            using(var streamWriter = new StreamWriter(await request.GetRequestStreamAsync()))
            {
                await streamWriter.WriteAsync(jsonValues);
                await streamWriter.FlushAsync();
            }

            string responseData;
            var response = (HttpWebResponse)await request.GetResponseAsync();

            using(var reader = new StreamReader(response.GetResponseStream()))
            {
                responseData = await reader.ReadToEndAsync();
            }

            var getPaymentKeyResponse =
                (HesabAzGetPaymentKeyResponse)JsonConvert.DeserializeObject(responseData, typeof(HesabAzGetPaymentKeyResponse));

            if(getPaymentKeyResponse.Status.Code != 1)
                throw new Exception("Error while getting paymentKey, code=" + getPaymentKeyResponse.Status.Code + ", message=" +
                                    getPaymentKeyResponse.Status.Code);

            var student = this.GetStudent(_userManager, _unitOfWorkManager.Repository<Student>());

            _unitOfWorkManager.Repository<AccountTopUp>().Add(new AccountTopUp
            {
                StudentId = student.Id,
                Amount = amount,
                DateTime = DateTime.Now,
                PaymetOrigin = PaymentOrigin.HesabAz,
                PaymentKey = getPaymentKeyResponse.PaymentKey
            });
            _unitOfWorkManager.Commit();

            return Redirect(HesabAzProperties.RequestToServerUrlPayPage + getPaymentKeyResponse.PaymentKey);
        }

        public async Task<IActionResult> PaySuccess()
        {
            var paymentKey = Request.Query["payment_key"].ToString();

            if(string.IsNullOrEmpty(paymentKey)) //invalid key
                return NotFound();

            var student = this.GetStudent(_userManager, _unitOfWorkManager.Repository<Student>());
            var accountTopUp =
                _unitOfWorkManager.Repository<AccountTopUp>().GetSingle(
                    at => at.StudentId == student.Id && at.PaymentKey == paymentKey);

            if(accountTopUp == null) //if payment key is wrong or isn't belong to this user
                return NotFound();

            int statusCode;
            if(accountTopUp.StatusCode != null) //already updated the status
            {
                statusCode = int.Parse(accountTopUp.StatusCode.Split('-')[0]);
                return statusCode != 1 ? View("PayError", statusCode) : View();
            }

            statusCode = await GetPaymentResult(paymentKey);

            return statusCode != 1 ? View("PayError", statusCode) : View();
        }

        public async Task<IActionResult> PayError()
        {
            var paymentKey = Request.Query["payment_key"].ToString();

            if(string.IsNullOrEmpty(paymentKey)) //invalid key
                return NotFound();

            var student = this.GetStudent(_userManager, _unitOfWorkManager.Repository<Student>());
            var accountTopUp =
                _unitOfWorkManager.Repository<AccountTopUp>().GetSingle(
                    at => at.StudentId == student.Id && at.PaymentKey == paymentKey);

            if(accountTopUp == null) //if payment key is wrong or isn't belong to this user
                return NotFound();

            int statusCode;
            if(accountTopUp.StatusCode != null) //already updated the status
            {
                statusCode = int.Parse(accountTopUp.StatusCode.Split('-')[0]);
                return statusCode == 1 ? View("PaySuccess") : View(statusCode);
            }

            statusCode = await GetPaymentResult(paymentKey);

            return statusCode == 1 ? View("PaySuccess") : View(statusCode);
        }

        private async Task<int> GetPaymentResult(string paymentKey)
        {
            var request =
                (HttpWebRequest)
                WebRequest.Create(HesabAzProperties.RequestToServerUrlGetPaymentResult + "?payment_key=" + paymentKey +
                                  "&hash_code=" + HesabAzProperties.GetMd5HashCode(HesabAzProperties.AuthKey + paymentKey));
            request.ContentType = "application/json; charset=utf-8";
            request.Method = "POST";
            request.Accept = "application/json";
            var response = (HttpWebResponse)await request.GetResponseAsync();
            string responseData;
            using(var reader = new StreamReader(response.GetResponseStream()))
            {
                responseData = await reader.ReadToEndAsync();
            }

            var paymentResult =
                (HesabAzGetPaymentResult)JsonConvert.DeserializeObject(responseData, typeof(HesabAzGetPaymentResult));
            var accountTopUp = _unitOfWorkManager.Repository<AccountTopUp>().GetSingle(at => at.PaymentKey == paymentKey);

            if(string.IsNullOrEmpty(accountTopUp.StatusCode))
            {
                var amount = (double)paymentResult.Amount / 100;
                accountTopUp.Amount = amount;
                accountTopUp.CardNumber = paymentResult.CardNumber;

                var status = paymentResult.Status.Code + "-" + paymentResult.Status.Message;

                if(status.Length > 100) status = status.Remove(100);

                accountTopUp.StatusCode = status;
                accountTopUp.DateTime = DateTime.Parse(paymentResult.PaymentDate);
                accountTopUp.Rrn = paymentResult.Rrn;
                _unitOfWorkManager.Repository<AccountTopUp>().Update(accountTopUp);

                if(paymentResult.Status.Code == 1) //if success payment
                {
                    var student = _unitOfWorkManager.Repository<Student>().GetSingle(s => s.Id == accountTopUp.StudentId,
                        s => s.Person);
                    var bonusAmount = BonusCalculator.GetBonusAmount(amount);

                    var initialAmount = amount;

                    amount += bonusAmount;

                    if(bonusAmount > 0) // if gets bonus
                    {
                        MakeBonusPayment(student.Id, bonusAmount, accountTopUp.PaymentKey, PaymentOrigin.CledAcademyPaymentBonus);
                        try
                        {
                            var user = await _userManager.GetUserAsync(User);
                            var bonusForEachPayment = EmailTemplates.BonusForPayment;
                            await _emailSender.SendEmailAsync(user.Email, bonusForEachPayment.Title, bonusForEachPayment.SubTitle,
                                string.Format(bonusForEachPayment.Body, initialAmount, bonusAmount));

                            _logger.LogInformation(3, "Payment bonus email successfully sent to " + user.Email);
                        }
                        catch(Exception exc)
                        {
                            _logger.LogError(3, "Error occured while sending bonus email. Error message is: " + exc.Message);
                        }
                    }

                    student.Balance += amount;
                    _unitOfWorkManager.Repository<Student>().Update(student);
                }

                _unitOfWorkManager.Commit();
            }

            return paymentResult.Status.Code;
        }

        private void MakeBonusPayment(int studentId, double bonusAmount, string paymentKey, PaymentOrigin paymentOrigin)
        {
            var bonusAccountTopUp = new AccountTopUp
            {
                StudentId = studentId,
                Amount = bonusAmount,
                DateTime = DateTime.Now,
                PaymentKey = paymentKey,
                PaymetOrigin = paymentOrigin,
                StatusCode = "1-success",
                Rrn = DateTime.Now.ToString("yyyyMMddHHmmssfff"), //dummy rrn
            };
            _unitOfWorkManager.Repository<AccountTopUp>().Add(bonusAccountTopUp);
        }

        //
        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string userId, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            ViewBag.Days = this.GetDays();
            ViewBag.Months = this.GetMonths();
            ViewBag.Years = this.GetYears();

            ViewBag.UserId = userId;

            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string userId, string requestOrigin,
                                                  string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if(ModelState.IsValid)
            {
                var person = new Person
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Gender = (Gender)model.Gender,
                    Student = new Student { Phone = model.Phone, DateOfBirth = new DateTime(model.Year, model.Month, model.Day) }
                };
                var user = new ApplicationUser { UserName = model.Username, Email = model.Email, Person = person };
                var result = await _userManager.CreateAsync(user, model.Password);
                if(result.Succeeded)
                {
                    #region Send Welcome Email

                    try
                    {
                        var suggestUrl = Url.Action("Register", "Account", new { userId = user.Id }, HttpContext.Request.Scheme);
                        var registerAlert = EmailTemplates.Welcome;
                        await _emailSender.SendEmailAsync(model.Email, registerAlert.Title, registerAlert.SubTitle,
                            string.Format(registerAlert.Body, suggestUrl));

                        _logger.LogInformation(3, "Welcome email successfully sent to " + model.Email);
                    }
                    catch(Exception exc)
                    {
                        _logger.LogError(3, "Error occured while sending welcome email. Error message is: " + exc.Message);
                    }

                    #endregion

                    #region Make 1 AZN bonus payment for suggested student and send info email

                    var bonusMadeUser = await _userManager.FindByIdAsync(userId);
                    if(bonusMadeUser != null) //we have such user
                    {
                        var bonusMadeStudent =
                            _unitOfWorkManager.Repository<Student>().GetSingle(s => s.PersonId == bonusMadeUser.PersonId);
                        bonusMadeStudent.Balance += 1;
                        MakeBonusPayment(bonusMadeStudent.Id, 1, user.Id, PaymentOrigin.CledAcademyFriendBonus);
                        _unitOfWorkManager.Commit();

                        try
                        {
                            var bonusForFriendRegistration = EmailTemplates.BonusForFriendRegistration;
                            await _emailSender.SendEmailAsync(bonusMadeUser.Email, bonusForFriendRegistration.Title,
                                bonusForFriendRegistration.SubTitle,
                                string.Format(bonusForFriendRegistration.Body, model.FirstName, model.LastName));

                            _logger.LogInformation(3, "Bonus email successfully sent to " + bonusMadeUser.Email);
                        }
                        catch(Exception exc)
                        {
                            _logger.LogError(3, "Error occured while sending bonus email. Error message is: " + exc.Message);
                        }
                    }

                    #endregion

                    if(requestOrigin == "Facebook")
                    {
                        var info = await _signInManager.GetExternalLoginInfoAsync();
                        if(info != null)
                        {
                            result = await _userManager.AddLoginAsync(user, info);
                            if(result.Succeeded)
                            {
                                await _signInManager.SignInAsync(user, false);
                                _logger.LogInformation(6, "User created an account using {Name} provider.", info.LoginProvider);
                                return RedirectToLocal(returnUrl);
                            }
                        }
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, false);
                        _logger.LogInformation(3, "User created a new account with password.");
                        return RedirectToLocal(returnUrl);
                    }
                }

                #region region Identity Error translating

                var errors = result.Errors;
                foreach(var identityError in errors)
                {
                    var description = StaticResources.GetIdentityMessage(identityError.Code, model.Username, model.Email);
                    identityError.Description = description ?? identityError.Description;
                }

                AddErrors(result);

                #endregion
            }

            ViewBag.RequestOrigin = requestOrigin;
            ViewBag.UserId = userId;

            ViewBag.Days = this.GetDays();
            ViewBag.Months = this.GetMonths();
            ViewBag.Years = this.GetYears();

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if(ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);
                if(result.Succeeded)
                {
                    _logger.LogInformation(1, "User logged in.");
                    return RedirectToLocal(returnUrl);
                }

                if(model.Username.Contains("@")) //maybe email
                {
                    var user = await _userManager.FindByEmailAsync(model.Username);
                    if(user != null)
                    {
                        result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                        if(result.Succeeded)
                        {
                            _logger.LogInformation(1, "User logged in.");
                            return RedirectToLocal(returnUrl);
                        }
                    }
                }

                ModelState.AddModelError(string.Empty, "Uğursuz giriş cəhdi.");
                return View(model);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Edit
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            var student = _unitOfWorkManager.Repository<Student>().GetSingle(s => s.PersonId == user.PersonId, s => s.Person);
            var accountEditViewModel = new AccountEditViewModel
            {
                FirstName = student.Person.FirstName,
                LastName = student.Person.LastName,
                Phone = student.Phone,
                Day = student.DateOfBirth.Day,
                Month = student.DateOfBirth.Month,
                Year = student.DateOfBirth.Year,
                Gender = (byte)student.Person.Gender,
                Username = user.UserName,
                Email = user.Email
            };

            ViewBag.Days = this.GetDays();
            ViewBag.Months = this.GetMonths();
            ViewBag.Years = this.GetYears();

            return View(accountEditViewModel);
        }

        //
        // POST: /Account/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AccountEditViewModel model, IFormFile studentImage)
        {
            if(ModelState.IsValid && (studentImage?.Length ?? 0) <= 1 * 1024 * 1024)
            {
                var user = await _userManager.GetUserAsync(User);
                if(await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    user.UserName = model.Username;
                    user.Email = model.Email;

                    var student = _unitOfWorkManager.Repository<Student>().GetSingle(s => s.PersonId == user.PersonId,
                        s => s.Person);

                    student.Phone = model.Phone;
                    student.DateOfBirth = new DateTime(model.Year, model.Month, model.Day);
                    student.Person.FirstName = model.FirstName;
                    student.Person.LastName = model.LastName;
                    student.Person.Gender = (Gender)model.Gender;

                    if(studentImage?.Length > 0)
                    {
                        byte[] buffer;
                        using(var stream = studentImage.OpenReadStream())
                        {
                            buffer = new byte[stream.Length];
                            await stream.ReadAsync(buffer, 0, (int)stream.Length);
                        }

                        student.Person.ImageContentType = studentImage.ContentType;
                        student.Person.Image = buffer;
                    }

                    var result = await _userManager.UpdateAsync(user);

                    if(result.Succeeded)
                    {
                        _unitOfWorkManager.Repository<Student>().Update(student);
                        _unitOfWorkManager.Commit();
                        return RedirectToAction("Index", "Home");
                    }

                    AddErrors(result);
                }
                else
                {
                    ModelState.AddModelError("Password", "Daxil etdiyiniz şifrə yalnışdır");
                }
            }
            else if(studentImage?.Length > 1 * 1024 * 1024)
            {
                ViewBag.FileSizeInvalid = "Profil şəklinin həcmi 1 Mb -dan az olmalıdır";
            }

            ViewBag.Days = this.GetDays();
            ViewBag.Months = this.GetMonths();
            ViewBag.Years = this.GetYears();

            return View("Edit", model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation(4, "User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Suggest()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.SuggestUrl = Url.Action("Register", "Account", new { userId = user.Id }, HttpContext.Request.Scheme);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Suggest(SuggestViewModel model, string suggestUrl)
        {
            if(ModelState.IsValid)
            {
                var requstedUser = await _userManager.FindByEmailAsync(model.Email);
                if(requstedUser == null)
                {
                    var student = this.GetStudent(_userManager, _unitOfWorkManager.Repository<Student>());
                    var suggestToFriend = EmailTemplates.SuggestToFriend;

                    await _emailSender.SendEmailAsync(model.Email, suggestToFriend.Title,
                        string.Format(suggestToFriend.SubTitle, student.Person.FirstName, student.Person.LastName),
                        string.Format(suggestToFriend.Body, suggestUrl));
                }
                else
                {
                    ViewBag.SuggestResult = "alreadyRegistered";
                }
            }
            else
            {
                ViewBag.SuggestResult = "invalidEmail";
            }

            return View("SuggestSuccess");
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLogin(string userId, string provider, string returnUrl = null)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if(info != null) return RedirectToAction("ExternalLoginCallback");

            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { userId, ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        //
        // GET: /Account/ExternalLoginCallback
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string userId, string returnUrl = null, string remoteError = null)
        {
            if(remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View(nameof(Login));
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if(info == null) return RedirectToAction(nameof(Login));

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
            if(result.Succeeded)
            {
                _logger.LogInformation(5, "User logged in with {Name} provider.", info.LoginProvider);
                return RedirectToLocal(returnUrl);
            }

            // If the user does not have an account, then ask the user to create an account.
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["LoginProvider"] = info.LoginProvider;
            ViewBag.UserId = userId;

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var givenName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
            var surName = info.Principal.FindFirstValue(ClaimTypes.Surname);
            var dateOfBirth = info.Principal.FindFirstValue(ClaimTypes.DateOfBirth);
            var gender = info.Principal.FindFirstValue(ClaimTypes.Gender);
            var homePhone = info.Principal.FindFirstValue(ClaimTypes.HomePhone);
            var mobilePhone = info.Principal.FindFirstValue(ClaimTypes.MobilePhone);

            ViewBag.RequestOrigin = "Facebook";

            var registerViewModel = new RegisterViewModel
            {
                Username = email,
                Email = email,
                FirstName = givenName,
                LastName = surName,
                Phone = !string.IsNullOrEmpty(homePhone) ? homePhone : mobilePhone,
                Gender = (byte)(gender != null ? (gender == "male" ? 1 : 2) : 0)
            };

            if(dateOfBirth != null)
            {
                var dt = DateTime.Parse(dateOfBirth);
                registerViewModel.Day = dt.Day;
                registerViewModel.Month = dt.Month;
                registerViewModel.Year = dt.Year;
            }

            ViewBag.Days = this.GetDays();
            ViewBag.Months = this.GetMonths();
            ViewBag.Years = this.GetYears();

            return View("Register", registerViewModel);
        }

        //
        // GET: /Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if(user != null)
                {
                    //For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                    // Send an email with this link
                    var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code },
                        HttpContext.Request.Scheme);
                    var forgotPassword = EmailTemplates.ForgotPassword;
                    await _emailSender.SendEmailAsync(model.Email, forgotPassword.Title, forgotPassword.SubTitle,
                        string.Format(forgotPassword.Title, callbackUrl));
                    return View("ForgotPasswordConfirmation");
                }

                ModelState.AddModelError("Email", "Daxil edilən email ünvanına uyğun istifadəçi tapılmadı");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if(!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if(user == null) return RedirectToAction(nameof(ResetPasswordConfirmation), "Account");

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if(result.Succeeded) return RedirectToAction(nameof(ResetPasswordConfirmation), "Account");

            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach(var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if(Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        #endregion
    }
}