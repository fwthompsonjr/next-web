namespace next.core.entities
{
    internal class ContactIdentity
    {
        private const string NotFound = " - n/a -";
        public string UserName { get; set; } = NotFound;
        public string Email { get; set; } = NotFound;
        public string Created { get; set; } = NotFound;
        public string Role { get; set; } = "Guest";
        public string RoleDescription { get; set; } = "";
    }
}