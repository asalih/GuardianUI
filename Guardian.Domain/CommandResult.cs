namespace Guardian.Domain
{
    public class CommandResult<T>
    {
        public CommandResult()
        {

        }

        public CommandResult(bool isSucceeded)
        {
            IsSucceeded = isSucceeded;
        }

        public CommandResult(T result, bool isSucceeded)
        {
            Result = result;
            IsSucceeded = isSucceeded;
        }

        public T Result { get; set; }

        public bool IsSucceeded { get; set; }
    }
}
