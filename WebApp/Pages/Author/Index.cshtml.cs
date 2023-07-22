using Abc.BusinessService;
using Abc.UnitOfWorkLibrary;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Server.HttpSys;
using WebApp.Models;

namespace WebApp.Pages.Author
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles ="User")]
    public class IndexModel : PageModel
    {
        private readonly IAuthorService _authorService;
        private readonly IUnitOfWork _unitOfWork;

        public IndexModel(IUnitOfWork unitOfWork, IAuthorService authService)
        {
            this._unitOfWork = unitOfWork;
            this._authorService = authService;
        }

        [BindProperty]
        public bool IsNotified { set; get; }

        [BindProperty]
        public IEnumerable<AuthorModel> Authors { get; set; }      

        public async Task OnGet()
        {
            await LoadData();
        }

        public async Task<IActionResult> OnPostDeleteAuthorHandler(int id)
        {
            using (var trans = await this._unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    await this._authorService.DeleteAuthor(id);
                    await this._unitOfWork.CommitTransactionAsync(trans);
                }
                catch (Exception)
                {
                    await this._unitOfWork.RollbackTransactionAsync(trans);
                }
                finally
                {
                    await trans.DisposeAsync();
                }
            }

            return RedirectToPage("index");
        }

        public async Task OnPostNotifyAuthorHandler(int id)
        {
            this.IsNotified = true;

            await LoadData();
        }

        public async Task OnPostDisableNotify()
        {
            this.IsNotified = false;
            
            await LoadData();
        }

        private async Task LoadData()
        {
            var listData = await this._authorService.GetAllAuthors();
            List<AuthorModel> authors = new List<AuthorModel>();

            foreach (var item in listData)
            {
                var model = new AuthorModel { Id = item.Id, Name = item.Name };
                List<BookModel> books = new List<BookModel>();

                if (item.Books != null)
                {
                    foreach (var book in item.Books)
                    {
                        var _book = new BookModel
                        {
                            Id = book.Id,
                            Title = book.Title,
                            Description = book.Description,
                            AuthorName = item.Name
                        };

                        books.Add(_book);
                    }
                }

                model.Books = books;
                authors.Add(model);
            }

            this.Authors = authors;
        }

    }
}
