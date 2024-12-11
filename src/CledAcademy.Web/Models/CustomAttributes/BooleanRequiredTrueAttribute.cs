using System;
using System.ComponentModel.DataAnnotations;

namespace CledAcademy.Web.Models.CustomAttributes
{
    /// <summary>
    /// Validation attribute that demands that a boolean value must be true.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class BooleanRequiredTrueAttribute : RequiredAttribute
    {
        public override bool IsValid(object value)
        {
            return value != null && (bool)value;
        }
    }
}