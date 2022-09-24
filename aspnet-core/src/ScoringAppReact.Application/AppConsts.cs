namespace ScoringAppReact
{
    public class AppConsts
    {
        /// <summary>
        /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
        /// </summary>
        public const string DefaultPassPhrase = "gsKxGZ012HLL3MI5";
        public const string SuccessfullyInserted = "Record Successfully Inserted";
        public const string SuccessfullyUpdated = "Record Successfully Updated";
        public const string SuccessFullyGetData = "Success";
        public const string SuccessfullyDeleted = "Record Successfully Deleted";
        public const string InsertFailure = "Record Failed To Insert";
        public const string UpdateFailure = "Record Failed To Update";
        public const string DeleteFailure = "Record Failed To Delete";

        public const int Tournament = 1;
        public const int Friendly = 2;
        public const int Series = 3;
    }

    public class MatchTypeConsts
    {
        public const int Tournament = 1;
        public const int Series = 2;
        public const int Friendly = 3;
    }

    public class EventStageConsts
    {
        public const int Group = 1;
        public const int Quarter = 2;
        public const int Semi = 3;
        public const int Final = 3;
    }

    public class ScoringBy
    {
        public const int Manuual = 1;
        public const int ScoringApp = 2;
    }

    public class MatchStatus
    {
        public const int Started = 1;
        public const int Break = 2;
        public const int Ended = 3;
        public const int Suspended = 4;
        public const int FirstInning = 1;
        public const int SecondInning = 2;
    }

    public class HowOut
    {
        public const int Not_Out = 1;
        public const int Bowled = 2;
        public const int Catch = 3;
        public const int Stump = 4;
        public const int Hit_Wicket = 5;
        public const int LBW = 6;
        public const int Run_Out = 7;
        public const int Retired = 8;
    }
}
