using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Security
{
    public static class ApiManagementPermissions
    {
        public const string GroupName = nameof(Api) + "." + nameof(ApiManagementPermissions);
        public const string Get = GroupName + ".Get";
        public const string Post = GroupName + ".Post";
    }
}
