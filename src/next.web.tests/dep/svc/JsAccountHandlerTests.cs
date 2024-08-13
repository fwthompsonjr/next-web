using legallead.desktop.entities;
using legallead.desktop.interfaces;
using Microsoft.AspNetCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace next.web.tests.dep.svc
{
    public class JsAccountHandlerTests
    {

        private sealed class AccountApi(int statusCode) : IPermissionApi
        {
            private readonly int _statusCode = statusCode;


            public IInternetStatus? InternetUtility => throw new NotImplementedException();

            public KeyValuePair<bool, ApiResponse> CanGet(string name)
            {
                return new(true, new());
            }

            public KeyValuePair<bool, ApiResponse> CanPost(string name, object payload, UserBo user)
            {
                return new(true, new());
            }

            public ApiResponse CheckAddress(string name)
            {
                throw new();
            }

            public Task<ApiResponse> Get(string name, UserBo user)
            {
                throw new NotImplementedException();
            }

            public Task<ApiResponse> Get(string name)
            {
                throw new NotImplementedException();
            }

            public Task<ApiResponse> Get(string name, Dictionary<string, string> parameters)
            {
                throw new NotImplementedException();
            }

            public Task<ApiResponse> Get(string name, UserBo user, Dictionary<string, string> parameters)
            {
                throw new NotImplementedException();
            }

            public Task<ApiResponse> Post(string name, object payload, UserBo user)
            {
                var response = new ApiResponse { StatusCode = _statusCode };
                response.Message = name switch
                {
                    "profile-edit-contact-name" => string.Empty,
                    "profile-edit-contact-address" => string.Empty,
                    "profile-edit-contact-phone" => string.Empty,
                    "frm-profile-email" => string.Empty,
                    "permissions-change-password" => string.Empty,
                    "permissions-set-discount" => string.Empty,
                    "permissions-set-permission" => string.Empty,
                    _ => "Response is not mapped."
                };
                return Task.FromResult(response);
            }
        }
    }
}
