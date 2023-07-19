using Abc.BusinessService;
using Abc.UnitOfWorkLibrary;
using ABC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Models;

namespace WebApp.Pages.Book
{
    public class EditModel : PageModel
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IBookService bookService;
        private readonly IAuthorService authorService;

        public EditModel(IUnitOfWork unitOfWork, IBookService bookService, IAuthorService authorService)
        {
            this.unitOfWork = unitOfWork;
            this.bookService = bookService;
            this.authorService = authorService;
        }

        [BindProperty]
        public BookModel Book { get; set; }

        [BindProperty]
        public IEnumerable<SelectListItem> Authors { get; set; }

        public async Task<IActionResult> OnGet(int id)
        {
            var listData = await this.authorService.GetAllAuthors();
            this.Authors = listData.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();

            var data = (await this.bookService.GetBooks(x => x.Id == id)).FirstOrDefault();

            if (data == null)
                return RedirectToPage("index");

            this.Book = new BookModel { Id = data.Id, Title =data.Title, Description = data.Description, AuthorId = data.AuthorId, AuthorName = data.Author.Name };

            return Page();
        }

        public async Task<IActionResult> OnPost(int id, BookModel book)
        {
            if (!ModelState.IsValid)
                return Page();

            using (var transaction = await this.unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    await this.bookService.UpdateBook(new ABC.Models.Book { Id = id, Title = book.Title, Description = book.Description, AuthorId = book.AuthorId });
                    await this.unitOfWork.CommitTransactionAsync(transaction);
                    return RedirectToPage("index");
                }
                catch (Exception)
                {
                    await this.unitOfWork.RollbackTransactionAsync(transaction);
                    throw;
                }
                finally
                {
                    await transaction.DisposeAsync();
                }
            }
        }
    }
}
