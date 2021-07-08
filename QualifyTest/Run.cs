using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Lead.Detect.BussinessModel;
using Lead.Detect.Global;
using Lead.Detect.Interfaces;
using Lead.Detect.Interfaces.Communicate;
using Lead.Detect.LogHelper;
using Lead.Detect.DataOutputCommon;

public class RunState
{
	private IPrim _vpAssemble=null;
	private IPrim _vpDetector=null;
	
	#region Elements define
	private IEle _eleBtnLock = null;
	
	private IEle _eleDiUplineEntry = null;
	private IEle _eleDiUplineExit = null;
	private IEle _eleDiUplineWork = null;
	private IEle _eleDiUpLineCheck = null;
	
	private IEle  _eleAxisAbove = null;
	private IEle  _eleAxisBelow = null;
	private IEle _eleAxisX = null;
	private IEle _eleAxisY = null;
	private IEle _eleAxisZ = null;
	//private IEle _eleAxisU = null;
	private IEle _eleCylBlock = null;
	private IEle _eleCylRaise = null;
	private IEle _eleCylPush = null;
	private IEle _eleCylScrew=null;	
	private IEle _eleVac=null;
	#endregion
	
	#region Public variables define
	public bool ErrFlag = false;
	public bool AutoRun = false;
	public bool Continue = false;
	
	public bool TaskError = false;
	public short TaskErrLv = -1;
	public string TaskErrMsg = "";
	public short CurStep = -1;
	public short LastStep = -1;
	public short TargetStep = -1;
	
	public bool QyReady = false;
	public bool TaskCancel = false;
	public int TaskResult = -1;
	
	public string CurRep = "";
	#endregion
	
	#region Private variables define
	private bool _firstFlag = true;
	private string TaskName = "";
	private DialogResult _mBox1Result = new DialogResult();
	private ITask _lastTask = null;
	
	private bool Capture1=false;
	private bool Capture2=false;
	private double CapResultX=0;
	private double CapResultY=0;
	private double CapResultR=0;
	private bool Dector=false;
	
	private DateTime _trayDockTime=DateTime.Now;
	private DateTime _autoStartTime=DateTime.Now;
	private DateTime _autoEndTime=DateTime.Now;
	
	private bool DryRunMode=false;
	private string ModeString="";
	
	private FileStream CTLogfs,CTTimefs,AllDatafs;
	private StreamWriter CTLogsw,CTTimesw,AllDatasw;
	private string CTLogPath="D:\\TotalCTFile-Qualify.txt";
	private string CTTimePath="D:\\TotalTimeFile-Qualify.txt";
	private string AllRunDataPath="D:\\AllData\\Qualify\\";
	private string AllRunDataFullPath="";
	private string VisionProDataFullPath = "";
	private string AutoTaskString="";
	private string VisionProString = "";
	private string UplineString="";
	private string CurFileDate="";
	private string CurVisionProDate = "";
	private string Pic1= "";
	private string Pic3 = "";
	private int count = 0;
	private bool _ngFlag = false;
	
	private QualifyDataOutPutControl _qy = null;
	private string _qy_TrayName = null;
	private string _qy_OperateName = null;
	private string _qy_PartNO = null;
	private string _qy_PartName = null;
	private string _qy_TaskName = null;
		
	#endregion
	
	#region //LM
	private string DSN = "50062";
	private string TrayID = "NA";
	private string FactoryID = "NA";
	private string ProjectName = "NA";
	private string BuildConfig = "NA";
	private string OperatorName = "NA";
	private string StartTime = "NA";
	#endregion
	public int Exec(ITask task)
	{
		int iRet = 0;
		_firstFlag = true;
		while(task.TaskRunStat != TaskRunState.Stop)
		{
			Thread.Sleep(3);
			if (_firstFlag) 
			{
				_firstFlag = false;
				
				count = 0;
				CurFileDate="Screw1-"+GetDayString(DateTime.Now);
				CurVisionProDate = "VisionPro5Data-" + GetDayString(DateTime.Now);
				AllRunDataFullPath=AllRunDataPath + CurFileDate;
				VisionProDataFullPath = AllRunDataPath + CurVisionProDate;
				
				if(File.Exists(AllRunDataFullPath)==false)
				{
					CreateNewCSV(AllRunDataFullPath);
				}
				if(File.Exists(VisionProDataFullPath) == false)
				{
					CreateNewVisionCSV(VisionProDataFullPath);
				}
				
				DataOutputManager.Instance.ClientDataOutputManagerUpdateVar("UpLine_Vc",((IEleAxisLine)_eleAxisAbove).DefaultParams.Velocity.ToString());
				DataOutputManager.Instance.ClientDataOutputManagerUpdateVar("DnLine_Vc",((IEleAxisLine)_eleAxisBelow).DefaultParams.Velocity.ToString());
				DataOutputManager.Instance.ClientDataOutputManagerUpdateVar("Axis_X_Vc",((IEleAxisLine)_eleAxisX).DefaultParams.Velocity.ToString());
				DataOutputManager.Instance.ClientDataOutputManagerUpdateVar("Axis_Y_Vc",((IEleAxisLine)_eleAxisY).DefaultParams.Velocity.ToString());
				DataOutputManager.Instance.ClientDataOutputManagerUpdateVar("Axis_Z_Vc",((IEleAxisLine)_eleAxisZ).DefaultParams.Velocity.ToString());
				//DataOutputManager.Instance.ClientDataOutputManagerUpdateVar("Axis_U_Vc",((IEleAxisLine)_eleAxisU).DefaultParams.Velocity.ToString());
			
				DataInfoManager.Instance.DataInfoSetVal("QyReady", false);
				DataInfoManager.Instance.DataInfoSetVal("TaskCancel", false);
				DataInfoManager.Instance.DataInfoSetVal("TaskResult", -1);
				DataInfoManager.Instance.DataInfoSetVal("QyCount",count);
				
				((IEleAxisLine)_eleAxisAbove).MC_Stop();
				
				((IEleCyldLine)_eleCylBlock).CyldRetract();
				((IEleCyldLine)_eleCylRaise).CyldRetract();
				((IEleCyldLine)_eleCylScrew).CyldRetract();
				((IEleCyldLine)_eleCylPush).CyldRetract();
				
				_qy = DataOutputManager.Instance.QualifyControl_;
				if(_qy!=null)
				{
					_qy_TrayName = _qy.TrayNo;
					_qy_OperateName = _qy.OperatorName;
					_qy_PartNO = _qy.PartNo;
					_qy_PartName = _qy.PartName;
					_qy_TaskName = _qy.TaskName;
					
					int partNo = int.Parse(_qy_PartNO);
					DataInfoManager.Instance.DataInfoSetVal("QyPartNo",partNo);
					DataInfoManager.Instance.DataInfoSetVal("QyPartName",_qy_PartName);
				}
				SetStep(1, "Wait for the button of 'Tray Lock'");
			}
			
		RunStep1: //Check Lock Mode
			#region
			if (CheckAutoRun() && (CurStep == 1)) 
			{
				if (((IEleDI)_eleBtnLock).Read())
				{
					SetTaskError(true, AlarmLv.Error, 1, "Please put in Unlock mode!", task);
					_mBox1Result = MessageBox.Show("Please put in Unlock mode.","Error!", MessageBoxButtons.RetryCancel);
					SetStep(1001);
				}
				else
				{
					SetStep(2,"Waiting for the button of 'Tray Lock'");
				}
			}
			#endregion			
		RunStep1001:
			#region
			if (CheckAutoRun() && (CurStep == 1001)) 
			{
				if (_mBox1Result == DialogResult.Retry)
				{
					SetStep(1, "Please put in Unlock mode.");
				}
				else
				{
					SetStep(100, "Task end...");
				}
			}
			#endregion			
		RunStep2:  //Check Tray is Lock or not
			#region
			if (CheckAutoRun() && (CurStep == 2)) 
			{
				if (_qy.IsTrayLock)
				{
					((IVpAssemble)_vpAssemble).SetRunMode(2);
        			((IVpDetector)_vpDetector).SetRunMode(2);
					SetStep(201,"Kill the other subtask...");
				}
			}
			#endregion			
		RunStep201:  //Kill all task whithout "QualifyTest"
			#region
			if (CheckAutoRun() && (CurStep == 201)) 
			{
				foreach (var subTask in TasksManager.Instance.ListTask)
	            {
	                if (subTask.Enable && subTask.Name != "QualifyTest")
	                {
	                   	    //subTask.ITaskPause();
	                   	    subTask.TaskRunStat = TaskRunState.Stop;
	                   	    Thread.Sleep(200);
	                   	    subTask.ITaskStop();
	                   	    Thread.Sleep(500);
	                        if (subTask.ITaskDispose() != 0)
	                        {
	                            Log.Debug(5501003, "SubTask Stop failed!", LogClassification.Task, subTask.Name);
	                            SetStep(100,"Task end...");
	                            goto RunStepErr;
	                        }
	                }
	            }
				SetStep(202,"Start the selected task at qualify mode.");
			}
			#endregion				
		RunStep202:  //Start the selected task at qualify mode
			#region 
			if (CheckAutoRun() && (CurStep == 202)) 
			{
				ITask taskT = TasksManager.Instance.GetTaskByName(_qy_TaskName);
				if (taskT != null) 
				{
					DataOutputManager.Instance.ClientDataOutputManagerUpdateState("UpTaskName", taskT.Name);
					LogAdd("Start" + taskT.Name + "at qualify mode." ,AlarmLv.Info,202);
					taskT.TaskRunStat = TaskRunState.Running;
					taskT.RunMode = TaskRunMode.Qualify;
					Thread.Sleep(200);
					taskT.ITaskRun();
					DataInfoManager.Instance.DataInfoSetVal("QyMode",true);
				}
				_qy.SystemStatus = "run";
				SetStep(9,"Waiting for trigger signal...");
			}
			#endregion					
		RunStep9:  //Waiting the button of 'Run Cycle'
			#region
			if (CheckAutoRun() && (CurStep == 9))
			{
				var val=DataParamManager.Instance.DataParamGetParam("QyAuto");
				bool auto=(val!=null?(bool)val.DataVal:false);
				if (_qy.IsRunning||auto) 
				{
					LogAdd("Run an cycle at qualify mode", AlarmLv.Info,9);
					
					DataInfoManager.Instance.DataInfoSetVal("QyReady", true);
					
					_autoStartTime = DateTime.Now;
					SetStep(10, "The feeding action is completed, and the assembly action can be performed.");
				}
			}
			#endregion			
		RunStep10:  //The feeding action is completed, and the assembly action can be performed.
			#region
			if (CheckAutoRun() && (CurStep == 10))
			{
				TaskCancel = (bool)DataInfoManager.Instance.DataInfoGetVal("TaskCancel");
				if (TaskCancel) 
				{
					DataOutputManager.Instance.ClientDataOutputManagerUpdateState("Result", "NG");
					
					_autoEndTime = DateTime.Now;
					
					DataInfoManager.Instance.DataInfoSetVal("TotalAutoTime", (_autoEndTime-_autoStartTime).TotalSeconds);
					
					TaskResult = -1;
					
					SetStep(11, "Task Aborted, UpLine will go forward...");
					goto RunStep11;
				}
				
				QyReady = (bool)DataInfoManager.Instance.DataInfoGetVal("QyReady");
				if (!QyReady) 
				{
					_autoEndTime = DateTime.Now;
					
					DataInfoManager.Instance.DataInfoSetVal("TotalAutoTime", (_autoEndTime-_autoStartTime).TotalSeconds);
					
					TaskResult = CheckDryRun() ? 0 : (int)DataInfoManager.Instance.DataInfoGetVal("TaskResult");
					
					SetStep(11, "Assembly action completed, CylRaise will retract...");
				}
			}
			#endregion	
		RunStep11:  //Assembly action completed, CylRaise will retract
			#region
			if (CheckAutoRun() && (CurStep == 11))
			{
				string ct = (_autoEndTime-_autoStartTime).TotalSeconds.ToString("#.000");
				
				string taskRet = "";
				if (TaskResult == 0) 
				{
					taskRet = "OK";
				}
				else
				{
					taskRet = "NG";
				}
				string strCt = string.Format("Last work CT: {0}s", ct);
				string ctStr = string.Format("{0};{1};{2};", taskRet, ct, System.DateTime.Now.ToString("HH:mm:ss"));
				
				LogAdd(strCt, AlarmLv.Info, 999);
				
				DataOutputManager.Instance.ClientDataOutputManagerUpdateState("CT", ct.ToString());
				
				DataOutputManager.Instance.ClientAddPartInfo("1", ctStr);
				DataInfoManager.Instance.DataInfoSetVal("Auto_CT", ct);
		 		
				string _strQy = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};Qualify",_qy_TrayName,_qy_PartName,_qy_PartNO,_qy_OperateName,
		 		                              _trayDockTime.ToString("yyyyMMdd_HH:mm:ss"),
		 		                              _autoStartTime.ToString("yyyyMMdd_HH:mm:ss"),
		 		                              _autoEndTime.ToString("yyyyMMdd_HH:mm:ss"),
		 		                              (_autoEndTime-_autoStartTime).TotalSeconds.ToString("#.000"),
		 		                              taskRet);
		 		_qy.AddQualifyInfo("1",_strQy);
		 		//WriteCTLog((_trayOutTime-_trayInTime).TotalSeconds,(_trayReleaseTime-_trayDockTime).TotalSeconds);
		 		//WriteCTTime(_trayInTime,_trayOutTime,_trayDockTime,_trayReleaseTime);
		 		var val = DataInfoManager.Instance.DataInfoGetVal("Capture1");
		 		Capture1 = (val != null)?(bool)val:false;
		 		val = DataInfoManager.Instance.DataInfoGetVal("Capture2");
		 		Capture2 = (val != null)?(bool)val:false;
		 		val = DataInfoManager.Instance.DataInfoGetVal("Dector");
		 		Dector = (val != null)?(bool)val:false;
		 		val = DataInfoManager.Instance.DataInfoGetVal("CapResultX");
		 		CapResultX = (val != null)?(double)val:0;
		 		val = DataInfoManager.Instance.DataInfoGetVal("CapResultY");
		 		CapResultY = (val != null)?(double)val:0;
		 		val = DataInfoManager.Instance.DataInfoGetVal("CapResultR");
		 		CapResultR = (val != null)?(double)val:0;
		 		AutoTaskString=Capture1.ToString()+","+Capture2.ToString()+","+CapResultX.ToString()+","+CapResultY.ToString()+","+CapResultR.ToString()+","+Dector.ToString();
		 		ModeString="Qualify";
		 		
		 		UplineString= _qy_TrayName + "," + _qy_PartName + "," + "PartNo" + "," + _qy_OperateName + "," + _trayDockTime.ToString("yyyyMMdd_HH:mm:ss")+","+
		 			_autoStartTime.ToString("yyyyMMdd_HH:mm:ss") + "," + _autoEndTime.ToString("yyyyMMdd_HH:mm:ss") + "," + 
					 			ct+","+ taskRet + "," +
		 			"Qualify"+","+ ((IEleAxisLine)_eleAxisX).DefaultParams.Velocity.ToString()+","+((IEleAxisLine)_eleAxisY).DefaultParams.Velocity.ToString()+","+ ((IEleAxisLine)_eleAxisZ).DefaultParams.Velocity.ToString() + ",";
					 		
		 		try
		 		{
		 			string pass = "";
		 			if(Capture1)
		 			{
		 				Pic1 = "OK";
		 			}
		 			else
		 				Pic1 = "NG";
		 			if(Dector)
		 			{
		 				Pic3 = "OK";
		 				pass = "Pass";
		 			}
		 			else
		 			{
		 				Pic3 = "NG";
		 				pass = "Faile";
		 			}
		 			string CurStationName=DevRecipeManager.Instance.StationName;
		 			//VisionProString= count.ToString() +"," + _qy_TrayName +"," + DateTime.Now.ToString("yyyyMMdd_HH:mm:ss")+","+ModeString+"," + Pic1+","+CapResultX.ToString()+","+CapResultY.ToString()+","+CapResultR.ToString()+","+Pic3+","+taskRet;
		 			//WriteAllData(AutoTaskString,UplineString,VisionProString);
		 			string AllDataString = DSN+","+_qy_TrayName+","+FactoryID+","+ProjectName+","+BuildConfig+","+
		 							       _qy.OperatorName+","+_autoStartTime.ToString("yyyyMMdd_HH:mm:ss")+","+CurStationName+","+"NA" +","+_qy_PartName+","+_qy_TaskName+","+ct+","+
		 								   "NA,NA,NA,"+"NA"+","+"NA"+","+
		 								   "NA"+","+ModeString+","+((IEleAxisLine)_eleAxisAbove).JogVelocity.ToString()+","+((IEleAxisLine)_eleAxisBelow).JogVelocity.ToString()+","+((IEleAxisLine)_eleAxisX).DefaultParams.Velocity.ToString()+","+
		 				                   ((IEleAxisLine)_eleAxisY).DefaultParams.Velocity.ToString()+","+((IEleAxisLine)_eleAxisZ).DefaultParams.Velocity.ToString()+",NA,NA,NA,"+
		 								   "NA,NA,NA,NA,NA,"+
		 					  			   "NA,NA,NA,NA,NA,"+
		 								   "NA,NA,NA,NA,NA,"+
		 								   "NA,NA,NA,NA,NA,"+
		 								   "NA,NA,NA,NA,NA,"+
		 				                   "NA,NA,NA,NA,"+taskRet+","+
		 								   "NA,NA";
		 			WriteAllData(AllDataString);
		 		}
		 		catch(Exception ex)
		 		{}
		 		
		 		_qy.IsRunning = false;
		 		
		 		if(TaskCancel)
				{
					TaskCancel = false;
					DataInfoManager.Instance.DataInfoSetVal("TaskCancel", false);
					Thread.Sleep(500);
				}
		 		count++;
		 		DataInfoManager.Instance.DataInfoSetVal("QyCount",count);
		 		SetStep(100, "Task End...");
			}
			#endregion				
		RunStep100:  //Chose again or end
			#region
			if (CheckAutoRun() && (CurStep == 100))
			{
				var val=DataParamManager.Instance.DataParamGetParam("QyAuto");
				bool auto=(val!=null?(bool)val.DataVal:false);
	            val=DataParamManager.Instance.DataParamGetParam("QyTimeLimit");
	            int QyTimeLimit=(val!=null?(int)val.DataVal:10);
				if (_qy.IsRunning||auto) 
				{
					SetStep(9, "Test again.'");
				}
				
				if (!_qy.IsTrayLock||(count>=QyTimeLimit))
				{
					//SetStep(101, "Stop qualify test,");
					SetStep(104, "Stop qualify test,");
				}
			}
			#endregion		
		RunStep104:
			#region
			if (CheckAutoRun() && (CurStep == 104))
			{
				task.TaskRunStat = TaskRunState.Stop;
				DataInfoManager.Instance.DataInfoSetVal("QyMode",false);
			}
			#endregion
				
		RunStepErr:
			if (task.CurState == TaskMachineState.Continue  && CheckAutoRun())
			{
				task.CurState = TaskMachineState.Run;
				if(CurStep < 0)
				{
					CurStep = (short)-CurStep;
				}
			}
		}
		return iRet;
	}
	public int Init(ITask task)
	{
		int iRet = 0;
		TaskName = task.Name;
		if (!GetPrim("VpAssemble0", ref  _vpAssemble, 1)) goto TaskInitErr;
	   
	    if (!GetPrim("VpDetector0", ref _vpDetector, 2)) goto TaskInitErr;
		#region Get Eles
		if (!GetEle("Btn_Lock", ref _eleBtnLock, 100)) goto TaskInitErr;
		if (!GetEle("UpLineEntry", ref _eleDiUplineEntry, 101)) goto TaskInitErr;
		if (!GetEle("UpLineExit", ref _eleDiUplineExit, 102)) goto TaskInitErr;
		if (!GetEle("UpLineWork", ref _eleDiUplineWork, 103)) goto TaskInitErr;
		if (!GetEle("UpLineCheck", ref _eleDiUpLineCheck, 104)) goto TaskInitErr;

		if (!GetEle("Line1", ref _eleAxisAbove, 120)) goto TaskInitErr;
		if (!GetEle("Line2", ref _eleAxisBelow, 121)) goto TaskInitErr;
		if (!GetEle("AxisX", ref _eleAxisX, 122)) goto TaskInitErr;
		if (!GetEle("AxisY", ref _eleAxisY, 123)) goto TaskInitErr;
		if (!GetEle("AxisZ", ref _eleAxisZ, 124)) goto TaskInitErr;
		//if (!GetEle("AxisU", ref _eleAxisU, 125)) goto TaskInitErr;
		if (!GetEle("CylBlock", ref _eleCylBlock, 126)) goto TaskInitErr;
		if (!GetEle("CylRaise", ref _eleCylRaise, 127)) goto TaskInitErr;
		if (!GetEle("CylPush",ref _eleCylPush,128)) goto TaskInitErr;
		if (!GetEle("CylScrew",ref _eleCylScrew,129)) goto TaskInitErr;
		if (!GetEle("Vac1",ref _eleVac,130)) goto TaskInitErr;
		#endregion
		
		#region Others
		
		#endregion
		task.TaskRunStat = TaskRunState.Inited;
		
		return iRet;
		
		TaskInitErr:
        		task.TaskRunStat = TaskRunState.Err;
        		return -10;
	}
	
	public bool CheckDryRun()
	{
		IDataParam objDry1 = DataParamManager.Instance.DataParamGetParam("DryRunMode");
    
        bool bDry1 = (objDry1 != null && objDry1.DataVal != null) ? (bool)objDry1.DataVal : false;
        
		var var = DataInfoManager.Instance.DataInfoGetVal("DryRunMode");
		bool bDry2 = (var != null)?(bool)var:false;
		
		DryRunMode = (bDry1 || bDry2);
		return DryRunMode;
	}
	
	public void LogAdd(string log,AlarmLv lv,int id)
	{
		/*
		  if(!LogEnable)
		{
			return;
		}
		  */
		switch(lv)
		{
			case AlarmLv.Fatal:
				Log.Fatal(15100000+id,log,LogClassification.Task,TaskName);
				break;
			case AlarmLv.Error:
				Log.Error(25100000+id,log,LogClassification.Task,TaskName);
				break;
			case AlarmLv.Warn:
				Log.Warn(35100000+id,log,LogClassification.Task,TaskName);
				break;
			case AlarmLv.Info:
				Log.Info(45100000+id,log,LogClassification.Task,TaskName);
				break;
			case AlarmLv.Debug:
				Log.Debug(55100000+id,log,LogClassification.Task,TaskName);
				break;
			case AlarmLv.Trace:
				Log.Trace(65100000+id,log,LogClassification.Task,TaskName);
				break;
		}
	}

    public bool GetPrim(string primName, ref IPrim prim, int id)
    {
        prim = PrimsManager.Instance.GetPrimByName(primName);
        if (prim != null) return true;
        LogAdd(string.Format("{0} was not defined!",primName), AlarmLv.Debug, id);
        MessageBox.Show(string.Format("{0} couldn't be found!",primName));
        return false;
    }

    public bool GetEle(string eleName, ref IEle ele, int id)
    {
        ele = ElesManager.Instance.GetEleByName(eleName);
        if (ele != null) return true;
         LogAdd(string.Format("{0} was not defined!",eleName), AlarmLv.Debug, id);
         MessageBox.Show(string.Format("{0} couldn't be found!",eleName));
        return false;
    }

	public bool CheckAutoRun()
	{
		object obj1 = DataInfoManager.Instance.DataInfoGetVal("AutoRun");
		if (obj1 == null)
		{
			obj1 = (bool) false;
		}
		AutoRun = (bool)obj1;
		return AutoRun;
	}

	public void SetStep(short StepIndex)
	{
		LastStep = CurStep;
		CurStep = StepIndex;
	}

	public void SetStep(short StepIndex, string StepInfo)
	{
		LastStep = CurStep;
		CurStep = StepIndex;

		LogAdd(StepInfo, AlarmLv.Trace, (int)StepIndex);
	}

	public void SetTaskError(bool errFlag, AlarmLv lv, int id, string errMsg, ITask task)
	{
		if(errFlag)
		{
			task.CurState = TaskMachineState.Error;
			LogAdd(errMsg, lv, id);
		}
		else
		{
			task.CurState = TaskMachineState.Other;
		}
	}
	
	private void WriteCTLog(double totalCT,double moveCT)
    {
		CTLogfs = new FileStream(CTLogPath, System.IO.FileMode.Append, System.IO.FileAccess.Write);
		CTLogsw = new StreamWriter(CTLogfs, System.Text.Encoding.Default);
        CTLogsw.AutoFlush = true;

    	string timeStr=GetDateString(DateTime.Now)+"  TotalCT="+totalCT.ToString("#.000") + "  MoveCT="+moveCT.ToString("#.000");
    	CTLogsw.WriteLine(timeStr);
    	
    	CTLogsw.Close();
    	CTLogfs.Close();
    }

	private void WriteCTTime(DateTime _trayInTime, DateTime _trayOutTime, DateTime _trayDockTime, DateTime _trayReleaseTime)
	{
		CTTimefs = new FileStream(CTTimePath, System.IO.FileMode.Append, System.IO.FileAccess.Write);
		CTTimesw = new StreamWriter(CTTimefs, System.Text.Encoding.Default);
        CTTimesw.AutoFlush = true;

        string timeStr=GetDateString(DateTime.Now)+"  _trayInTime="+GetDateString(_trayInTime) + "  _trayOutTime="+GetDateString(_trayOutTime) +
        	"  _trayDockTime="+GetDateString(_trayDockTime)+"  _trayReleaseTime="+GetDateString(_trayReleaseTime);
    	CTTimesw.WriteLine(timeStr);
    	
    	CTTimesw.Close();
    	CTTimefs.Close();
	}
	
	private string GetDateString(DateTime myTime)
	{
		string tempstring="";

    	tempstring=myTime.Year.ToString()+((myTime.Month<10)?("0"+myTime.Month.ToString()):(myTime.Month.ToString()))+((myTime.Day<10)?("0"+myTime.Day.ToString()):(myTime.Day.ToString()));
    	tempstring=tempstring+"_";
    	tempstring=tempstring+((myTime.Hour<10)?("0"+myTime.Hour.ToString()):(myTime.Hour.ToString()))+":"+((myTime.Minute<10)?("0"+myTime.Minute.ToString()):(myTime.Minute.ToString()))+":"+
    		((myTime.Second<10)?("0"+myTime.Second.ToString()):(myTime.Second.ToString()));
		
    	return tempstring;
	}
	
	private string GetDayString(DateTime myTime)
	{
		string tempstring="";

    	tempstring=myTime.Year.ToString()+"-"+((myTime.Month<10)?("0"+myTime.Month.ToString()):(myTime.Month.ToString()))+"-"+((myTime.Day<10)?("0"+myTime.Day.ToString()):(myTime.Day.ToString()));
    	tempstring=tempstring+".csv";

    	return tempstring;
	}
	
	private void WriteAllData(string AutoString = "")
	{
		if(File.Exists(AllRunDataFullPath)==false)
		{
			CreateNewCSV(AllRunDataFullPath);
		}
		
		AllDatafs = new FileStream(AllRunDataFullPath, System.IO.FileMode.Append, System.IO.FileAccess.Write);
		AllDatasw = new StreamWriter(AllDatafs, System.Text.Encoding.Default);
        AllDatasw.AutoFlush = true;
        if(AutoString != "")
        	AllDatasw.WriteLine(AutoString);
    	
    	AllDatasw.Close();
    	AllDatafs.Close();
	}
	private void CreateNewCSV(string filePath)
	{
		if(!Directory.Exists(AllRunDataPath))
			Directory.CreateDirectory(AllRunDataPath);
		AllDatafs = new FileStream(filePath, System.IO.FileMode.Append, System.IO.FileAccess.Write);
		AllDatasw = new StreamWriter(AllDatafs, System.Text.Encoding.Default);
        AllDatasw.AutoFlush = true;

        string tempstr="DSN,Tray_ID,Factory_ID,Project_name,Build_configuration,Operator_name,Start_time,Station_ID,No of Parts,Part Name" +
        	",Task_ID,Cycle_time,Delay_time,Part_barcode,Tray_enterance_time,Tray_dock_time,Tray_release_time,Tray_exit_time," +
        		"Run Mode,Top Conveyer Speed,Bottom Conveyer Speed,X-Axis Speed,Y-Axis Speed,Z-Axis Speed,U-AxisSpeed,Capture the part,"+
        	  "Capture the host,Capture finally,Part_X_offset,Part_Y_offset,Part_R_offset,Host_X_offset,Host_Y_offset,Host_R_offset," +
        	 "X_offset_to_robot,Y_offset_to_robot,R_offset_to_robot,X_offset_result,Y_offset_result,R_offset_result,Critical_errors_ID,"+
        	"Screw_Torque,Pressure_value,PressTime,1# Delta-X for Screw station,1# Delta-Y for Screw station,2# Delta-X for Screw station,"+
        	"2# Delta-Y for Screw station,3# Delta-X for Screw station,3# Delta-Y for Screw station,4# Delta-X for Screw station,4# Delta-Y for Screw station,"+
        	"1# Screw TryTime,2# Screw TryTime,3# Screw TryTime,4# Screw TryTime,OK_NG_Name,Arbitrary_variable_1,Arbitrary_variable_2";
    	AllDatasw.WriteLine(tempstr);
    	
    	AllDatasw.Close();
    	AllDatafs.Close();
	}
	private void CreateNewVisionCSV(string filePath)
	{
		AllDatafs = new FileStream(filePath, System.IO.FileMode.Append, System.IO.FileAccess.Write);
		AllDatasw = new StreamWriter(AllDatafs, System.Text.Encoding.Default);
        AllDatasw.AutoFlush = true;

        string tempstr="No,TrayID,Time,Mode,Station,Pic1,Pic2 X,Pic2 Y,Pic2 R,Pic3,Pass or Fail";
    	AllDatasw.WriteLine(tempstr);
    	
    	AllDatasw.Close();
    	AllDatafs.Close();
	}
}
