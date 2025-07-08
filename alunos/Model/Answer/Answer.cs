namespace alunos.Model.Answer
{
    public class Answer
    {

        public string message;
        public int statusCode;

        public Answer(string message, int statusCode)
        {
            this.message = message;
            this.statusCode = statusCode;
        }
    }

    public class Answer<T> : Answer where T : class
    {

        public T data;
        public Answer(string message, int statusCode, T data) : base(message, statusCode)
        {
            this.data = data;
        }
    }
}
