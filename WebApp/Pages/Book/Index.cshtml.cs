using Abc.BusinessService;
using Abc.UnitOfWorkLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Models;

namespace WebApp.Pages.Book
{
    public class IndexModel : PageModel
    {
        private readonly IBookService _bookService;
        private readonly IUnitOfWork unitOfWork;

        public IndexModel(IUnitOfWork unitOfWork, IBookService bookService)
        {
            this.unitOfWork = unitOfWork;
            this._bookService = bookService;
        }

        [BindProperty]
        public IEnumerable<BookModel> Books { get; set; }

        public async Task OnGet()
        {
            var booksObj = await this._bookService.GetAllBooks();

            List<BookModel> models = new List<BookModel>();

            foreach (var item in booksObj)
            {
                var _book = new BookModel { Id = item.Id, 
                                        Title = item.Title,
                                        Description = item.Description, 
                                        AuthorName = item.Author.Name, 
                                        AuthorId = item.AuthorId 
                                    };
                models.Add(_book);
            }

            this.Books = models;
        }
    }
}
