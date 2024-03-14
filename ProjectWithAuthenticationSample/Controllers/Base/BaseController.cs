using Microsoft.AspNetCore.Mvc;
using Sample.Common.Exceptions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ILogger = Sample.Common.Logging.ILogger;
using Microsoft.Data.SqlClient;
using Sample.Common.Extensions;
using ProjectWithAuthenticationSample.Utilities;


namespace ProjectWithAuthenticationSample.Controllers.Base
{
    public class BaseController : Controller
    {
        protected ILogger Logger { get; set; }

        protected BaseController(ILogger logger)
        {
            Logger = logger;
        }

        protected void ShowSuccessMessage(string message = "Muvaffaqiyatli.")
        {
            TempData["SuccessMessage"] = EscapeMessage(message);
        }

        protected void ShowErrorMessage(string message = "Xatolik yuz berdi.", bool withLocalization = false)
        {
            if (withLocalization || message.Equals("Error happened.", StringComparison.InvariantCultureIgnoreCase))
            {
                TempData["ErrorMessage"] = EscapeMessage(message);

                return;
            }

            TempData["ErrorMessage"] = EscapeMessage(message);
        }

        protected void ShowNotification(string message, bool withLocalization = false)
        {
            if (string.IsNullOrEmpty(message))
                return;

            if (withLocalization)
            {
                TempData["Notification"] = EscapeMessage(message);

                return;
            }

            TempData["Notification"] = EscapeMessage(message);
        }

        protected IActionResult JsonResult(object data)
        {
            return Json(new
            {
                data
            });
        }

        protected IActionResult JsonOkResult()
        {
            return Json(new
            {
                data = "Successful."
            });
        }

        protected IActionResult JsonUnauthorizedResult()
        {
            Response.StatusCode = 401;

            return Json(new
            {
                errorMessage = "Unauthorized."
            });
        }

        protected IActionResult JsonForbidResult()
        {
            Response.StatusCode = 403;

            return Json(new
            {
                errorMessage = "Forbidden."
            });
        }

        protected IActionResult JsonNotFoundResult()
        {
            Response.StatusCode = 404;

            return Json(new
            {
                errorMessage = "Not Found."
            });
        }

        protected IActionResult JsonErrorResult(Exception exception)
        {
            Response.StatusCode = 500;

            return Json(new
            {
                errorMessage = exception.Message,
                stackTrace = exception.StackTrace
            });
        }

        protected IActionResult JsonErrorResult(string errorMessage)
        {
            Response.StatusCode = 500;

            return Json(new
            {
                errorMessage
            });
        }

        protected IActionResult JsonValidationErrorResult(IEnumerable<ValidationResult> validationResults)
        {
            Response.StatusCode = 400;

            var errorsDictionary = new Dictionary<string, string>();

            foreach (var validationResult in validationResults)
            {
                foreach (var memberName in validationResult.MemberNames.Where(m => !string.IsNullOrEmpty(m)))
                {
                    if (errorsDictionary.ContainsKey(memberName))
                        errorsDictionary[memberName] =
                            errorsDictionary[memberName] + " " + validationResult.ErrorMessage;
                    else
                        errorsDictionary.Add(memberName, validationResult.ErrorMessage);
                }
            }

            return Json(errorsDictionary);
        }

        /// <summary>
        /// Handle exception hook.
        /// </summary>
        /// <param name="exception">Exception to handle.</param>
        /// <returns>IAction result. If NULL do not return it from controller later.</returns>
        protected virtual IActionResult HandleException(Exception exception)
        {
            if (exception is NotFoundException)
                return NotFound();
            if (exception is ForbiddenException)
                return Forbid();
            if (exception is ValidationResultException validationResultException)
                HandleValidationResultException(validationResultException);
            else if (exception is BaseSampleException betonException)
                ShowErrorMessage(betonException.Message);
            else
            {
                // This type of error happens when user tries to delete an entity without deleting its child objects
                if (exception.InnerException is SqlException sqlException && sqlException.Number == 547)
                    ShowErrorMessage("Delete all related entities first.");
                else
                {
                    Logger.Error("Error while doing generic operation:", exception);
                    ShowErrorMessage(exception.GetInnerExceptionMessage());
                }
            }

            return null;
        }

        protected void HandleValidationResultException(ValidationResultException validationResultException)
        {
            ModelState.AddValidationResultException(validationResultException);
        }

        protected void HandleValidationResultException(ValidationResultException validationResultException, bool showErrorMessage)
        {
            var errorMessage = new StringBuilder();

            foreach (var validationResult in validationResultException.ValidationResults)
            {
                validationResult.ErrorMessage = validationResult.ErrorMessage;
                errorMessage.AppendLine(validationResult.ErrorMessage);
            }
            
            if (!showErrorMessage)
                ModelState.AddValidationResultException(validationResultException);
            else
                ShowErrorMessage(errorMessage.ToString());
        }

        private static string EscapeMessage(string message)
        {
            if (message.Length > 500)
                message = message.Substring(0, 500) + "...";

            var stringBuilder = new StringBuilder(message);
            stringBuilder.Replace("\r", "");
            stringBuilder.Replace("\n", "<br/>");
            return stringBuilder.ToString();
        }
    }
}
