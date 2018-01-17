using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Polyclinic.CustomValidators
{
    public class MoreThanProperty:ValidationAttribute,IClientModelValidator
    {
        private readonly string  _otherProperty;
        private readonly string  _errorMessage;

        public MoreThanProperty(string OtherProperty,string ErrorMessage)
        {
            _otherProperty = OtherProperty;
            _errorMessage = ErrorMessage;

        }

        public void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-rule-moreThanInput", _otherProperty);
            MergeAttribute(context.Attributes, "data-msg-moreThanInput", $"{ _errorMessage }");
        }

        private bool MergeAttribute(IDictionary<string, string> attributes,
                                   string key, string value)
        {
            if (attributes.ContainsKey(key))
            {
                return false;
            }
            attributes.Add(key, value);
            return true;
        }

        protected override ValidationResult IsValid(object value, ValidationContext property)
        {
            Type type = property.ObjectType;

            var otherProperty = type.GetProperty(_otherProperty);
            if (otherProperty == null)
            {
                return new ValidationResult(
                    string.Format("Unknown property: {0}", _otherProperty)
                );
            }

            var otherValue = otherProperty.GetValue(property.ObjectInstance, null);

            MethodInfo Compare = otherProperty.PropertyType.GetMethod("Compare");

            if(Compare == null)
            {
                return new ValidationResult(
                    string.Format("Have no compare {0}", type.Name)
                );
            }

            int CompareRes = (int)Compare.Invoke(null, new object[] { value, otherValue });

            if(CompareRes < 0)
            {
                return new ValidationResult(
                     string.Format("{0} must be more than {1}", property.MemberName, otherProperty.Name)
                     );
            }

            return null;

        }
    }
}
