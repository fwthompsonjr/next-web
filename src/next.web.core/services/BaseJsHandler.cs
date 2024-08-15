using Microsoft.AspNetCore.Http;
using next.web.core.interfaces;
using next.web.core.models;
using next.web.core.reponses;

namespace next.web.core.services
{
    internal class BaseJsHandler : IJsHandler
    {
        public virtual string Name => "base";

        public IApiWrapper? Wrapper { get; set; }

        public virtual Task<FormSubmissionResponse> Submit(FormSubmissionModel model)
        {
            throw new NotImplementedException();
        }

        public virtual Task<FormSubmissionResponse> Submit(FormSubmissionModel model, ISession session, IApiWrapper? wrapper = null)
        {
            throw new NotImplementedException();
        }
    }
}
