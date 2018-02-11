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

#include "afxwin.h"
#include "afxcmn.h"
#if !defined(AFX_LRDEMOCODEDLG_H__F9AA5915_C6E4_4CF0_95B7_2C6EB1964BAB__INCLUDED_)
#define AFX_LRDEMOCODEDLG_H__F9AA5915_C6E4_4CF0_95B7_2C6EB1964BAB__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

/////////////////////////////////////////////////////////////////////////////
// CLRDemoCodeDlg dialog

class CLRDemoCodeDlg : public CDialog
{
// Construction
public:
	void AddRecievedData(CString NewData);
	
	void ConnectDevice();
	void DisableRangefinderControls();
	void EnableRangefinderControls();
	unsigned int BuildConfigValue (void);
	void GetControlValues (void);
	void GetProductInfo(unsigned int Offset);
	void UpdateControlValues (void);
	boolean LrfRun;
	boolean IsConnected;
	boolean ReceivedInitialConfig;
	HINSTANCE hLib;  
	unsigned int MeasurementCounter;
	PRODUCT_INFO ProductInfo;

	CLRDemoCodeDlg(CWnd* pParent = NULL);	// standard constructor

// Dialog Data
	//{{AFX_DATA(CLRDemoCodeDlg)
	enum { IDD = IDD_LRDEMOCODE_DIALOG };
	//CButton	m_FwUpgrade;
	//CListBox	m_RecievedData;
	CButton	m_Start;
	//}}AFX_DATA

	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CLRDemoCodeDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	HICON m_hIcon;

	// Generated message map functions
	//{{AFX_MSG(CLRDemoCodeDlg)
	virtual BOOL OnInitDialog();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	afx_msg void OnStart();
	afx_msg void OnTimer(UINT nIDEvent);
	//afx_msg void OnFwUpgrade();
	virtual void OnCancel();
	afx_msg void OnButtonVidPid();
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
	BOOL OnDeviceChange(UINT nEventType, DWORD dwData);
public:
	CStatic m_Distance;
	CComboBox m_DistanceDisplay;
	CComboBox m_MeasurementMode;
	CEdit m_Interval;
	CSpinButtonCtrl m_IntervalSpin;
	CComboBox m_IntervalUnits;
	CComboBox m_Trigger;
	CButton m_KbEmulation;
	CButton m_DoDoubleMeasurements;
	CButton m_DontFilterErrors;
	CButton m_OnlySendChanges;
	CButton m_IgnoreOffToOn;
	CButton m_IgnoreOnToOff;
	CStatic m_ControlGroup;
	CStatic m_StaticDistanceDisplay;
	CStatic m_StaticMeasurementMode;
	CStatic m_StaticInterval;
	CStatic m_StaticTrigger;
	CStatic m_StatusCode;
	CStatic m_MeasurementCount;
	CButton m_SaveButton;
	CStatic m_ManufacturerName;
	CStatic m_ProductName;
	CStatic m_HwVersion;
	CStatic m_FwVersion;
	CStatic m_SerialNumber;

	afx_msg void OnBnClickedButtonSave();
	afx_msg void OnCbnSelchangeDistanceDisplay();
	afx_msg void OnCbnSelchangeMeasurementMode();
	afx_msg void OnEnChangeInterval();
	afx_msg void OnCbnSelchangeIntervalUnits();
	afx_msg void OnCbnSelchangeTrigger();
	afx_msg void OnBnClickedKbEmulation();
	afx_msg void OnBnClickedDoDoubleMeasurements();
	afx_msg void OnBnClickedDontFilterErrors();
	afx_msg void OnBnClickedOnlySendChanges();
	afx_msg void OnBnClickedIgnoreOffToOn();
	afx_msg void OnBnClickedIgnoreOnToOff();
};

//{{AFX_INSERT_LOCATION}}
// Microsoft Visual C++ will insert additional declarations immediately before the previous line.

#endif // !defined(AFX_LRDEMOCODEDLG_H__F9AA5915_C6E4_4CF0_95B7_2C6EB1964BAB__INCLUDED_)
