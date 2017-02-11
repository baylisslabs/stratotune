
import subprocess as sp
import json

class RpcEndPoint:
    def __init__(self,exeName):
        self.exeName = exeName
        self.next_id = 0
        
    def call_method(self,method,params):
        self.next_id = self.next_id + 1
        rpc_call = {'id':self.next_id, 'method': method, 'params': params}
        p1 = sp.Popen(self.exeName,stdin=sp.PIPE,stdout=sp.PIPE)
        outdata_json = p1.communicate(input=json.dumps(rpc_call))
        outdata=json.loads(outdata_json[0])
        if outdata['id'] != rpc_call['id']:
            raise Exception('rpc return id mismatch')
        if 'error' in outdata:
            raise Exception('service error',outdata['error'])
            
        return outdata['result']


