namespace FitnessApp.Common.Abstractions.Models.Validation
{
    public class ValidationError
    {
#pragma warning disable SA1300 // Element should begin with upper-case letter
        private string _message { get; set; }
        private string _field { get; set; }
#pragma warning restore SA1300 // Element should begin with upper-case letter

        public ValidationError(string message, string field)
        {
            _message = message;
            _field = field;
        }

        public override string ToString()
        {
            return $"Field validation failed, field name: {_field} , message: {_message}.";
        }
    }
}
