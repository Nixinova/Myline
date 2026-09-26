using System.Diagnostics.CodeAnalysis;

namespace Myline.Core.Models;

public readonly struct Result<TValue>
{
	[MemberNotNullWhen(true, nameof(Error))]
	[MemberNotNullWhen(false, nameof(Value))]
	public bool IsError { get; }

	public TValue Value
	{
		get => IsError
			? throw new InvalidOperationException("Cannot get value of failed Result")
			: field!;
		private init;
	}

	public string Error
	{
		get => IsError
			? field!
			: throw new InvalidOperationException("Cannot get error from successful Result");
		private init;
	}

	private Result(TValue? value, string? error)
	{
		if (!(value is null ^ error is null))
		{
			throw new InvalidOperationException("Mutually exclusive");
		}

		if (value is not null)
		{
			IsError = false;
			Value = value;
		}
		else
		{
			IsError = true;
			Error = error!;
		}
	}

	public static Result<TValue> Ok(TValue value) => new(value, null);

	public static Result<TValue> Fail(string error) => new(default, error);

	public static implicit operator Result<TValue>(TValue value) {
		return Ok(value);
	}
}
