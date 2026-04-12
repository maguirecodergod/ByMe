namespace HTT.BlazorWasm.App.Components
{
    public interface IToaster
    {
        void Success(string message, string? title = null);
        void Error(string message, string? title = null);
        void Warning(string message, string? title = null);
        void Info(string message, string? title = null);
    }

    public class NullToaster : IToaster
    {
        public void Success(string message, string? title = null) { }
        public void Error(string message, string? title = null) { }
        public void Warning(string message, string? title = null) { }
        public void Info(string message, string? title = null) { }
    }
}
