#ifndef _ATLR_H_
#define _ATLR_H_

// Error codes.
#define ERROR_USB_DEVICE_NOT_FOUND          0xE0000001
#define ERROR_USB_DEVICE_NO_CAPABILITIES    0xE0000002

// name of the DLL to be loaded
#define AT_USBHID_DLL "AtUsbHid"

// Implement the DLL export/import mechanism and allow a C-written program
// to use our DLL.
#ifdef ATLR_EXPORTS
#define ATLR_API extern "C" __declspec(dllexport)
#else
#define ATLR_API extern "C" __declspec(dllimport)
#endif


// This macro function calls the C runtime's _beginthreadex function. 
// The C runtime library doesn't want to have any reliance on Windows' data 
// types such as HANDLE. This means that a Windows programmer needs to cast
// values when using _beginthreadex. Since this is terribly inconvenient, 
// this macro has been created to perform the casting.
typedef unsigned(__stdcall *PTHREAD_START)(void *);

#define chBEGINTHREADEX(psa, cbStack, pfnStartAddr, \
   pvParam, fdwCreate, pdwThreadId)                 \
      ((HANDLE)_beginthreadex(                      \
         (void *)        (psa),                     \
         (unsigned)      (cbStack),                 \
         (PTHREAD_START) (pfnStartAddr),            \
         (void *)        (pvParam),                 \
         (unsigned)      (fdwCreate),               \
(unsigned *)(pdwThreadId)))

// Allow applications not built with Microsoft Visual C++ to link with our DLL.
#define STDCALL __stdcall

// These macros make calling our DLL functions through pointers easier.
#define DECLARE_FUNCTION_POINTER(FUNC)  PF_##FUNC lp##FUNC=NULL;  
#define LOAD_FUNCTION_POINTER(DLL,FUNC) lp##FUNC = (PF_##FUNC)GetProcAddress(DLL, #FUNC);
#define ADDR_CHECK(FUNC) if (lp##FUNC == NULL) {fprintf(stderr,"%s\n", "Error: Cannot get address of function."); return FALSE;}
#define DYNCALL(FUNC) lp##FUNC


///////////////////////////////////////////////////////////////////////////////
typedef BOOLEAN (STDCALL *PF_findHidDevice)(const UINT VendorID, const UINT ProductID);
typedef void    (STDCALL *PF_closeDevice)(void);
typedef BOOLEAN (STDCALL *PF_writeData)(UCHAR* buffer);
typedef BOOLEAN (STDCALL *PF_readData)(UCHAR* buffer);
typedef int     (STDCALL *PF_hidRegisterDeviceNotification)(HWND hWnd);
typedef void    (STDCALL *PF_hidUnregisterDeviceNotification)(HWND hWnd);
typedef int     (STDCALL *PF_isMyDeviceNotification)(DWORD dwData);
typedef BOOLEAN (STDCALL *PF_startBootLoader)(void);
typedef BOOLEAN (STDCALL *PF_setFeature)(UCHAR type,UCHAR direction, unsigned int length);
///////////////////////////////////////////////////////////////////////////////

// Exported functions prototypes.

///////////////////////////////////////////////////////////////////////////////
ATLR_API BOOLEAN STDCALL findHidDevice(const UINT VendorID, const UINT ProductID);

//  Closes the USB device and all handles before exiting the application.
ATLR_API void    STDCALL closeDevice(void);

ATLR_API BOOLEAN STDCALL writeData(UCHAR* buf);

ATLR_API BOOLEAN STDCALL readData(UCHAR* buffer);

ATLR_API int     STDCALL hidRegisterDeviceNotification(HWND hWnd);

ATLR_API void    STDCALL hidUnregisterDeviceNotification(HWND hWnd);

ATLR_API int     STDCALL isMyDeviceNotification(DWORD dwData);

ATLR_API BOOLEAN STDCALL setFeature(UCHAR type,UCHAR direction, unsigned int length);

///////////////////////////////////////////////////////////////////////////////

#ifndef ATLR_EXPORTS


DECLARE_FUNCTION_POINTER(findHidDevice);
DECLARE_FUNCTION_POINTER(closeDevice);
DECLARE_FUNCTION_POINTER(writeData);
DECLARE_FUNCTION_POINTER(readData);
DECLARE_FUNCTION_POINTER(hidRegisterDeviceNotification);
DECLARE_FUNCTION_POINTER(hidUnregisterDeviceNotification);
DECLARE_FUNCTION_POINTER(isMyDeviceNotification);
DECLARE_FUNCTION_POINTER(setFeature);

// this function call all function available in the DLL *
static bool loadFuncPointers(HINSTANCE hLib)
{
    LOAD_FUNCTION_POINTER(hLib, findHidDevice);
    ADDR_CHECK(findHidDevice);

	LOAD_FUNCTION_POINTER(hLib, closeDevice);
    ADDR_CHECK(closeDevice);

	LOAD_FUNCTION_POINTER(hLib, writeData);
    ADDR_CHECK(writeData);

	LOAD_FUNCTION_POINTER(hLib, readData);
    ADDR_CHECK(readData);

	LOAD_FUNCTION_POINTER(hLib, hidRegisterDeviceNotification);
	ADDR_CHECK(hidRegisterDeviceNotification);

	LOAD_FUNCTION_POINTER(hLib, hidUnregisterDeviceNotification);
	ADDR_CHECK(hidUnregisterDeviceNotification);

	LOAD_FUNCTION_POINTER(hLib, isMyDeviceNotification);
	ADDR_CHECK(isMyDeviceNotification);

	LOAD_FUNCTION_POINTER(hLib, setFeature);
	ADDR_CHECK(setFeature);

    return true;
}

#endif

#endif  // _ATLR_H_
