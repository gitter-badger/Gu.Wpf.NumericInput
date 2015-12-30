﻿namespace Gu.Wpf.NumericInput.Validation
{
    using System;
    using System.Windows.Controls;

    public class IsLessThanMinValidationResult : ValidationResult
    {
        public IsLessThanMinValidationResult(IFormattable value, IFormattable min, bool isValid, object errorContent)
            : base(isValid, errorContent)
        {
            this.Value = value;
            this.Min = min;
        }

        public IFormattable Value { get; }

        public IFormattable Min { get; }
    }
}