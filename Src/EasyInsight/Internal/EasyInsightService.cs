using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EasyInsight.Internal
{
    internal class EasyInsightService : IEasyInsight
    {
        private readonly string BaseUrl = "https://www.easy-insight.com/app/xml/";
        private readonly int PageSize = 1000;
        private string Username { get; set; }
        private string Password { get; set; }

        public event EventHandler<RequestEventArgs> OnRequest;
        public event EventHandler<ResponseEventArgs> OnResponse;

        public EasyInsightService(string username, string password)
        {
            this.Username = username;
            this.Password = password;
        }

        private async Task<string> Post(string uri, string xml)
        {
            var url = BaseUrl + uri;
            var client = new HttpClient();
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(Username + ":" + Password);
            var authHeader = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            client.DefaultRequestHeaders.Authorization = authHeader;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            if (OnRequest != null) OnRequest(this, new RequestEventArgs { Url = url, Xml = xml });
            var response = await client.PostAsync(url, new StringContent(xml));
            var result = await response.Content.ReadAsStringAsync();
            if (OnResponse != null) OnResponse(this, new ResponseEventArgs{ Code = response.StatusCode, Xml = result});
            if (!response.IsSuccessStatusCode)
                throw new Exception(string.Format("{0}\r\n{1}", response.StatusCode, result));
            return result;
        }

        private async Task<string> Define(Type type)
        {
            var dataSource = type.GetDataSource();
            var datafields = type.GetDataFields();
            var defineDataSource = new XElement("defineDataSource",
                new XElement("dataSourceName", dataSource.name),
                new XElement("fields", datafields.Select(f =>
                    new XElement("field",
                        new XAttribute("dataType", Enum.GetName(typeof(DataType), f.type).ToLower()),
                        new XElement("key", f.key),
                        new XElement("name", f.name)
                    ))
                )
            );
            var xml = defineDataSource.ToString();
            var result = await Post("defineDataSource", xml);
            return result.GetResponse().DataSourceKey;
        }

        private async Task Post<T>(string command, IEnumerable<T> data)
        {
            var type = typeof(T);
            var key = await Define(type);
            var dataSource = type.GetDataSource();
            var dataRows = (from d in data select d.GetData()).ToList();
            var rows = new XElement("rows",
                new XAttribute("dataSourceName", dataSource.name),
                dataRows.Select(row =>
                new XElement("row", row.Select(field =>
                    new XElement(field.Key.ToLower(), field.Value))
                ))
            );
            var xml = rows.ToString();
            await Post(command, xml);
        }

        private async Task Load<T>(string transactionId, IEnumerable<T> data)
        {
            var dataRows = (from d in data select d.GetData()).ToList();
            var rows = new XElement("rows",
                new XAttribute("transactionID", transactionId),
                dataRows.Select(row =>
                new XElement("row", row.Select(field =>
                    new XElement(field.Key.ToLower(), field.Value))
                ))
            );
            var xml = rows.ToString();
            await Post("loadRows", xml);
        }

        private async Task<string> BeginTransaction(string datasource, bool replace)
        {
            var beginTransaction = new XElement("beginTransaction",
                new XElement("dataSourceName", datasource),
                new XElement("operation", replace ? "replace" : "add")
            );
            var xml = beginTransaction.ToString();
            var result = await Post("beginTransaction", xml);
            return result.GetResponse().TransactionId;
        }

        private async Task Commit(string transactionId)
        {
            var beginTransaction = new XElement("commit",
                new XElement("transactionID", transactionId)
            );
            var xml = beginTransaction.ToString();
            var result = await Post("commit", xml);
        }

        public async Task Add<T>(IEnumerable<T> data)
        {
            if (data.Count() < PageSize) await Post("addRows", data);
            else
            {
                var type = typeof(T);
                var ds = typeof(T).GetDataSource().name;
                var key = await Define(type);
                var transactionid = await BeginTransaction(ds, false);
                await data.ForEachPage(PageSize, async (page) => { await Load(transactionid, page); });
                await Commit(transactionid);
            }
        }

        public async Task Replace<T>(IEnumerable<T> data)
        {
            if (data.Count() < PageSize) await Post("replaceRows", data);
            else
            {
                var type = typeof(T);
                var ds = typeof(T).GetDataSource().name;
                var key = await Define(type);
                var transactionid = await BeginTransaction(ds, true);
                await data.ForEachPage(PageSize, async (page) => { await Load(transactionid, page); });
                await Commit(transactionid);
            }
        }
    }
}
