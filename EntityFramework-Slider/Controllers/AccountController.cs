using EntityFramework_Slider.Models;
using EntityFramework_Slider.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EntityFramework_Slider.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;  

        private readonly SignInManager<AppUser> _signInManager;  
        public AccountController(UserManager<AppUser> userManager,
                                 SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            //userMeneger  -- usere role elave etmek, silmek, teze usercreate etmek, userlerin datalarina chta bilirik hamsi hazir bunun ichinden gelir
            //singinMeneger -- 

            //1ci user crate edirik

            AppUser newUser = new()  // oz yaratdigimiz modelden instans aliriq. bu modeli ona gore yaratmiwdiqki identitynin ichinde hazir olan propertilerden elave nese yazmaq isteyirikde yaza bilek deye. mes emal username ve s var bize fullname lazimdir elave olaraq, ona gore bu modeli yaradib ichine qoyuruq
            {
                UserName = model.Username,
                Email = model.Email,
                FullName = model.FullName, // bu oz yazdigimiz propertitydir. digerleri ise miras aldigimiza gore identityUser-den hazir gelir

                //passwordu burda beraber etmirik chunki create edende hawlanivb gonderilir

            };

            IdentityResult result = await _userManager.CreateAsync(newUser, model.Password);  // parolu create edende burdan gonderirikki hawlansin

            if (!result.Succeeded) //resultin ichinde errorlar var. result succes deyilse error chiartsin
            {
                foreach (var item in result.Errors)  // errorlar list formasindadir deye fora saliriq
                {
                    ModelState.AddModelError(string.Empty, item.Description); // errolarin descriptionlarini goster
                }

                return View(model);
            }

            //if e girmirse yeni herwey qaydasindadirsa singin etsin indexe qayitsin
            /*          await _signInManager.SignInAsync(newUser, false);*/ //appUser yeni hansi adam girirw edirse, isPersistent(remember me mentiginde, yeni yaddawda useri saxlamaq uchun), 3-cusu default token istifade edirikse gonderirikki bu adam giriw edib meselen. custom token etmirikse. yazmasaq bura null gelecek
                                                                            //login actionuna getsin deye signinManager i burada etmirik. sadece register olsun getsin login actionuna oradan signin olsun

            return RedirectToAction(nameof(Login));
        }


        [HttpGet]
        public IActionResult Login()
        {

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            AppUser user = await _userManager.FindByEmailAsync(model.EmailOrUsername); //databazadan AppUser tablesinden Email ne gore useri tapiriq.

            if (user == null)  //bazada bu emailde adam yoxdursa 
            {
                user = await _userManager.FindByNameAsync(model.EmailOrUsername);  //username ine gore tapiriq
            }

            if (user == null) // yuxarida hem email hem username ine gore yoxlayiriq her ikisi nulldirsa error chixardirriq
            {
                ModelState.AddModelError(string.Empty, "Email or password is wrong");

                return View(model);
            }

            //yuxaridaki wertlerde girmirse, yeni email veya username dogru yazilibsa 
            //burada parol yazib giriw edir
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false); //parolu yazib giriw etmek uchun bunu yaziriq
                                                                                                       //girish olacaq user, userin paswordu,false- rememberme var ya yox,false-logout tetbiq olunsun ya yox
                                                                                                       //eger parolda sehvdirse  giriw succeded olmur ve error mesage chixir
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Email or password is wrong");

                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();  // logout olmaq uchun 
            return RedirectToAction("Index", "Home");
        }
    }
}