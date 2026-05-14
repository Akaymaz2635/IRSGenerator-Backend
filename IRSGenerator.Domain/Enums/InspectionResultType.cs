namespace MES.Domain.Enums;

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
