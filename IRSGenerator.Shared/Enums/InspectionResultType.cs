namespace IRSGenerator.Shared.Enums;

public enum InspectionResultType
{
    Unidentified,
    OutOfTolerance,
    WithinTolerance,
    WrongFormat,
    MinMaxValueOverTolerance,
    MinMaxValueUnderTolerance,
    MaxValueOverTolerance,
    MinValueUnderTolerance
}
