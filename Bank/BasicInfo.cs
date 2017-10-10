namespace Kundbilden
{
    public abstract class BasicInfo
    {
        public int Id { get; set; }
        public abstract int CreateUniqueId(Bank bank);
    }
}