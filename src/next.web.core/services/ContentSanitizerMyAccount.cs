using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace next.web.core.services
{
    internal class ContentSanitizerMyAccount : ContentSanitizerBase
    {
        public override string Sanitize(string content)
        {
            var html = base.Sanitize(content);
            return html;
        }
    }
}
