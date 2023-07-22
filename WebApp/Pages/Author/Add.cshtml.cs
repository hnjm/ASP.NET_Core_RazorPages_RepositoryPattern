using Abc.AuthorLibrary;
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
    public class AddModel : PageModel
    {
        private readonly IAuthorService authorService;
        private readonly IUnitOfWork unitOfWork;

        public AddModel(IUnitOfWork unitOfWork, IAuthorService authorService)
        {
            this.authorService = authorService;
            this.unitOfWork = unitOfWork;
        }

        [BindProperty]
        public AuthorModel Author { get; set; }

        public void OnGet()
        {
            
        }

        public async Task<IActionResult> OnPost(AuthorModel author)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            using (var trans = await this.unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    List<ABC.Models.Book> _books = new List<ABC.Models.Book>();

                    if (author.Books != null)
                    {
                        _books.AddRange(author.Books.Select(x => new ABC.Models.Book { AuthorId = author.Id, Title = x.Title, Description = x.Description }));
                    }

                    await this.authorService.AddAuthor(new ABC.Models.Author { Id = author.Id, Name = author.Name, Books = _books });
                   
                    // await this.unitOfWork.CommitAsync();
                    await this.unitOfWork.CommitTransactionAsync(trans);
                    
                    return RedirectToPage("Index");
                }
                catch (Exception)
                {
                    //this.unitOfWork.Rollback();
                    await this.unitOfWork.RollbackTransactionAsync(trans);

                    throw;
                }
                finally
                {
                   await trans.DisposeAsync();
                }
            }

        }
    }
}
