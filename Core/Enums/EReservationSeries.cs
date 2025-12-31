namespace RR.Core.Enums
{
    public enum EReservationFrequency
    {
        MONTHLY,
        WEEKLY,
        YEARLY
    }

    [Flags]
    public enum EReservationDay
    {
        None = 0,//000000
        MO = 1 << 0,
        TU = 1 << 1,
        WE = 1 << 2,
        TH = 1 << 3,
        FR = 1 << 4,
        SA = 1 << 5,
        SU = 1 << 6,
        All = MO | TU | WE | TH | FR | SA | SU
    }
}
