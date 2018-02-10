using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CIMOB_IPS.Models;
using System.Data.SqlClient;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace CIMOB_IPS.Controllers
{
    /// <summary>
    /// Controlador para as acções do perfil de um utilizador.
    /// Contém métodos para visualização e edição de um perfil de utilizador.
    /// </summary>
    public class ProfileController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public ProfileController(IHostingEnvironment HostingEnvironment)
        {
            _hostingEnvironment = HostingEnvironment;
        }

        /// <summary>
        /// Retorna a chave primária associada à conta do utilizador autenticado no momento.
        /// </summary>
        /// <returns>Chave primária associada à conta do utilizador autenticado no momento</returns>
        /// <remarks></remarks>
        public int GetCurrentUserID()
        {
            return int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
        }

        /// <summary>
        /// Retorna o modelo de um conta pela chave primária da mesma passada como argumento.
        /// </summary>
        /// <param name="intId">Chave primária da conta</param>
        /// <returns>Modelo de um conta pela chave primária da mesma passada como argumento.</returns>
        public ProfileViewModel GetAccountModelByID(int intId)
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                ProfileViewModel profileViewModel = new ProfileViewModel { };

                profileViewModel.Student = context.Student
                    .Include(a => a.IdAccountNavigation)
                    .Include(a => a.IdNationalityNavigation)
                    .Include(a => a.IdCourseNavigation)
                    .Include(a => a.IdAddressNavigation)
                    .FirstOrDefault(s => s.IdAccount == intId);

                profileViewModel.Technician = context.Technician
                    .Include(a => a.IdAccountNavigation)
                    .FirstOrDefault(s => s.IdAccount == intId);

                if (profileViewModel.Student != null)
                {
                    profileViewModel.AccountType = EnumAccountType.STUDENT;

                    return profileViewModel;
                }
                else if (profileViewModel.Technician != null)
                {
                    profileViewModel.AccountType = EnumAccountType.TECHNICIAN;

                    return profileViewModel;
                }

                return null;
            }
        }

        /// <summary>
        /// Retorna a vista com as informações do perfil do utilizador autenticado.
        /// </summary>
        /// <returns>Vista com as informações do perfil do utilizador autenticado.</returns>
        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var accountViewModel = GetAccountModelByID(GetCurrentUserID());

            ViewData["edit-profile-display"] = "block";


            return View(accountViewModel);
        }

        /// <summary>
        /// Retorna a vista com as informações do perfil de um utilizador que tem como chave primária o inteiro passado como argumento.
        /// </summary>
        /// <param name="id">Chave primária do utilizador</param>
        /// <returns>Vista com as informações do perfil de um utilizador que tem como chave primária o inteiro passado como argumento./returns>
        public IActionResult Get(int id)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var accountViewModel = GetAccountModelByID(id);
            ViewData["edit-profile-display"] = "none";

            if (accountViewModel == null)
                return RedirectToAction("Index", "Home");

            return View("Index", accountViewModel);
        }

        /// <summary>
        /// Retorna a vista com o formulário para edição do perfil do utilizador autenticado.
        /// </summary>
        /// <returns>Vista com o formulário para edição do perfil do utilizador autenticado.</returns>
        public IActionResult Edit()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var accountViewModel = GetAccountModelByID(GetCurrentUserID());

            if (accountViewModel.AccountType == EnumAccountType.STUDENT)
            {
                var postalCode = accountViewModel.Student.IdAddressNavigation.PostalCode;

                accountViewModel.PostalCode1 = postalCode.Substring(0, 4);
                accountViewModel.PostalCode2 = postalCode.Substring(5, 3);
            }

            return View(accountViewModel);
        }

        /// <summary>
        /// Atualiza o perfil de um estudante com novas informações.
        /// </summary>
        /// <param name="model">Modelo do perfil</param>
        /// <returns>View Profile/Index com a visualização do perfil atualizado</returns>
        /// <remarks></remarks>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfileStudent(ProfileViewModel model)
        {
            if (GetCurrentUserID() != model.Student.IdAccount)
                return BadRequest();
            try
            {
                using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
                {

                    Student newStudent = await context.Student
                    .Include(s => s.IdAddressNavigation)
                    .SingleOrDefaultAsync(s => s.IdAccount == model.Student.IdAccount);

                    if (newStudent == null)
                        return NotFound();

                    newStudent.Telephone = model.Student.Telephone;
                    newStudent.IdAddressNavigation.PostalCode = model.Student.IdAddressNavigation.PostalCode;
                    newStudent.Credits = model.Student.Credits;

                    Address studentAddress = await context.Address.SingleOrDefaultAsync(s => s.IdAddress == newStudent.IdAddress);
                    studentAddress.PostalCode = model.PostalCode1 + "-" + model.PostalCode2;
                    studentAddress.AddressDesc = model.Student.IdAddressNavigation.AddressDesc;
                    studentAddress.DoorNumber = model.Student.IdAddressNavigation.DoorNumber;
                    studentAddress.Floor = model.Student.IdAddressNavigation.Floor;


                    context.Update(studentAddress);
                    context.Update(newStudent);
                    await UploadAvatar(newStudent.IdAccount.ToString());
                    await context.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Atualiza o perfil de um técnico do CIMOB com novas informações.
        /// </summary>
        /// <param name="model">Modelo do perfil</param>
        /// <returns>View Profile/Index com a visualização do perfil atualizado</returns>
        /// <remarks></remarks>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfileTechnician(ProfileViewModel model)
        {
            if (GetCurrentUserID() != model.Technician.IdAccount)
                return BadRequest();

            try
            {
                using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
                {
                    Technician newTechnician = await context.Technician.SingleOrDefaultAsync(t => t.IdAccount == model.Technician.IdAccount);

                    if (newTechnician == null)
                        return NotFound();

                    newTechnician.Telephone = model.Technician.Telephone;

                    context.Update(newTechnician);

                    await UploadAvatar(model.Technician.IdAccount.ToString());
                    await context.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }


            return RedirectToAction("Index");
        }

        /// <summary>
        /// Insere no servidor um ficheiro que representa a imagem avatar de um utilizador.
        /// O ficheiro ficará com o caminho images/avatars/(chave primária da conta do utilizador).(extensão).
        /// Por defeito a imagem é images/avatars/user1.png .
        /// </summary>
        /// <param name="accountid">Chave primária da conta do utilizador para ser guardada como nome do ficheiro</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task UploadAvatar(string accountid)
        {
            if (Request.Form.Files.Count > 0)
            {
                var ImageFile = Request.Form.Files[0];
                if (ImageFile != null)
                {
                    if (ImageFile.FileName != "")
                    {
                        var extention = Path.GetExtension(ImageFile.FileName);

                        var uploadName = Path.Combine(_hostingEnvironment.WebRootPath, "images/avatars", accountid + extention);

                        using (var fileStream = new FileStream(uploadName, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(fileStream);
                            UpdateAvatarURL(accountid, extention);
                        }
                    }

                }
            }
        }


        /// <summary>
        /// Atualiza o caminho para a imagem do perfil do utilizador.
        /// </summary>
        /// <param name="account_id">Chave Primária da conta do utilizador</param>
        /// <param name="extention">Extensão do ficheiro</param>
        /// <remarks></remarks>
        private void UpdateAvatarURL(string account_id, string extention)
        {
            using (SqlConnection scnConnection = new SqlConnection(CIMOB_IPS_DBContext.ConnectionString))
            {
                scnConnection.Open();
                string strQuery = "UPDATE Account Set avatarURL = @AvatarURL WHERE id_account = @AccountID";

                SqlCommand scmCommand = new SqlCommand(strQuery, scnConnection);
                scmCommand.Parameters.AddWithValue("@AvatarURL", "/images/avatars/" + account_id + extention);
                scmCommand.Parameters.AddWithValue("@AccountID", account_id);

                scmCommand.ExecuteNonQuery();

            }
        }


        /// <summary>
        /// Visualização do perfil de um estudante.
        /// </summary>
        /// <param name="id">Chave primária do estudante</param>
        /// <returns>Partial view com as informações do perfil de um estudante</returns>
        [HttpGet]
        public IActionResult ViewStudentProfile(string id)
        {
            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                Student student = context.Student.Where(s => s.IdStudent == int.Parse(id))
                    .Include(s => s.IdAccountNavigation)
                    .Include(s => s.IdAddressNavigation)
                    .Include(s => s.IdCourseNavigation)
                    .Include(s => s.IdNationalityNavigation)
                    .SingleOrDefault();

                return PartialView("_ViewStudentProfile", student);
            }
        }

        /// <summary>
        /// Retorna o número de créditos do estudante autenticado.
        /// </summary>
        /// <param name="user">Estudante autenticado</param>
        /// <returns>Número de créditos do estudante autenticado.</returns>
        public async Task<int> GetCurrentStudentECTS(ClaimsPrincipal user)
        {
            var intCurrentId = int.Parse(user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);

            using (var context = new CIMOB_IPS_DBContext(new DbContextOptions<CIMOB_IPS_DBContext>()))
            {
                return await context.Student.Where(s => s.IdAccount == intCurrentId).Select(s => s.Credits).SingleOrDefaultAsync();
            }
        }

    }
}