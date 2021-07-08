using System;
using System.Windows.Forms;
using Lead.Detect.Global;
using Lead.Detect.Interfaces;
using System.Threading;
using Lead.Detect.LogHelper;
using Lead.Detect.Interfaces.Communicate;
using System.Linq;

public class RunState
{
	#region define IPrim
	private IPrim _superClient = null;
	#endregion
	
	#region IEle
	private IEle _eleDiDnlineEntry = null;
	private IEle _eleDiDnlineExit = null;
	private IEle _eleDiDnEntryReq = null;
	private IEle _eleDiDnExitReq = null;
	private IEle _eleDoDnEntryReq = null;	
	private IEle _eleDoDnExitReq = null;
	private IEle _eleAxisBelow = null;
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
	
	public bool DryRunMode = false;
	public bool SocketEnable = false;
	public bool ConnState = false;
	#endregion
	
	#region Private variables define
	private bool _firstFlag = true;
	private string TaskName = "";
	private DialogResult _mBox1Result = new DialogResult();
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
				
				if(CheckConnectState())
				{
					((ISuperClient)_superClient).SendStationState("station:"+ DevRecipeManager.Instance.StationName + ";DTaskStat:Running;");
				}

				((IEleDO)_eleDoDnEntryReq).Write(false); //reset entry request downline
				((IEleDO)_eleDoDnExitReq).Write(false); //reset exit request downline
				((IEleAxisLine)_eleAxisBelow).MC_Power(true);
				if(!((IEleDI)_eleDiDnlineEntry).Read()&&!((IEleDI)_eleDiDnlineExit).Read()) //read upline and downline optical sensors, if nothing is read, do
				{
					var val = DataParamManager.Instance.GetDataParamByName("DownSpeed"); //retrieve downline speed
					int DownSpeed = (val != null ? (int)val.DataVal : 300);
					((IEleAxisLine)_eleAxisBelow).MC_MoveJog(true, DownSpeed); //move downline
					long st=DateTime.Now.Ticks/10000;
					while(true) //wait 3 seconds before stopping the line, or if something is detected stop
					{
						long et=DateTime.Now.Ticks/10000;
						long dt=et-st;
						if(((IEleDI)_eleDiDnlineEntry).Read()||((IEleDI)_eleDiDnlineExit).Read()) //if any sensor goes high, stop line and move into state machine
						{
							((IEleAxisLine)_eleAxisBelow).MC_Stop();
							SetStep(1, "Make sure it is the discharge status...");
							break;
						}
						if(dt>3000)
						{
							((IEleAxisLine)_eleAxisBelow).MC_Stop();
							SetStep(1, "Make sure it is the discharge status...");
							break;
						}
					}
				}
				
				SetStep(1, "Make sure it is the discharge status...");
			}
			
		RunStep1: //figure out current state

			if (CheckAutoRun() && (CurStep == 1)) 
			{
				if (((IEleDI)_eleDiDnlineExit).Read()) //ifdnline exit sensor
				{
					SetStep(4,"Waiting for the signal of '_eleDiDnExitReq'...");
				}
				else if(((IEleDI)_eleDiDnlineEntry).Read()&&!((IEleDI)_eleDiDnlineExit).Read()) //if dnline entry is high and dnline exit sensor is low
				{
					SetStep(3,"Waiting for the signal of '_eleDiDnExitReq'...");
				}
				else
				{
					SetStep(2,"Waiting for the signal of '_eleDiDnEntryReq' and DownLine will move..."); //if both sensors are low, wait for entry request
				}
			}
		
		RunStep2: //both sensors low

			if (CheckAutoRun() && (CurStep == 2)) 
			{
				((IEleDO)_eleDoDnEntryReq).Write(true); //request new carrier from previous station
				if (((IEleDI)_eleDiDnlineEntry).Read()) //wait here, if entry sensor reads, move carrier in
				{
					//((IEleDO)_eleDoDnEntryReq).Write(false);
					var val = DataParamManager.Instance.GetDataParamByName("DownSpeed");
					int DownSpeed = (val != null ? (int)val.DataVal : 300);
					((IEleAxisLine)_eleAxisBelow).MC_MoveJog(true, DownSpeed);
					SetStep(3,"Waiting for the signal of '_eleDiDnlineExit'...");
				}
			}
			else if(!CheckAutoRun())
			{
				((IEleDO)_eleDoDnEntryReq).Write(false);
			}
		
		RunStep3://if dnline entry is high and dnline exit sensor is low
			if (CheckAutoRun() && (CurStep == 3))
			{
				if(((IEleAxisLine)_eleAxisBelow).IsStop) //if dnline is stopped, entry sensor is high from step 2
				{
					var val = DataParamManager.Instance.GetDataParamByName("DownSpeed");
					int DownSpeed = (val != null ? (int)val.DataVal : 300);
					((IEleAxisLine)_eleAxisBelow).MC_MoveJog(true, DownSpeed); //move dnline to bring in carrier
				}
				DateTime st1=DateTime.Now;
				while(true)
				{
					Thread.Sleep(10);
					if (((IEleDI)_eleDiDnlineExit).Read()) //if exit sensor goes high
					{
						((IEleDO)_eleDoDnEntryReq).Write(false); //turn off entry request
						
						if(!((IEleDI)_eleDiDnExitReq).Read()) //if exit request is low, stop line. Next station is busy
						{
							((IEleAxisLine)_eleAxisBelow).MC_Stop(); //stop line and go to step 4 to wait for exit request
						}
						SetStep(4,"Waiting for the signal of '_eleDiDnExitReq'...");
						break;
					}
					if((DateTime.Now-st1).TotalMilliseconds>=10000) //timeout in case of state machine error
					{
						((IEleAxisLine)_eleAxisBelow).MC_Stop();
						SetStep(1,"Run too long time");
						break;
					}
					if(!CheckAutoRun())
					{
						((IEleAxisLine)_eleAxisBelow).MC_Stop();
						break;
					}
				}
			}
		
		RunStep4: //exit sensor goes high from runstep3, wait here for next station request for carrier

			if (CheckAutoRun() && (CurStep == 4)) 
			{
				if (((IEleDI)_eleDiDnExitReq).Read()) //if next station is ready, move line
				{
					((IEleDO)_eleDoDnExitReq).Write(true);
					
					var val = DataParamManager.Instance.GetDataParamByName("DownSpeed");
					int DownSpeed = (val != null ? (int)val.DataVal : 300);
					((IEleAxisLine)_eleAxisBelow).MC_MoveJog(true, DownSpeed); //move here
					SetStep(5,"Waiting for the signal of '_eleDiDnlineExit'..."); //wait for exit signal
				}
			}
			
		RunStep5: //wait for exit signal
			if (CheckAutoRun() && (CurStep == 5))
			{
				if(((IEleAxisLine)_eleAxisBelow).IsStop) //if line is stopped, why is line stopped?
				{
					var val = DataParamManager.Instance.GetDataParamByName("DownSpeed");
					int DownSpeed = (val != null ? (int)val.DataVal : 300);
					((IEleAxisLine)_eleAxisBelow).MC_MoveJog(true, DownSpeed);
				}
				/*
				if (!((IEleDI)_eleDiDnExitReq).Read())
				{
					((IEleDO)_eleDoDnExitReq).Write(false);
					
					((IEleAxisLine)_eleAxisBelow).MC_Stop();
					SetStep(1, "Make sure it is the discharge status...");
				}
				*/
				if (!((IEleDI)_eleDiDnlineExit).Read()) //if exit sensor goes low, meaning that tray has exitted station
				{
					Thread.Sleep(1000); //wait 1 second for tray to fully exit
					if (!((IEleDI)_eleDiDnlineExit).Read()) //if exit sensor is still low
					{
						((IEleDO)_eleDoDnExitReq).Write(false); //turn off exit request
						
						((IEleAxisLine)_eleAxisBelow).MC_Stop(); //stop line
						SetStep(1, "Make sure it is the discharge status..."); //loop back to 1
					}
				}
			}
			
		RunStepErr:

			if (task.CurState == TaskMachineState.Continue  && CheckAutoRun())
			{
				task.CurState = TaskMachineState.Run;

				
				if(CheckConnectState())
				{
					((ISuperClient)_superClient).SendStationState("station:"+ DevRecipeManager.Instance.StationName + ";DTaskStat:Running;");
				}

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
		if(!GetPrim("SuperClient0", ref _superClient, 4)) goto TaskInitErr;
		#region Get Eles
		if (!GetEle("DnLineEntry", ref _eleDiDnlineEntry, 1)) goto TaskInitErr;
	    if (!GetEle("DnLineExit", ref _eleDiDnlineExit, 2)) goto TaskInitErr;
		if (!GetEle("DnEntryReq", ref _eleDiDnEntryReq, 3)) goto TaskInitErr;
	    if (!GetEle("DnExitReq", ref _eleDiDnExitReq, 4)) goto TaskInitErr;
	    if (!GetEle("DnEntryReqO", ref _eleDoDnEntryReq, 5)) goto TaskInitErr;
	    if (!GetEle("DnExitReqO", ref _eleDoDnExitReq, 6)) goto TaskInitErr;
		if (!GetEle("Line2", ref _eleAxisBelow, 10)) goto TaskInitErr;
		#endregion
		LogAdd("DoLine task Init:" + "Init OK !!!", AlarmLv.Info, 100);

		var val11=DataParamManager.Instance.DataParamGetParam("SocketEnable");
		SocketEnable=(val11!=null?(bool)val11.DataVal:false);

		task.TaskRunStat = TaskRunState.Inited;
		return iRet;
		
		TaskInitErr:
        task.TaskRunStat = TaskRunState.Err;
        return iRet;
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
				Log.Fatal(15000000+id,log,LogClassification.Task,TaskName);
				break;
			case AlarmLv.Error:
				Log.Error(25000000+id,log,LogClassification.Task,TaskName);
				break;
			case AlarmLv.Warn:
				Log.Warn(35000000+id,log,LogClassification.Task,TaskName);
				break;
			case AlarmLv.Info:
				Log.Info(45000000+id,log,LogClassification.Task,TaskName);
				break;
			case AlarmLv.Debug:
				Log.Debug(55000000+id,log,LogClassification.Task,TaskName);
				break;
			case AlarmLv.Trace:
				Log.Trace(65000000+id,log,LogClassification.Task,TaskName);
				break;
		}
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
	
	public bool CheckDryRun()
	{
		object obj1 = DataInfoManager.Instance.DataInfoGetVal("DryRunMode");
		if (obj1 == null)
		{
			obj1 = (bool) false;
		}
		DryRunMode = (bool)obj1;
		return DryRunMode;
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
		
		DataOutputManager.Instance.ClientDataOutputManagerUpdateState("DownStepInfo", StepInfo);
	}

	public bool CheckConnectState()
    {
		IDataParam valSocket = DataParamManager.Instance.GetDataParamByName("SocketEnable");
		bool enable = (valSocket != null ? (bool)valSocket.DataVal : false);
		if (!enable) 
		{
			return false;
		}
		ConnState=(bool)DataInfoManager.Instance.DataInfoGetVal("ConnState");
		if(SocketEnable && ConnState)
		{
			return true;
		}
		return false;
    }
}


