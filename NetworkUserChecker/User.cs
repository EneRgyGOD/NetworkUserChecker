namespace NetworkUserChecker
{
    public class User
    {
        public string Name;
        public string Adress;
        public bool OnlineState;

        public string getOnlineStatus()
        {
            return (OnlineState ? "online" : "offline");
        
        }
        public User(string Name, string Adress)
        {
            this.Name = Name;
            this.Adress = Adress;
        }
    }
}