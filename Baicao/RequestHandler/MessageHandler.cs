using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using StackExchange.Redis;

namespace Baicao.RequestHandler
{
    public abstract class MessageHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var corId = Guid.NewGuid().ToString();
            var requestMessage = await request.Content.ReadAsByteArrayAsync();
            var uri = request.RequestUri.ToString();
            var reqMethod = request.Method.ToString();
            await HandleIncomingMessageAsync(corId, reqMethod, uri, requestMessage);
            var response = await base.SendAsync(request, cancellationToken);
            
            byte[] responseMessage;
            if (response.IsSuccessStatusCode)
            {
                responseMessage =  await response.Content.ReadAsByteArrayAsync();
            }
            else
            {
                responseMessage = Encoding.UTF8.GetBytes(response.ReasonPhrase);
            }
            await HandleResponseMessageAsync(corId, responseMessage);
            return response;
        }

        protected abstract Task HandleIncomingMessageAsync(string corId, string requestMethod, string uri,
            byte[] requestMessage);

        protected abstract Task HandleResponseMessageAsync(string corId, byte[] responseMessage);
    }

    public class PerformanceWatch : MessageHandler
    {
        private IConnectionMultiplexer multiplexer = null;
        private IDatabase db = null;
        public PerformanceWatch()
        {
            try
            {
                multiplexer = ConnectionMultiplexer.Connect("117.48.197.92");
                db = multiplexer.GetDatabase();
            }
            catch
            {

            }
        }

        protected override async Task HandleIncomingMessageAsync(string corId, string requestMethod, string uri,
            byte[] requestMessage)
        {
            if (db == null)
            {
                return;
            }
            
        }

        protected override async Task HandleResponseMessageAsync(string corId, byte[] responseMessage)
        {
            if (db == null)
            {
                return;
            }
            //send to redis server
           // throw new NotImplementedException();
        }
    }
}