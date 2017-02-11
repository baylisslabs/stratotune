
import jsonpiperpc as jrpc


class Proxy:
    def __init__(self):
        self.ep = jrpc.RpcEndPoint("bin/Release/bit.shared.audio.FuncTests.exe")

    def PitchDetector_Process32BitMonoLinearPCM(self,pcmData,sampleRate):
        return self.ep.call_method('PitchDetector_Process32BitMonoLinearPCM',[pcmData,sampleRate])
        
    def EnvelopeModulator_Generate (self,gain,nSamples,sampleRate,waveForm,f0):
        return self.ep.call_method('EnvelopeModulator_Generate',[gain,nSamples,sampleRate,waveForm,f0])        

