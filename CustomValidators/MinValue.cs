using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

using System.ComponentModel.DataAnnotations;

namespace Polyclinic.CustomValidators
{
    public class MinValue:ValidationAttribute, IClientModelValidator
    {
        private readonly int _minValue;
        private readonly string _ErrorMessage;

        public MinValue(int minValue,string ErrorMessage)
        {
            _minValue = minValue;
            _ErrorMessage = ErrorMessage;
        }

        public void AddValidation(ClientModelValidationContext context)
        {

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            MergeAttribute(context.Attributes, "data-rule-min", _minValue.ToString());
            MergeAttribute(context.Attributes, "data-msg-min", $"{ _ErrorMessage }");   

        }

        private bool MergeAttribute(IDictionary<string, string> attributes,
                                    string key,string value)
        { 
            if (attributes.ContainsKey(key))
            {
                return false;
            }
            attributes.Add(key, value);
            return true;
        }

        public override bool IsValid(object value)
        {
            return (int)value >= _minValue;
        }

    }
}
