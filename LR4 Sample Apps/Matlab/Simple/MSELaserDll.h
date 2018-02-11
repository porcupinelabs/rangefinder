// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the MSELASERDLL_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// MSELASERDLL_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef MSELASERDLL_EXPORTS
#define MSELASERDLL_API __declspec(dllexport)
#else
#define MSELASERDLL_API __declspec(dllimport)
#endif


#ifndef MYMETHOD_H
#define MYMETHOD_H
#ifdef __cplusplus
extern "C" {  // only need to export C interface if
			  // used by C++ source code
#endif

	extern MSELASERDLL_API int nMSELaserDll;

	MSELASERDLL_API int MseInitLRF(void);
	MSELASERDLL_API int MseReadSingleDistance(void);
	MSELASERDLL_API int MseCloseLRF(void);

#ifdef __cplusplus
}
#endif
#endif

