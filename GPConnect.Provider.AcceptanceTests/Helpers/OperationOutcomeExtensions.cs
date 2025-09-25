using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Hl7.Fhir.Model;

namespace GPConnect.Provider.AcceptanceTests.Helpers
{
    public static class OperationOutcomeExtensions
    {
        /// <summary>
        /// Formats an OperationOutcome into a multi-line string that is easy to read in test logs.
        /// </summary>
        public static string ToMultilineString(this OperationOutcome oo, string title = null)
        {
            if (oo == null) return "<OperationOutcome: null>";

            var sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(title))
                sb.AppendLine(title);

            sb.AppendLine("---- OperationOutcome ----");
            sb.AppendLine($"Issues: {oo.Issue?.Count ?? 0}");

            if (oo.Issue != null && oo.Issue.Count > 0)
            {
                int i = 0;
                foreach (var issue in oo.Issue)
                {
                    i++;
                    sb.AppendLine($"[{i}] severity={issue.Severity} code={issue.Code}");

                    var detailsText = issue.Details?.Text;
                    var coding = issue.Details?.Coding?.FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(detailsText))
                        sb.AppendLine($"    details.text: {detailsText}");
                    if (coding != null)
                        sb.AppendLine($"    details.coding: {coding.System}|{coding.Code} ({coding.Display})");
                    if (!string.IsNullOrWhiteSpace(issue.Diagnostics))
                        sb.AppendLine($"    diagnostics: {issue.Diagnostics}");
                    if (issue.Location != null && issue.Location.Any())
                        sb.AppendLine($"    location: {string.Join(", ", issue.Location)}");
                    if (issue.Expression != null && issue.Expression.Any())
                        sb.AppendLine($"    expression: {string.Join(", ", issue.Expression)}");
                }
            }

            // Include narrative (plain text) if present
            var xhtml = oo.Text?.Div;
            if (!string.IsNullOrWhiteSpace(xhtml))
            {
                var stripped = Regex.Replace(xhtml, "<.*?>", string.Empty).Trim();
                if (!string.IsNullOrWhiteSpace(stripped))
                {
                    sb.AppendLine("Narrative:");
                    sb.AppendLine(stripped);
                }
            }

            sb.AppendLine("--------------------------");
            return sb.ToString();
        }
    }
}
