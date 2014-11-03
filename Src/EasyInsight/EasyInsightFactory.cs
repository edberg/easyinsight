using EasyInsight.Internal;

namespace EasyInsight
{
    public static class EasyInsightFactory
    {
        public static IEasyInsight Create(string username, string password)
        {
            return new EasyInsightService(username, password);
        }
    }
}
