using SecureSubmit.Infrastructure.Validation;
using SecureSubmit.Infrastructure;
namespace SecureSubmit.Entities
{
    public class HpsConsumer
    {
        private string firstName;
        private string lastName;
        private string phone;
        private string email;
        /// <summary>Gets or sets the consumer's first name.</summary>
        public string FirstName
        {
            get
            {
                return firstName;
            }
            set
            {
                firstName = HpsInputValidation.CardHolderDetails(value, AddressFields.FirstName);
            }
        }

        /// <summary>Gets or sets the consumer's first name.</summary>
        public string LastName
        {
            get
            {
                return lastName;
            }
            set
            {
                lastName = HpsInputValidation.CardHolderDetails(value, AddressFields.LastName);
            }
        }

        /// <summary>Gets or sets the consumer's phone number (optional).</summary>
        public string Phone
        {
            get
            {
                return phone;
            }
            set
            {
                phone = HpsInputValidation.CheckPhoneNumber(value);
            }
        }

        /// <summary>Gets or sets the consumer's email address (optional).</summary>
        public string Email
        {
            get
            {
                return email;
            }
            set
            {
                email = HpsInputValidation.CheckValidEmail(value);
            }
        }

        /// <summary>Gets or sets the address.</summary>
        public HpsAddress Address { get; set; }
    }
}
