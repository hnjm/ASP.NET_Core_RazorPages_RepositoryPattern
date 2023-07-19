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
            try
            {
                await this.authorService.DeleteAuthor(id);
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
