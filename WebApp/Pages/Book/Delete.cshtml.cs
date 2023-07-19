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
            using (var trans = await this.unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    await this.bookService.DeleteBook(id);
                    await this.unitOfWork.CommitTransactionAsync(trans);
                    return RedirectToPage("index");
                }
                catch (Exception)
                {
                    await this.unitOfWork.RollbackTransactionAsync(trans);
                    return RedirectToPage("index");
                }
                finally
                {
                    await trans.DisposeAsync();
                }
            }
        }
    }
}
