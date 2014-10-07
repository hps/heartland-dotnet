namespace SecureSubmit.Entities
{
    public class HpsConsumer
    {
        /// <summary>Gets or sets the consumer's first name.</summary>
        public string FirstName { get; set; }

        /// <summary>Gets or sets the consumer's first name.</summary>
        public string LastName { get; set; }

        /// <summary>Gets or sets the consumer's phone number (optional).</summary>
        public string Phone { get; set; }

        /// <summary>Gets or sets the consumer's email address (optional).</summary>
        public string Email { get; set; }

        /// <summary>Gets or sets the address.</summary>
        public HpsAddress Address { get; set; }
    }
}
