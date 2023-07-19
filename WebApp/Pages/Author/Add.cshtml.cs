using Abc.AuthorLibrary;
using Abc.BusinessService;
using Abc.UnitOfWorkLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Models;

namespace WebApp.Pages.Author
{
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

            try
            {
                List<ABC.Models.Book> _books = new List<ABC.Models.Book>();

                if (author.Books != null)
                {
                    _books.AddRange(author.Books.Select(x => new ABC.Models.Book { AuthorId = author.Id, Title = x.Title, Description = x.Description }));
                }

                await this.authorService.AddAuthor(new ABC.Models.Author { Id = author.Id, Name = author.Name, Books = _books });
                await this.unitOfWork.CommitAsync();
                return RedirectToPage("Index");
            }
            catch(Exception)
            {
                this.unitOfWork.Rollback();
                throw;
            }

        }
    }
}
