namespace Stratis.Bitcoin.Features.AzureIndexer.Tests
{
    using NBitcoin;
    using NBitcoin.Networks;
    using Stratis.Bitcoin.Networks;

    public static class KnownNetworks
    {
        public static Network Main => NetworkRegistration.GetNetwork("Main") ?? NetworkRegistration.Register(new BitcoinMain());

        public static Network TestNet => NetworkRegistration.GetNetwork("TestNet") ?? NetworkRegistration.Register(new BitcoinTest());

        public static Network RegTest => NetworkRegistration.GetNetwork("RegTest") ?? NetworkRegistration.Register(new BitcoinRegTest());

        public static Network StraxMain => NetworkRegistration.GetNetwork("StraxMain") ?? NetworkRegistration.Register(new StraxMain());

        public static Network StraxTest => NetworkRegistration.GetNetwork("StraxTest") ?? NetworkRegistration.Register(new StraxTest());

        public static Network StraxRegTest => NetworkRegistration.GetNetwork("StraxRegTest") ?? NetworkRegistration.Register(new StraxRegTest());
    }
}
