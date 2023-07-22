using Abc.BusinessService;
using Abc.UnitOfWorkLibrary;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace WebApp.Pages.Book
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
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
