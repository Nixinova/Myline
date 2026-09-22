using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Myline.Core.Utilities.Extensions;

public static class StringExtensions
{
	public static string RegexReplace(this string str, [StringSyntax(StringSyntaxAttribute.Regex)] string match, string replacement)
	{
		return Regex.Replace(str, match, replacement);
	}
}
