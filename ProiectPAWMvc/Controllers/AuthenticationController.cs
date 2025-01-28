using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using ProiectPAW.ContextModels;
using ProiectPAW.Models;
using System.Security.Claims;

namespace ProiectPAWMvc.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly ProdusContext _produsContext;

        public AuthenticationController(ProdusContext produsContext)
        {
_produsContext = produsContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(Utilizator user)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(user.Nume))
                    {
                        ModelState.AddModelError(string.Empty, "The username is empty!");

                    }
                    else if (string.IsNullOrWhiteSpace(user.Parola))
                    {
                        ModelState.AddModelError(string.Empty, "The password field is empty!");

                    }
                    else if (string.IsNullOrWhiteSpace(user.Email))
                    {
                        ModelState.AddModelError(string.Empty, "The email field is empty!");

                    }
                    else if (_produsContext.Utilizator.Where(us => us.Email!.ToLower() == user.Email.ToLower()).Count() > 0)
                    {
                        ModelState.AddModelError(string.Empty, "An account linked to this email already exists!");
                    }
                    else
                    {

                        try
                        {
                            _produsContext.Utilizator.Add(user);
                            _produsContext.SaveChanges();
                            return RedirectToAction("Index","Home");


                        }
                        catch (Exception ex) { ModelState.AddModelError(string.Empty, "Error creating account: " + ex.Message);
                        }
                    }

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }

            }
            return View(user);


        }

        [HttpGet]
        public IActionResult Register()
        {

            return View(); // ne afiseaza pagina de register cand accesam link de register
        }


        [HttpPost]
        public async Task<IActionResult> Login(Utilizator model)
        {
            ModelState.Remove("Email"); // sa nu mai fie nevoei sa adaugam campul email la logare si in codul html
            // modelstate cere sa avem campuri pentru toate atributele clasei de aceea am eliminat email

            if (ModelState.IsValid)
            {
                Console.WriteLine("VALID");
                try
                {
                    if (string.IsNullOrWhiteSpace(model.Nume))
                        ModelState.AddModelError(string.Empty, "The username is empty!");
                    else if (string.IsNullOrWhiteSpace(model.Parola))
                        ModelState.AddModelError(string.Empty, "The password is empty!");

                    Utilizator? user = _produsContext.Utilizator.Where(user =>
                    user.Nume!.ToLower() == model.Nume!.ToLower()  && user.Parola == model.Parola).FirstOrDefault();

                    if (user != null)
                    {
                        List<Claim> claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Nume!),
                        new Claim("IdUserLogat", user.ID.ToString()), 


                        new Claim(ClaimTypes.Role, user.TipUtilizator.ToString())
                    };
                        var claimIdentity = new ClaimsIdentity(claims, "AuthenticationCookie");

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimIdentity));
                        return RedirectToAction("Index", "Home");
                    }
                    else
                        ModelState.AddModelError(string.Empty, "Invalid username or password!");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Error logging in: " + ex.Message);
                }

            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Login()
        {
    return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            if (User.Identity.IsAuthenticated == false)
                return RedirectToAction("Index", "Home");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); //stergem cookies
            return RedirectToAction("Login");
        }
    }


}    
