// Copyright (c) 2016, Porcupine Electronics, LLC
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are permitted
// provided that the following conditions are met:
//
// * Redistributions of source code must retain the above copyright notice, this list of
//   conditions and the following disclaimer.
// * Redistributions in binary form must reproduce the above copyright notice, this list of
//   conditions and the following disclaimer in the documentation and/or other materials provided
//   with the distribution.
// * The name "Porcupine Electronics" may not be used to endorse or promote products derived from
//   this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR
// IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#include "stdafx.h"
#include "lr3.h"
#include "LRDemoCode.h"
#include "LRDemoCodeDlg.h"
#include <winuser.h>
#include <windows.h>
#include <dbt.h>
#include <math.h>
#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

#include "AtUsbHid.h"


/*---------------------------------------------------------------------------

FUNCTION: handleError

PURPOSE: Call when an error is return by a function call

PARMATERS: DWORD errorCode - error code that represent the error
  	
RETURN:
   
COMMENTS:
  
---------------------------------------------------------------------------*/
void handleError(DWORD errorCode)
{
    switch(errorCode)
    {
    case ERROR_MOD_NOT_FOUND:
		AfxMessageBox( "Could not find USB HID DLL: AtUsbHid.dll\nPlease update the PATH variable.\n", MB_ICONSTOP,0);
		exit(-1);        
        break;

    case ERROR_USB_DEVICE_NOT_FOUND:
        OutputDebugString("Error: Could not open the device.\n");
        break;

    case ERROR_USB_DEVICE_NO_CAPABILITIES:
        OutputDebugString("Error: Could not get USB device capabilities.\n");
        break;

    case ERROR_WRITE_FAULT:
        OutputDebugString("Error: Could not write.\n");
        break;

    case ERROR_READ_FAULT:
        OutputDebugString("Error: Could not read.\n");
        break;

    default:
        OutputDebugString("Error: Unknown error code.\n");
    }
}

/////////////////////////////////////////////////////////////////////////////
// CAboutDlg dialog used for App About

class CAboutDlg : public CDialog
{
public:
	CAboutDlg();

// Dialog Data
	//{{AFX_DATA(CAboutDlg)
	enum { IDD = IDD_ABOUTBOX };
	//}}AFX_DATA

	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CAboutDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	//{{AFX_MSG(CAboutDlg)
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

CAboutDlg::CAboutDlg() : CDialog(CAboutDlg::IDD)
{
	//{{AFX_DATA_INIT(CAboutDlg)
	//}}AFX_DATA_INIT
}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CAboutDlg)
	//}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CAboutDlg, CDialog)
	//{{AFX_MSG_MAP(CAboutDlg)
		// No message handlers
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CLRDemoCodeDlg dialog

CLRDemoCodeDlg::CLRDemoCodeDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CLRDemoCodeDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CLRDemoCodeDlg)
	//}}AFX_DATA_INIT
	// Note that LoadIcon does not require a subsequent DestroyIcon in Win32
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);


}

void CLRDemoCodeDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CLRDemoCodeDlg)
	DDX_Control(pDX, IDC_START, m_Start);
	//}}AFX_DATA_MAP
	DDX_Control(pDX, IDC_DISTANCE, m_Distance);
	DDX_Control(pDX, IDC_DISTANCE_DISPLAY, m_DistanceDisplay);
	DDX_Control(pDX, IDC_MEASUREMENT_MODE, m_MeasurementMode);
	DDX_Control(pDX, IDC_INTERVAL, m_Interval);
	DDX_Control(pDX, IDC_INTERVAL_SPIN, m_IntervalSpin);
	DDX_Control(pDX, IDC_INTERVAL_UNITS, m_IntervalUnits);
	DDX_Control(pDX, IDC_TRIGGER, m_Trigger);
	DDX_Control(pDX, IDC_KB_EMULATION, m_KbEmulation);
	DDX_Control(pDX, IDC_DO_DOUBLE_MEASUREMENTS, m_DoDoubleMeasurements);
	DDX_Control(pDX, IDC_DONT_FILTER_ERRORS, m_DontFilterErrors);
	DDX_Control(pDX, IDC_ONLY_SEND_CHANGES, m_OnlySendChanges);
	DDX_Control(pDX, IDC_IGNORE_OFF_TO_ON, m_IgnoreOffToOn);
	DDX_Control(pDX, IDC_IGNORE_ON_TO_OFF, m_IgnoreOnToOff);
	DDX_Control(pDX, IDC_ControlGroup, m_ControlGroup);
	DDX_Control(pDX, IDC_STATIC_DISTANCE_DISPLAY, m_StaticDistanceDisplay);
	DDX_Control(pDX, IDC_STATIC_MEASUREMENT_MODE, m_StaticMeasurementMode);
	DDX_Control(pDX, IDC_STATIC_INTERVAL, m_StaticInterval);
	DDX_Control(pDX, IDC_STATIC_TRIGGER, m_StaticTrigger);
	DDX_Control(pDX, IDC_STATUS_CODE, m_StatusCode);
	DDX_Control(pDX, IDC_MEASUREMENT_COUNT, m_MeasurementCount);
	DDX_Control(pDX, IDC_BUTTON_SAVE, m_SaveButton);
	DDX_Control(pDX, IDC_MANUFACTURER, m_ManufacturerName);
	DDX_Control(pDX, IDC_MODEL, m_ProductName);
	DDX_Control(pDX, IDC_HW_VERSION, m_HwVersion);
	DDX_Control(pDX, IDC_FW_VERSION, m_FwVersion);
	DDX_Control(pDX, IDC_SERIAL_NUMBER, m_SerialNumber);
}

BEGIN_MESSAGE_MAP(CLRDemoCodeDlg, CDialog)
	//{{AFX_MSG_MAP(CLRDemoCodeDlg)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_BN_CLICKED(IDC_START, OnStart)
	ON_WM_TIMER()
	//}}AFX_MSG_MAP
	ON_WM_DEVICECHANGE()
	ON_BN_CLICKED(IDC_BUTTON_SAVE, &CLRDemoCodeDlg::OnBnClickedButtonSave)
	ON_CBN_SELCHANGE(IDC_DISTANCE_DISPLAY, &CLRDemoCodeDlg::OnCbnSelchangeDistanceDisplay)
	ON_CBN_SELCHANGE(IDC_MEASUREMENT_MODE, &CLRDemoCodeDlg::OnCbnSelchangeMeasurementMode)
	ON_EN_CHANGE(IDC_INTERVAL, &CLRDemoCodeDlg::OnEnChangeInterval)
	ON_CBN_SELCHANGE(IDC_INTERVAL_UNITS, &CLRDemoCodeDlg::OnCbnSelchangeIntervalUnits)
	ON_CBN_SELCHANGE(IDC_TRIGGER, &CLRDemoCodeDlg::OnCbnSelchangeTrigger)
	ON_BN_CLICKED(IDC_KB_EMULATION, &CLRDemoCodeDlg::OnBnClickedKbEmulation)
	ON_BN_CLICKED(IDC_DO_DOUBLE_MEASUREMENTS, &CLRDemoCodeDlg::OnBnClickedDoDoubleMeasurements)
	ON_BN_CLICKED(IDC_DONT_FILTER_ERRORS, &CLRDemoCodeDlg::OnBnClickedDontFilterErrors)
	ON_BN_CLICKED(IDC_ONLY_SEND_CHANGES, &CLRDemoCodeDlg::OnBnClickedOnlySendChanges)
	ON_BN_CLICKED(IDC_IGNORE_OFF_TO_ON, &CLRDemoCodeDlg::OnBnClickedIgnoreOffToOn)
	ON_BN_CLICKED(IDC_IGNORE_ON_TO_OFF, &CLRDemoCodeDlg::OnBnClickedIgnoreOnToOff)
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CLRDemoCodeDlg message handlers

BOOL CLRDemoCodeDlg::OnInitDialog()
{
	CDialog::OnInitDialog();

	// Add "About..." menu item to system menu.

	// IDM_ABOUTBOX must be in the system command range.
	ASSERT((IDM_ABOUTBOX & 0xFFF0) == IDM_ABOUTBOX);
	ASSERT(IDM_ABOUTBOX < 0xF000);

	CMenu* pSysMenu = GetSystemMenu(FALSE);
	if (pSysMenu != NULL)
	{
		CString strAboutMenu;
		strAboutMenu.LoadString(IDS_ABOUTBOX);
		if (!strAboutMenu.IsEmpty())
		{
			pSysMenu->AppendMenu(MF_SEPARATOR);
			pSysMenu->AppendMenu(MF_STRING, IDM_ABOUTBOX, strAboutMenu);
		}
	}

	// Set the icon for this dialog.  The framework does this automatically
	//  when the application's main window is not a dialog
	SetIcon(m_hIcon, TRUE);			// Set big icon
	SetIcon(m_hIcon, FALSE);		// Set small icon

	// Disable all controls until connection
	DisableRangefinderControls();
	
	// Explicitely load the AtLR library.
	hLib = LoadLibrary(AT_USBHID_DLL);
    if (hLib == NULL)        
    {
        handleError(GetLastError());
        return 0;
    }

    // Get USB HID library functions addresses.    
    if (loadFuncPointers(hLib)==NULL) {        
		AfxMessageBox( "Could not get USB HID library functions addresses",MB_ICONSTOP,0);
        return 0;
	}

	// try to connect Device
    ConnectDevice();

	DYNCALL(hidRegisterDeviceNotification)((m_hWnd));	
	
	return TRUE;  // return TRUE  unless you set the focus to a control
}

void CLRDemoCodeDlg::OnSysCommand(UINT nID, LPARAM lParam)
{
	if ((nID & 0xFFF0) == IDM_ABOUTBOX)
	{
		CAboutDlg dlgAbout;
		dlgAbout.DoModal();
	}
	else
	{
		CDialog::OnSysCommand(nID, lParam);
	}
}

// If you add a minimize button to your dialog, you will need the code below
//  to draw the icon.  For MFC applications using the document/view model,
//  this is automatically done for you by the framework.

void CLRDemoCodeDlg::OnPaint() 
{
	if (IsIconic())
	{
		CPaintDC dc(this); // device context for painting

		SendMessage(WM_ICONERASEBKGND, (WPARAM) dc.GetSafeHdc(), 0);

		// Center icon in client rectangle
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// Draw the icon
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CDialog::OnPaint();
	}
}

// The system calls this to obtain the cursor to display while the user drags
//  the minimized window.
HCURSOR CLRDemoCodeDlg::OnQueryDragIcon()
{
	return (HCURSOR) m_hIcon;
}

void CLRDemoCodeDlg::DisableRangefinderControls()
{
	LrfRun = false ;

	// Disable all controls while disconnected
	m_Distance.EnableWindow(false);
	m_Start.EnableWindow(false);

	m_ControlGroup.EnableWindow(false);
	m_DistanceDisplay.EnableWindow(false);
	m_MeasurementMode.EnableWindow(false);
	m_Interval.EnableWindow(false);
	m_IntervalSpin.EnableWindow(false);
	m_IntervalUnits.EnableWindow(false);
	m_Trigger.EnableWindow(false);
	m_KbEmulation.EnableWindow(false);
	m_DoDoubleMeasurements.EnableWindow(false);
	m_DontFilterErrors.EnableWindow(false);
	m_OnlySendChanges.EnableWindow(false);
	m_IgnoreOffToOn.EnableWindow(false);
	m_IgnoreOnToOff.EnableWindow(false);
	m_StaticDistanceDisplay.EnableWindow(false);
	m_StaticMeasurementMode.EnableWindow(false);
	m_StaticInterval.EnableWindow(false);
	m_StaticTrigger.EnableWindow(false);
	m_SaveButton.EnableWindow(false);

	// Change Led Text
	m_Start.SetWindowText(_T("Start"));
	
	m_ManufacturerName.SetWindowText(_T("Disconnected"));
	m_ProductName.SetWindowText(_T(" "));
	m_HwVersion.SetWindowText(_T(" "));
	m_FwVersion.SetWindowText(_T(" "));
	m_SerialNumber.SetWindowText(_T(" "));

	m_StatusCode.SetWindowText(_T("---"));
	m_MeasurementCount.SetWindowText(_T("0"));
	MeasurementCounter = 0;

	CDialog::KillTimer(1);

	IsConnected = false;
	ReceivedInitialConfig = false;
}

/*---------------------------------------------------------------------------

FUNCTION: ConnectDevice

PURPOSE: Connect Device using Current Vid and Pid
         if connection is succefull, change status to Connected
		 if connection fail Status is set to disconnected

PARMATERS:

	
RETURN:
   

COMMENTS:
  
---------------------------------------------------------------------------*/
void CLRDemoCodeDlg::ConnectDevice()
{
	// Open our USB device.
	if (DYNCALL(findHidDevice)(DEFAULT_VID, DEFAULT_PID)) {
		EnableRangefinderControls();		
	}
    else {
        DisableRangefinderControls();
    }
}


void CLRDemoCodeDlg::EnableRangefinderControls()
{
	
	// Enable all controls while connected
	m_Distance.EnableWindow(true);

	m_Start.EnableWindow(true);

	m_ControlGroup.EnableWindow(true);
	m_DistanceDisplay.EnableWindow(true);
	m_MeasurementMode.EnableWindow(true);
	m_Interval.EnableWindow(true);
	m_IntervalSpin.EnableWindow(true);
	m_IntervalUnits.EnableWindow(true);
	m_Trigger.EnableWindow(true);
	m_KbEmulation.EnableWindow(true);
	m_DoDoubleMeasurements.EnableWindow(true);
	m_DontFilterErrors.EnableWindow(true);
	m_OnlySendChanges.EnableWindow(true);
	m_IgnoreOffToOn.EnableWindow(true);
	m_IgnoreOnToOff.EnableWindow(true);
	m_StaticDistanceDisplay.EnableWindow(true);
	m_StaticMeasurementMode.EnableWindow(true);
	m_StaticInterval.EnableWindow(true);
	m_StaticTrigger.EnableWindow(true);
	m_SaveButton.EnableWindow(true);

	m_Distance.SetWindowText(_T("0"));
	m_Start.SetWindowText(_T("Start"));
	m_DistanceDisplay.SetCurSel(1);
	m_MeasurementMode.SetCurSel(0);
	m_Interval.SetWindowText(_T("1"));
	m_IntervalUnits.SetCurSel(0);
	m_Trigger.SetCurSel(0);

	m_StatusCode.SetWindowText(_T("---"));
	m_MeasurementCount.SetWindowText(_T("0"));
	MeasurementCounter = 0;

	SetTimer(1,50,0);
	IsConnected = true ;
	GetControlValues();
}

/*---------------------------------------------------------------------------

FUNCTION: OnDeviceChange

PURPOSE: This function is call each time a status change for a device using 
            ON_WM_DEVICECHANGE()
         The function will check if this our device change it status :
		 There is 2 important type of event :
			DBT_DEVICEARRIVAL : in this case, we try to connect a device using 
			   current VID and PID
			DBT_DEVICEREMOVECOMPLETE : if our device as been deconnected,
			  we close the device properly using closeDevice()

		if OnDeviceChange is called by another device nothing is done


PARMATERS: UINT nEventType : Event Id
           DWORD dwData : data associated to the Event

RETURN:

COMMENTS:
  
---------------------------------------------------------------------------*/
BOOL CLRDemoCodeDlg::OnDeviceChange(UINT nEventType, DWORD dwData)
{
	
	int isOurDevice;
	switch(nEventType)
		{		
			case DBT_DEVICEARRIVAL :
				isOurDevice=DYNCALL(isMyDeviceNotification(dwData));
				if(isOurDevice&&IsConnected) {
					OutputDebugString(">>> Our Device Already Connected.\n");	
				}
				else {
					// Connect Only if status is disconnected
					OutputDebugString(">>> A device has been inserted and is now available.\n");
					ConnectDevice();
				}
				
				break;
				
			case DBT_DEVICEREMOVECOMPLETE :
				isOurDevice=DYNCALL(isMyDeviceNotification(dwData));
				if(IsConnected&&isOurDevice) {
					// Close Connection only once
					DisableRangefinderControls();
					DYNCALL(closeDevice());
					OutputDebugString(">>> A device has been removed.\n");
				}
				
				break;			
			default :				
				OutputDebugString(">>> OnDeviceChange : default\n");
				break;
		}		
	return TRUE;
}

/*---------------------------------------------------------------------------

FUNCTION: OnTimer

PURPOSE: This function allow us to call the check if a new data as been recieved
          if yes, the buffer imfarmation are display using AddRecievedData
		The Timer for this function must be killed if Connection 
		 is lost uasing function : CDialog::KillTimer(1);
		If a device is connected, the timer must be set using :
		  SetTimer(1,50,0);

PARMATERS:
  nIDEvent	  -	   Timer identifier
  	
RETURN:
   
COMMENTS:
  
---------------------------------------------------------------------------*/
void CLRDemoCodeDlg::OnTimer(UINT nIDEvent) 
{
	UCHAR sbuffer[255];
	CString str;
	unsigned int Millimeters;
	float Feet, Inches, Meters, Centimeters;
	unsigned int ConfigValue;
	unsigned int Offset, i;
	UCHAR *pProductInfo;

	if(DYNCALL(readData(sbuffer))!=0) {
		if (sbuffer[0] == STS_MEASUREMENT_DATA) {
			++MeasurementCounter;

			// Raw data from LR3 is always millimeters (16 bit unsigned int in first two bytes of sbuffer)
			Millimeters = (sbuffer[2] << 8) + sbuffer[1];

			switch (m_DistanceDisplay.GetCurSel()) {
				case 0:			// Feet and inches
					Inches = (float)Millimeters / (float)25.4;
					Feet = (float)floor((Inches / 12));
					Inches -= Feet * 12;
					str.Format(_T("%2.0f\' %2.1f\""), Feet, Inches);
					break;
				case 1:			// Meters
					Meters = (float)Millimeters / (float)1000;
					str.Format(_T("%2.3f m"), Meters);
					break;
				case 2:			// Feet
					Feet = ((float)Millimeters / (float)25.4) / (float)12;
					str.Format(_T("%2.2f\'"), Feet);
					break;
				case 3:			// Inches
					Inches = (float)Millimeters / (float)25.4;
					str.Format(_T("%4.1f\""), Inches);
					break;
				case 4:			// Centimeters
					Centimeters = (float)Millimeters / (float)10;
					str.Format(_T("%4.1f cm"), Centimeters);
					break;
				default:		// Error
					str = _T("Error");
					break;
			}

			m_Distance.SetWindowText(str);

			str.Format(_T("%04X %02X %02X"), ((sbuffer[4] << 8) + sbuffer[5]), sbuffer[6], sbuffer[7]);
			m_StatusCode.SetWindowText(str);

			str.Format(_T("%d"), MeasurementCounter);
			m_MeasurementCount.SetWindowText(str);
		}
		else if (sbuffer[0] == STS_CONFIG_DATA) {
			ConfigValue = sbuffer[1] + (sbuffer[2] << 8);

			m_DistanceDisplay.SetCurSel((ConfigValue >> 0) & 0x07);		// 3 bits from bit[2:0]
			m_MeasurementMode.SetCurSel((ConfigValue >> 3) & 0x03);		// 2 bits from bit[4:3]
			m_IntervalUnits.SetCurSel  ((ConfigValue >> 5) & 0x03);		// 2 bits from bit[6:5]
			m_Trigger.SetCurSel        ((ConfigValue >> 7) & 0x03);		// 2 bits from bit[8:7]

			str.Format(_T("%5d"), (sbuffer[3] + (sbuffer[4] << 8)));
			m_Interval.SetWindowText(str); //IntervalValue

			m_KbEmulation.SetCheck         ((ConfigValue >> 9) & 0x01);		// 1 bit from bit[9]
			m_DoDoubleMeasurements.SetCheck((ConfigValue >> 10) & 0x01);	// 1 bit from bit[10]
			m_DontFilterErrors.SetCheck    ((ConfigValue >> 11) & 0x01);	// 1 bit from bit[11]
			m_OnlySendChanges.SetCheck     ((ConfigValue >> 12) & 0x01);	// 1 bit from bit[12]
			m_IgnoreOffToOn.SetCheck       ((ConfigValue >> 13) & 0x01);	// 1 bit from bit[13]
			m_IgnoreOnToOff.SetCheck       ((ConfigValue >> 14) & 0x01);	// 1 bit from bit[14]

			ReceivedInitialConfig = true;
			GetProductInfo(0);  // Start filling the product info structure 
		}
		else if (sbuffer[0] == STS_PRODUCT_INFO) {
			Offset = sbuffer[1];
			if (Offset == 0)
				memset(&ProductInfo, 0, sizeof(PRODUCT_INFO));

			pProductInfo = (UCHAR *)&ProductInfo;
			pProductInfo += Offset;
			for (i=2; i<8; i++) {				// Grab the 6 bytes of product info from packet
				if (Offset < sizeof(PRODUCT_INFO))
					*pProductInfo++ = sbuffer[i];
				++Offset;
			}

			if (Offset < sizeof(PRODUCT_INFO))
				GetProductInfo(Offset);
			else { // If this is the final ProductInfo packet
				m_ManufacturerName.SetWindowText(_T(ProductInfo.ManufacturerName));

				str.Format(_T("Model: %s"), ProductInfo.ProductName);
				m_ProductName.SetWindowText(str);
				str.Format(_T("Board: %s"), ProductInfo.HardwareVersion);
				m_HwVersion.SetWindowText(str);

				str.Format(_T("Firmware: %s"), ProductInfo.FirmwareVersion);
				m_FwVersion.SetWindowText(str);
				str.Format(_T("Serial #: %s"), ProductInfo.SerialNumber);
				m_SerialNumber.SetWindowText(str);
			}
		}
	}	
	CDialog::OnTimer(nIDEvent);
}


/*---------------------------------------------------------------------------

FUNCTION: OnCancel

PURPOSE: On one cancel if device is connected, this one is disconnected.
		 the application is Unregister from the device notification table using
		  hidUnregisterDeviceNotification(m_hWnd)

PARMATERS:
  	
RETURN:
   
COMMENTS:
  
---------------------------------------------------------------------------*/
void CLRDemoCodeDlg::OnCancel() 
{
	if (IsConnected) //if our device is attached
	{
		DYNCALL(closeDevice());//close all handles
		CDialog::KillTimer(1);//close the timer
	}
	DYNCALL(hidUnregisterDeviceNotification(m_hWnd));
	
	FreeLibrary(hLib);

	CDialog::OnCancel();
}


unsigned int CLRDemoCodeDlg::BuildConfigValue (void)
{
	// Config data definition
	//
	// Distance Display: Bits[2:0]
	//     000 = Feet & Inches
	//     001 = Meters
	//     010 = Feet
	//     011 = Inches
	//     100 = Centimeters
	//     101 - 111 = Reserved
	// Measurement Mode: Bits[4:3]
	//      00 = Continuous
	//      01 = Single
	//      10 = Interval
	//      11 = Reserved
	// Interval Units: Bits[6:5]
	//      00 = Seconds
	//      01 = Minutes
	//      10 = Hours
	//      11 = Reserved
	// Trigger: Bits[8:7]
	//      00 = "Start" Button
	//      01 = Caps Lock
	//      10 = Num Lock
	//      11 = Scroll Lock
	// Keyboard Enulation Mode: Bit 9  (0 = Disabled / 1 = Enabled)
	// Do Double Measurements:  Bit 10 (0 = Disabled / 1 = Enabled)
	// Don't Filter Errors:     Bit 11 (0 = Disabled (filter on) / 1 = Enabled (filter off))
	// Only Send Changes:       Bit 12 (0 = Disabled / 1 = Enabled)
	// Ignore Off-to-On:        Bit 13 (0 = Don't ignore / 1 = Ignore)
	// Ignore On-to-Off:        Bit 14 (0 = Don't ignore / 1 = Ignore)
	// Laser Rangefinder Run:   Bit 15 (0 = Not running / 1 = Running)
	// Interval: Bits[31:16]
	//     16 Bit Unsigned Integer

	unsigned int ConfigValue;

	ConfigValue = 0;
	ConfigValue |= (m_DistanceDisplay.GetCurSel() & 0x07) << 0;		// 3 bits into bit[2:0]
	ConfigValue |= (m_MeasurementMode.GetCurSel() & 0x03) << 3;		// 2 bits into bit[4:3]
	ConfigValue |= (m_IntervalUnits.GetCurSel()   & 0x03) << 5;		// 2 bits into bit[6:5]
	ConfigValue |= (m_Trigger.GetCurSel()         & 0x03) << 7;		// 2 bits into bit[8:7]

	ConfigValue |= ((m_KbEmulation.GetCheck()         == BST_CHECKED) ? 1 : 0) << 9;	// 1 bit into bit[9]
	ConfigValue |= ((m_DoDoubleMeasurements.GetCheck()== BST_CHECKED) ? 1 : 0) << 10;	// 1 bit into bit[10]
	ConfigValue |= ((m_DontFilterErrors.GetCheck()    == BST_CHECKED) ? 1 : 0) << 11;	// 1 bit into bit[11]
	ConfigValue |= ((m_OnlySendChanges.GetCheck()     == BST_CHECKED) ? 1 : 0) << 12;	// 1 bit into bit[12]
	ConfigValue |= ((m_IgnoreOffToOn.GetCheck()       == BST_CHECKED) ? 1 : 0) << 13;	// 1 bit into bit[13]
	ConfigValue |= ((m_IgnoreOnToOff.GetCheck()       == BST_CHECKED) ? 1 : 0) << 14;	// 1 bit into bit[14]

	ConfigValue |= (LrfRun ? 1 : 0) << 15;	// 1 bit into bit[15]
	
	CString Str;
	m_Interval.GetWindowText(Str);
	ConfigValue |= (atoi(Str) & 0x0000FFFF) << 16;		// 16 bits into bit[31:16]

	return (ConfigValue);
}


void CLRDemoCodeDlg::OnBnClickedButtonSave()
{
	UCHAR CmdBuf[8];

	if (IsConnected) {	// If our device is attached
		CmdBuf[0] = CMD_WRITE_CONFIG;
		CmdBuf[1] = 0;
		CmdBuf[2] = 0;
		CmdBuf[3] = 0;
		CmdBuf[4] = 0;
		CmdBuf[5] = 0;
		CmdBuf[6] = 0;
		CmdBuf[7] = 0;

		DYNCALL(writeData)(&CmdBuf[0]);

		AfxMessageBox( "Rangefinder control values were successfully written to rangerinder's non-volatile memory.",MB_ICONINFORMATION,0);
	}
	else
		AfxMessageBox( "Could not write control values to rangefinder.",MB_ICONINFORMATION,0);
}


void CLRDemoCodeDlg::GetControlValues (void)
{
	UCHAR CmdBuf[8];
	if (IsConnected) {	// If our device is attached

		CmdBuf[0] = CMD_GET_CONFIG;
		CmdBuf[1] = 0;
		CmdBuf[2] = 0;
		CmdBuf[3] = 0;
		CmdBuf[4] = 0;
		CmdBuf[5] = 0;
		CmdBuf[6] = 0;
		CmdBuf[7] = 0;

		DYNCALL(writeData)(&CmdBuf[0]);
		// In a few milliseconds the rangefinder will
		// respond with an Out report packet that will
		// be read by OnTimer function
	}
}

void CLRDemoCodeDlg::GetProductInfo(unsigned int Offset)
{
	UCHAR CmdBuf[8];
	if (IsConnected) {	// If our device is attached

		CmdBuf[0] = CMD_GET_PRODUCT_INFO;
		CmdBuf[1] = Offset;
		CmdBuf[2] = 0;
		CmdBuf[3] = 0;
		CmdBuf[4] = 0;
		CmdBuf[5] = 0;
		CmdBuf[6] = 0;
		CmdBuf[7] = 0;

		DYNCALL(writeData)(&CmdBuf[0]);
		// In a few milliseconds the rangefinder will respond
		// with an Out report packet that will be read by OnTimer function
	}
}

void CLRDemoCodeDlg::UpdateControlValues (void)
{
	unsigned int ConfigValue;
	UCHAR CmdBuf[8];

	// If our device is attached and we have already read it's config data
	if ((IsConnected) && (ReceivedInitialConfig)) {
		ConfigValue = BuildConfigValue();

		CmdBuf[0] = CMD_SET_CONFIG;
		CmdBuf[1] = (UCHAR)(ConfigValue);
		CmdBuf[2] = (UCHAR)(ConfigValue >> 8);
		CmdBuf[3] = (UCHAR)(ConfigValue >> 16);
		CmdBuf[4] = (UCHAR)(ConfigValue >> 24);
		CmdBuf[5] = 0;
		CmdBuf[6] = 0;
		CmdBuf[7] = 0;

		DYNCALL(writeData)(&CmdBuf[0]);
	}
}

void CLRDemoCodeDlg::OnCbnSelchangeDistanceDisplay()
{
	UpdateControlValues();
}

void CLRDemoCodeDlg::OnCbnSelchangeMeasurementMode()
{
	switch (m_MeasurementMode.GetCurSel()) {
		case 0:		// Continuous mode
			if (LrfRun == (boolean)true)
				m_Start.SetWindowText(_T("Stop"));
			else m_Start.SetWindowText(_T("Start"));
			UpdateControlValues();
			break;
		case 1:		// Single mode
		case 2:		// Interval mode
			m_Start.SetWindowText(_T("Start"));		
			LrfRun = false;
			UpdateControlValues();
			break;
	}
}

void CLRDemoCodeDlg::OnEnChangeInterval()
{
	if (::IsWindow(m_Interval.m_hWnd))	// Needed to avoid an assert failure inside GetWindowText
		UpdateControlValues();
}

void CLRDemoCodeDlg::OnCbnSelchangeIntervalUnits()
{
	UpdateControlValues();
}

void CLRDemoCodeDlg::OnCbnSelchangeTrigger()
{
	UpdateControlValues();
}

void CLRDemoCodeDlg::OnBnClickedKbEmulation()
{
	UpdateControlValues();
}

void CLRDemoCodeDlg::OnBnClickedDoDoubleMeasurements()
{
	UpdateControlValues();
}

void CLRDemoCodeDlg::OnBnClickedDontFilterErrors()
{
	UpdateControlValues();
}

void CLRDemoCodeDlg::OnBnClickedOnlySendChanges()
{
	UpdateControlValues();
}

void CLRDemoCodeDlg::OnBnClickedIgnoreOffToOn()
{
	UpdateControlValues();
}


void CLRDemoCodeDlg::OnBnClickedIgnoreOnToOff()
{
	UpdateControlValues();
}


void CLRDemoCodeDlg::OnStart() 
{
	switch (m_MeasurementMode.GetCurSel()) {
		case 0:		// Continuous mode
		case 2:		// Interval mode
			if (LrfRun == false) {
				LrfRun = true;
				m_Start.SetWindowText(_T("Stop"));		
			}
			else {
				LrfRun = false;
				m_Start.SetWindowText(_T("Start"));		
			}
			UpdateControlValues();
			break;
		case 1:		// Single mode
			m_Start.SetWindowText(_T("Start"));		
			LrfRun = true;
			UpdateControlValues();
			LrfRun = false;
			UpdateControlValues();
			break;
	}
}




