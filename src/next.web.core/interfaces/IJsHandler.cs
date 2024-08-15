using Microsoft.AspNetCore.Http;
using next.web.core.models;
using next.web.core.reponses;

namespace next.web.core.interfaces
{
    internal interface IJsHandler
    {
        string Name { get; }

        Task<FormSubmissionResponse> Submit(FormSubmissionModel model);
        Task<FormSubmissionResponse> Submit(FormSubmissionModel model, ISession session, IApiWrapper? wrapper = null);
    }
}
