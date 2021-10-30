namespace Wikibot.Logic.Extensions
{
    public class EmailResult
    {
        public string ResultCode { get; set; }
        public bool IsSuccess { get; set; }

        public EmailResult(string resultCode, bool isSuccess)
        {
            ResultCode = resultCode;
            IsSuccess = isSuccess;
        }
    }
}