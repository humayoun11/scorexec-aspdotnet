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
        public const int Tournament = 2;
        public const int Friendly = 1;
        public const int Series = 3;
    }

    public class EventStageConsts
    {
        public const int Group = 1;
        public const int Quarter = 2;
        public const int Semi = 3;
        public const int Final = 3;
    }
}
