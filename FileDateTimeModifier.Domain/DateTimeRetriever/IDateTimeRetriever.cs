using System;

namespace FileDateTimeModifier.Domain.DateTimeRetriever
{
    interface IDateTimeRetriever
    {
        DateTime? RetrieveDateTime(string fullFilePath);
    }
}
