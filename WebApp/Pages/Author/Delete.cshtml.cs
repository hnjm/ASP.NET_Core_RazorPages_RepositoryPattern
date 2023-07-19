using Abc.AuthorLibrary;
using Abc.BusinessService;
using Abc.UnitOfWorkLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Author
{
    public class DeleteModel : PageModel
    {
        private readonly IAuthorService authorService;
        private readonly IUnitOfWork unitOfWork;

        public DeleteModel(IUnitOfWork unitOfWork, IAuthorService authorService)
        {
            this.authorService = authorService;
            this.unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> OnGet(int id)
        {
            using (var trans = await this.unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    await this.authorService.DeleteAuthor(id);
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
