using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Common.Exceptions
{
    public class ValidationResultException : BaseSampleException
    {
        public IEnumerable<ValidationResult> ValidationResults { get; }

        public ValidationResultException(IEnumerable<ValidationResult> validationResults)
        {
            ValidationResults = validationResults;
        }

        public ValidationResultException(string field, string errorMessage)
        {
            ValidationResults = new List<ValidationResult>
            {
                new ValidationResult(errorMessage, new [] { field })
            };
        }

        public ValidationResultException(string errorMessage)
        {
            ValidationResults = new List<ValidationResult>
            {
                new ValidationResult(errorMessage)
            };
        }
    }
}
