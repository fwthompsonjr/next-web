namespace next.core.interfaces
{
    internal interface IUserMailboxMapper
    {
        string Substitute(IMailPersistence? persistence, string source);
    }
}
