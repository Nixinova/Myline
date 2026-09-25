namespace Myline.Core.Utilities;

public static class OutputFormatHelper
{
	public const string Reset = "\00";
	public const string Primary = "\01";
	public const string Secondary = "\02";
	public const string Tertiary = "\03";
	public const string Addendum = "\0A";

	private static string Apply(string mode, string text) => mode + text + Reset;

	public static string PrimaryText(string text) => Apply(Primary, text);
	public static string SecondaryText(string text) => Apply(Secondary, text);
	public static string TertiaryText(string text) => Apply(Tertiary, text);
	public static string UserText(string text) => Apply(Addendum, text);
}

public static class Fmt
{
	public static string Prim(string text) => OutputFormatHelper.PrimaryText(text);
	public static string Sec(string text) => OutputFormatHelper.SecondaryText(text);
	public static string Ter(string text) => OutputFormatHelper.TertiaryText(text);
	public static string Usr(string text) => OutputFormatHelper.UserText(text);
}
