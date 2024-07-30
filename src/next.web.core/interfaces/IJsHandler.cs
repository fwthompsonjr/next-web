using next.web.core.models;
using next.web.core.reponses;

namespace next.web.core.interfaces
{
    internal interface IJsHandler
    {
        string Name { get; }

        FormSubmissionResponse Submit(FormSubmissionModel model);
    }
}
