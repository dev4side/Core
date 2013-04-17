using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Validation
{
    public sealed class ValidationMessages : List<ValidationMessage>
    {
        internal ValidationMessages() { }

        internal string GetFullErrorMessage()
        {
            var sb = new StringBuilder();
            foreach (var mex in this)
                sb.AppendLine(mex.Message);
            return sb.ToString();
        }

        internal string GetFullErrorMessageInHtml()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Join(@" \r\n ", this.Select(x => x.Message).ToArray()));
            return sb.ToString();
        }  
    }
}
