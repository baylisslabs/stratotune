using System;

using Jayrock.JsonRpc;
using Jayrock.Services;

namespace bit.shared.testutil
{			
	public class JsonPipeRpcDispatcher
	{
        private IService _service;
        
        public JsonPipeRpcDispatcher(IService service)
        {
            _service = service; 
        }
        
		public void RunLoop(string[] args)
		{
			JsonRpcDispatcherFactory.Current = s => new JsonRpcDispatcher(s);
			var dispatcher = JsonRpcDispatcherFactory.CreateDispatcher(_service);
			
			string line;
			while((line=Console.ReadLine())!=null) {
				string result = dispatcher.Process(line);
				Console.WriteLine(result);
			}
		}
	}
}
