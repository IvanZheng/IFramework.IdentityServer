namespace IdentityServer4Client.Authorizations
{   
    class Role
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string ScopeId { get; set; }

        public Role(){}

        public Role(string roleClaimValue)
        {
            Key = roleClaimValue;
            var values = roleClaimValue.Split(':');
            if (values.Length > 1)
            {
                Name = values[0];
                ScopeId = values[1];
            }
            else
            {
                Name = values[0];
            }
        }
    }
}
