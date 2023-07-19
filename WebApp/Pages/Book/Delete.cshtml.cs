using Abc.BusinessService;
using Abc.UnitOfWorkLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Book
{
    public class DeleteModel : PageModel
    {
        private readonly IBookService bookService;
        private readonly IUnitOfWork unitOfWork;

        public DeleteModel(IBookService bookService, IUnitOfWork unitOfWork)
        {
            this.bookService = bookService;
            this.unitOfWork = unitOfWork;   
        }

        public async Task<IActionResult> OnGet(int id)
        {
            try
            {
                await this.bookService.DeleteBook(id);
                await this.unitOfWork.CommitAsync();
                return RedirectToPage("index");
            }
            catch (Exception)
            {
                this.unitOfWork.Rollback();
                return RedirectToPage("index");
            }
        }
    }
}
