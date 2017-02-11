using System;
using System.Runtime.InteropServices;

namespace bit.shared.numerics
{
    public static class vDSP
    {          
#if  __ARM_NEON__
		[DllImport("__Internal",EntryPoint = "vDSP_vmul")]
		public extern unsafe static void vmul (
			float* input_1, /* input vector 1 */
			int stride_1, /* address stride for input vector 1 */
			float* input_2, /* input vector 2 */
			int stride_2, /* address stride for input vector 2 */
			float* result, /* output vector */
			int strideResult, /* address stride for output vector */ 
			int size /* real output count */);

		[DllImport("__Internal",EntryPoint = "vDSP_vmulD")]
        public extern unsafe static void vmulD (
			double* input_1, /* input vector 1 */
			int stride_1, /* address stride for input vector 1 */
			double* input_2, /* input vector 2 */
			int stride_2, /* address stride for input vector 2 */
			double* result, /* output vector */
			int strideResult, /* address stride for output vector */ 
			int size /* real output count */);
#else
		public unsafe static void vmul (
			float* input_1, /* input vector 1 */
			int stride_1, /* address stride for input vector 1 */
			float* input_2, /* input vector 2 */
			int stride_2, /* address stride for input vector 2 */
			float* result, /* output vector */
			int strideResult, /* address stride for output vector */ 
			int size /* real output count */)
        {
            int a=0,b=0,c=0;
            
            for(int i=0;i<size;++i)
            {
                result[c] = input_1[a]*input_2[b];    
                a += stride_1;
                b += stride_2;
                c += strideResult;
            }            
        } 
#endif

#if  __ARM_NEON__
		[DllImport("__Internal",EntryPoint = "vDSP_vsq")]
		public extern static void vsq(
			float[] input_1, /* input vector 1 */
			int stride_1, /* address stride for input vector 1 */
			float[] result, /* output vector */
			int strideResult, /* address stride for output vector */ 
			int size /* real output count */);

		[DllImport("__Internal",EntryPoint = "vDSP_vsqD")]
		public extern static void vsqD(
			double[] input_1, /* input vector 1 */
			int stride_1, /* address stride for input vector 1 */
			double[] result, /* output vector */
			int strideResult, /* address stride for output vector */ 
			int size /* real output count */);
#else
        public static void vsq(
            float[] input_1, /* input vector 1 */
            int stride_1, /* address stride for input vector 1 */
            float[] result, /* output vector */
            int strideResult, /* address stride for output vector */ 
            int size /* real output count */)
        {
            int a=0,c=0;
            
            for(int i=0;i<size;++i)
            {
                result[c] = input_1[a]*input_1[a];    
                a += stride_1;
                c += strideResult;
            }     
        } 
#endif

#if  __ARM_NEON__
		[DllImport("__Internal",EntryPoint = "vDSP_sve")]
		public extern static void sve (
			float[] input_1, /* input vector 1 */
			int stride_1, /* address stride for input vector 1 */
			out float result,
			int size);

		[DllImport("__Internal",EntryPoint = "vDSP_sveD")]
		public extern static void sveD (
			double[] input_1, /* input vector 1 */
			int stride_1, /* address stride for input vector 1 */
			out double result,
			int size);
#else
        public static void sve (
           float[] input_1, /* input vector 1 */
           int stride_1, /* address stride for input vector 1 */
           out float result,
           int size)
        {
            int a=0;
            result = 0;
            for(int i=0;i<size;++i)
            {
                result += input_1[a];
                a += stride_1;
            }
        }
#endif

#if  __ARM_NEON__
		[DllImport("__Internal",EntryPoint = "vDSP_vdpsp")]
		public extern static void vdpsp (
			double[] input_1, /* input vector 1 */
			int stride_1, /* address stride for input vector 1 */
			float[] result, /* output vector */
			int strideResult, /* address stride for output vector */ 
			int size);			
#else
        public static void vdpsp (
           double[] input_1, /* input vector 1 */
            int stride_1, /* address stride for input vector 1 */
            float[] result, /* output vector */
            int strideResult, /* address stride for output vector */ 
            int size)
        {
            int a=0,c=0;
            
            for(int i=0;i<size;++i)
            {
                result[c] = (float)input_1[a];
                a += stride_1;
                c += strideResult;
            }   
        }
#endif
	}
}

