using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sample.Common.Exceptions;
using System.Text;

namespace ProjectWithAuthenticationSample.Utilities
{
    public static class ModelStateExtensions
    {
        public static void AddValidationResultException(this ModelStateDictionary modelStateDictionary,
           ValidationResultException validationResultException)
        {
            foreach (var validationResult in validationResultException.ValidationResults)
            {
                if (validationResult.MemberNames.Any())
                    foreach (var memberName in validationResult.MemberNames)
                        modelStateDictionary.AddModelError(memberName, validationResult.ErrorMessage);
                else
                    modelStateDictionary.AddModelError("", validationResult.ErrorMessage);
            }
        }

        public static string GetErrorMessage(this ModelStateDictionary modelStateDictionary)
        {
            var stringBuilder = new StringBuilder();

            foreach (var modelState in modelStateDictionary.Values)
                foreach (var error in modelState.Errors)
                    stringBuilder.Append(error.ErrorMessage + Environment.NewLine);

            return stringBuilder.Length == 0 ? null : stringBuilder.ToString(0, stringBuilder.Length - Environment.NewLine.Length);
        }
    }
}
