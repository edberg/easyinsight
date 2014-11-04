using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyInsight
{
    public interface IEasyInsight
    {
        Task Add<T>(IEnumerable<T> data);
        Task Replace<T>(IEnumerable<T> data);
        Task Define(Type type);
        Task DefineComposite(string name, IEnumerable<Connection> connections);

        event EventHandler<RequestEventArgs> OnRequest;
        event EventHandler<ResponseEventArgs> OnResponse;
    }
}