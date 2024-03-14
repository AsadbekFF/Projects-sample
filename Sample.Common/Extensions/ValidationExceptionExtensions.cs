using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Common.Extensions
{
    public static class ValidationExceptionExtensions
    {
        public static void AddError(this IList<ValidationResult> validationResults, string fieldName,
            string errorMessage)
        {
            validationResults.Add(new ValidationResult(errorMessage, new[] { fieldName }));
        }

        public static void AddError(this IList<ValidationResult> validationResults, string errorMessage)
        {
            validationResults.Add(new ValidationResult(errorMessage));
        }

        public static string ToString(this IList<ValidationResult> validationResults)
        {
            var result = new StringBuilder();

            foreach (var validationResult in validationResults)
            {
                if (validationResult.MemberNames != null && validationResult.MemberNames.Any())
                    result.Append(string.Join(", ", validationResult.MemberNames) + ": ");

                result.AppendLine(validationResult.ErrorMessage);
            }

            return result.ToString();
        }

        public static string GetErrorMessage(this IEnumerable<ValidationResult> validationResults)
        {
            var result = new StringBuilder();

            foreach (var validationResult in validationResults)
            {
                if (validationResult.MemberNames != null && validationResult.MemberNames.Any())
                    result.Append(string.Join(", ", validationResult.MemberNames) + ": ");

                result.AppendLine(validationResult.ErrorMessage);
            }

            return result.ToString();
        }
    }
}
