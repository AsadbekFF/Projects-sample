using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectWithAuthenticationSample.Infrastructure;
using Sample.BLL.Infrastructure;
using Sample.Common.Base;
using System.Security.Cryptography;
using ILogger = Sample.Common.Logging.ILogger;

namespace ProjectWithAuthenticationSample.Controllers.Base
{
    [Authorize]
    [Route("[controller]")]
    public class CrudController<T, TViewModel, TId, TContext> : BaseController where T : BaseEntity<TId>, new() where TContext : DbContext
    {
        protected readonly IMapper Mapper;
        protected readonly BaseService<T, TId, TContext> Service;

        protected CrudController(ILogger logger, IMapper mapper, BaseService<T, TId, TContext> service) : base(logger)
        {
            Mapper = mapper;
            Service = service;
        }

        [HttpGet]
        public virtual IActionResult Index()
        {
            return View();
        }

        [HttpGet("[action]")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public virtual async Task<IActionResult> GetDataJson(string sort, string order, int limit, int offset)
        {
            var result = await Service.FilterAsync(sort, order, limit, offset);
            return Json(new { total = result.Total, rows = result.Rows });
        }

        [HttpGet("[action]")]
        public virtual IActionResult Add()
        {
            return View("AddEdit");
        }

        [HttpPost("[action]")]
        public virtual async Task<IActionResult> Add(TViewModel model, bool goBack)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var id = await AddAction(model);
                    ShowSuccessMessage();
                    return goBack ? RedirectToIndexAfterSave(model) : RedirectToAction("Edit", new { id });
                }
                catch (Exception ex)
                {
                    var result = HandleException(ex);
                    if (result != null)
                        return result;
                }
            }

            return View("AddEdit", model);
        }

        /// <summary>
        /// This action will be called if the model for Add method will be validated. Usually services for add are called here. 
        /// It is moved to separate method in order to keep the error handling and other common things DRY.
        /// </summary>
        /// <param name="model">View model.</param>
        /// <returns></returns>
        protected virtual async Task<TId> AddAction(TViewModel model)
        {
            var entity = Mapper.Map<TViewModel, T>(model);
            await AfterMappingToEntity(entity, model);
            await Service.AddAsync(entity);
            return entity.Id;
        }

        [HttpGet("[action]/{id}")]
        public virtual async Task<IActionResult> Edit(TId id)
        {
            var entity = await Service.GetByIdNotTrackingAsync(id);
            var viewModel = Mapper.Map<T, TViewModel>(entity);
            await AfterMappingToViewModel(entity, viewModel);
            return View("AddEdit", viewModel);
        }

        [HttpPost("[action]/{id}")]
        public virtual async Task<IActionResult> Edit(TId id, TViewModel model, bool goBack)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await EditAction(model, id);
                    ShowSuccessMessage();

                    return goBack ? RedirectToIndexAfterSave(model) : RedirectToAction("Edit", new { id });
                }
                catch (Exception ex)
                {
                    var result = HandleException(ex);
                    if (result != null)
                        return result;
                }
            }

            return View("AddEdit", model);
        }

        /// <summary>
        /// This action will be called if the model for Edit method will be validated. Usually services for update are called here. 
        /// It is moved to separate method in order to keep the error handling and other common things DRY.
        /// </summary>
        /// <param name="model">View model.</param>
        /// <param name="id">Entity ID.</param>
        /// <returns></returns>
        protected virtual async Task EditAction(TViewModel model, TId id)
        {
            var entity = Mapper.Map<TViewModel, T>(model);
            await AfterMappingToEntity(entity, model);
            entity.Id = id;
            await Service.UpdateAsync(entity);
        }

        [HttpPost("[action]/{id}")]
        public virtual async Task<IActionResult> Delete(TId id, IFormCollection formCollection, string returnUrl)
        {
            try
            {
                await DeleteAction(id);
                ShowSuccessMessage();
            }
            catch (Exception ex)
            {
                var result = HandleException(ex);
                if (result != null)
                    return result;
            }

            if (string.IsNullOrEmpty(returnUrl))
                return RedirectToIndexAfterSave(default);

            return Redirect(returnUrl);
        }

        /// <summary>
        /// This hook will be called on entity delete. Usually services for delete are called here. It is moved to separate method in order to keep the error handling and other common things DRY.
        /// </summary>
        /// <param name="id">Entity ID.</param>
        protected virtual async Task DeleteAction(TId id)
        {
            await Service.DeleteByIdAsync(id);
        }

        /// <summary>
        /// Hook for specific logic which cannot be stored in AutoMapper profile. The entity can be modified here.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="viewModel">View model.</param>
        protected virtual async Task AfterMappingToEntity(T entity, TViewModel viewModel)
        {

        }

        /// <summary>
        /// Hook for specific logic which cannot be stored in AutoMapper profile. The view model can be modified here.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="viewModel">View model.</param>
        protected virtual async Task AfterMappingToViewModel(T entity, TViewModel viewModel)
        {

        }

        /// <summary>
        /// Action fo redirection to grid after save on add or edit pages.
        /// </summary>
        /// <param name="viewModel">View model.</param>
        /// <returns>IAction result of the redirect.</returns>
        protected virtual IActionResult RedirectToIndexAfterSave(TViewModel viewModel)
        {
            return RedirectToAction("Index");
        }
    }
}
