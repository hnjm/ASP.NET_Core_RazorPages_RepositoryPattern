using Abc.BusinessService;
using Abc.UnitOfWorkLibrary;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using WebApp.Models;

namespace WebApp.Pages.Author
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
    public class EditModel : PageModel
    {
        private readonly IAuthorService authorService;
        private readonly IUnitOfWork unitOfWork;
        private readonly IWebHostEnvironment environment;

        public EditModel(IUnitOfWork unitOfWork, IAuthorService authorService, IWebHostEnvironment webHostEnvironment)
        {
            this.authorService = authorService;
            this.unitOfWork = unitOfWork;
            this.environment = webHostEnvironment;
        }


        [BindProperty]
        public AuthorModel Author { get; set; }

        [BindProperty]
        public string PhotoPath { get; set; }

        [BindProperty]
        public IFormFile Photo { get; set; }

        public async Task OnGet(int id)
        {
            var data = (await this.authorService.GetAuthors(x => x.Id == id)).FirstOrDefault();

            this.Author = new AuthorModel { Id = data.Id, Name = data.Name, PhotoName = data.PhotoName };
           
            this.PhotoPath = "/images/" + (string.IsNullOrEmpty(data.PhotoName) ? "noimage.jpg" : data.PhotoName);
        }

        public async Task<IActionResult> OnPostBackToIndex()
        {
            return RedirectToPage("index");
        }

        public async Task<IActionResult> OnPost(int id, AuthorModel author)
        {
            if (!ModelState.IsValid)
                return Page();

            using (var trans = await this.unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var photo_name = processUploadFile();
                    await this.authorService.UpdateAuthor(new ABC.Models.Author { Id = id, Name = author.Name, PhotoName = photo_name });
                    await this.unitOfWork.CommitTransactionAsync(trans);
                    return RedirectToPage("index");
                }
                catch (Exception)
                {
                    await this.unitOfWork.RollbackTransactionAsync(trans);
                    throw;
                }
                finally
                {
                    await trans.DisposeAsync();
                }
            }
         }

        private string processUploadFile()
        {
            string filename = "noimage.jpg";

            if (Photo != null)
            {
                var folder = Path.Combine(environment.WebRootPath, "images");
                var fileGuid = Guid.NewGuid().ToString() + "_" + Photo.FileName;
                var fullpath = Path.Combine(folder, fileGuid);

                using(FileStream fs = new FileStream(fullpath, FileMode.Create))
                {
                    Photo.CopyTo(fs);
                }

                filename = fileGuid;
            }

            return filename;
        }
    }
}
