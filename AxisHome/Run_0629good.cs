using System;
using System.Windows.Forms;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using Lead.Detect.Global;
using Lead.Detect.Interfaces;
using Lead.Detect.LogHelper;
using Lead.Detect.Interfaces.Communicate;

using Lead.Detect.BussinessModel;

using System.Linq;
using System.Net.Sockets;
using System.Net;

public class RunState
{
	public IEle _eleMotion=null;
	public IEle _eleAxisAbove = null;
	public IEle _eleAxisBelow = null;
	public IEle _eleAxisX = null;
	public IEle _eleAxisY = null;
	public IEle _eleAxisZ = null;
	public IEle _eleAxisU = null;
	
	private int GoOriginStep=-1;
	private int GoOriginErrStep = -1;
	private bool GoOriginFlag=false;
	private bool GoOriginDone=false;
	
	private IEleAxisLine[] AxisGroup;
	private IEleAxisLine[] AxisGroup1;
	
	private double[] WaitPos;
	private double[] HomeVelGroup;
	private double[] MoveVelGroup;
	private double[] HomeVelGroup1;
	private double[] MoveVelGroup1;
	private string TaskName = "";
	
	public int Exec(ITask task)
	{
		int iRet = 0;
		
		AxisGroup=new[]{(IEleAxisLine)_eleAxisX,(IEleAxisLine)_eleAxisY,(IEleAxisLine)_eleAxisZ};
		HomeVelGroup=new[]{((IEleAxisLine)_eleAxisX).HomeVelocity,((IEleAxisLine)_eleAxisY).HomeVelocity,((IEleAxisLine)_eleAxisZ).HomeVelocity};
		MoveVelGroup=new[]{((IEleAxisLine)_eleAxisX).DefaultParams.Velocity,((IEleAxisLine)_eleAxisY).DefaultParams.Velocity,((IEleAxisLine)_eleAxisZ).DefaultParams.Velocity};
		
		AxisGroup1=new[]{(IEleAxisLine)_eleAxisX,(IEleAxisLine)_eleAxisY};
		HomeVelGroup1=new[]{((IEleAxisLine)_eleAxisX).HomeVelocity,((IEleAxisLine)_eleAxisY).HomeVelocity};
		MoveVelGroup1=new[]{((IEleAxisLine)_eleAxisX).DefaultParams.Velocity,((IEleAxisLine)_eleAxisY).DefaultParams.Velocity};
		
		var pos = ElesManager.Instance.ListPos.FirstOrDefault(p => p.Name == "Wait");
        WaitPos=pos.Data();
        
		GoOriginStep = 0;
		GoOriginErrStep = 0;
		//GoOriginFlag = false;
		GoOriginDone = false;
		
		while(task.TaskRunStat != TaskRunState.Stop)
		{
			DataInfoManager.Instance.DataInfoSetVal("MotionHomed",false);
			Thread.Sleep(20);
				if(CheckDoor()&&GoOriginStep==0)
				{
					//GoOriginFlag = true;
					LogAdd("Every axis of MotionGroup motors on.", AlarmLv.Info,8000);
					((IEleAxisLine)_eleAxisX).MC_Power(true);
					((IEleAxisLine)_eleAxisY).MC_Power(true);
					((IEleAxisLine)_eleAxisZ).MC_Power(true);

					GoOriginStep=10;
				}
				if(CheckDoor()&&GoOriginStep==10)
				{
					LogAdd("Axis Z go to -limit", AlarmLv.Info,8010);
					((IEleAxisLine)_eleAxisZ).CheckLimitEnable = false;
					if(((IEleAxisGroup)_eleMotion).MoveAbs(AxisGroup[2],-200,MoveVelGroup[2],-1,false)!=0)
					{
						LogAdd("Axis X and Y go home failed",AlarmLv.Error,8010);
						GoOriginErrStep=-10;
						GoOriginStep=100;
					}
					else
					{
						GoOriginStep=20;
						
					}
				}
				if(CheckDoor()&&GoOriginStep==20)
				{
					LogAdd("Axis X and Y go home", AlarmLv.Info,8020);
					if(((IEleAxisGroup)_eleMotion).Home(AxisGroup1,HomeVelGroup1) != 0)
					{
						LogAdd("Axis X and Y go home failed!", AlarmLv.Error,8020);
						
						GoOriginErrStep=-20;
						GoOriginStep=100;
					}
					else
					{
						GoOriginStep=30;
					}
				}
				if(CheckDoor()&&GoOriginStep==30)
				{
					LogAdd("Axis X and Y move to Wait pos", AlarmLv.Info,8030);
					if(((IEleAxisGroup)_eleMotion).MoveAbs(AxisGroup1,new double[]{WaitPos[0],WaitPos[1]},MoveVelGroup1,100000,false) != 0)
					{
						LogAdd("Axis X and Y move to Wait pos failed!", AlarmLv.Error,8030);
						
						GoOriginErrStep=-30;
						GoOriginStep=100;
					}
					else
					{
						GoOriginStep=40;
					}
				}
				if(CheckDoor()&&GoOriginStep==40)
				{
					LogAdd("Axis Z go home", AlarmLv.Info,8040);
					if(((IEleAxisGroup)_eleMotion).Home((IEleAxisLine)_eleAxisZ,(int)((IEleAxisLine)_eleAxisZ).HomeVelocity) !=0 )
					{
						LogAdd("Axis Z go home failed!", AlarmLv.Error,8040);
						
						GoOriginErrStep=-40;
						GoOriginStep=100;
					}
					else
					{
						GoOriginStep=50;
					}
				}
				if(CheckDoor()&&GoOriginStep==50)
				{
					LogAdd("Axis go to wait pos", AlarmLv.Info,8060);
					if(((IEleAxisGroup)_eleMotion).MoveAbs(AxisGroup[2],WaitPos[2],MoveVelGroup[2],-1,false) != 0)
					{
						LogAdd("Axis go to wait pos failed!", AlarmLv.Error,8060);
						
						GoOriginErrStep=-50;
						GoOriginStep=100;
					}
					else
					{
						GoOriginDone = true;
						GoOriginStep=100;
					}
				}
				
				if(CheckDoor()&&GoOriginStep==100)
				{
					
					if(GoOriginDone)
					{
						DataInfoManager.Instance.DataInfoSetVal("MotionHomed",true);
						task.TaskRunStat = TaskRunState.Stop;
						break;
					}
					else
					{
						DataInfoManager.Instance.DataInfoSetVal("MotionHomed",false);
						task.TaskRunStat = TaskRunState.Stop;
						break;
					}
				}
			StepError:
				if (GoOriginErrStep < -1) 
				{
					Thread.Sleep(100);
					var ee=DataInfoManager.Instance.DataInfoGetVal("EStopPressed");
					bool EStopPressed=ee!=null?(bool)ee:false;
					if(!EStopPressed)
					{
						var var = MessageBox.Show("MotionGroup gone to find origin and 'WaitPos' error!","Error!", MessageBoxButtons.RetryCancel);
						if (var == DialogResult.Retry) 
						{
							GoOriginStep = -GoOriginErrStep;
							//MotionContinue();
							GoOriginErrStep = 0;
						}
						else
						{
							GoOriginDone = false;
							GoOriginStep=100;
							GoOriginErrStep=0;
							//GoOriginFlag = false;
						}
					}
					else
					{
						GoOriginDone = false;
						GoOriginStep=100;
						GoOriginErrStep=0;
						//GoOriginFlag = false;
					}
					
				}
				
			}
			
		return iRet;
	}
	 
	public int Init(ITask task)
	{
		int iRet = 0;
		TaskName = task.Name;
		if(!GetEle("Motion", ref _eleMotion, 1))   goto TaskInitErr; 
	    if (!GetEle("Line1", ref _eleAxisAbove, 120)) goto TaskInitErr;
	    if (!GetEle("Line2", ref _eleAxisBelow, 121)) goto TaskInitErr;
		if (!GetEle("AxisX", ref _eleAxisX, 122)) goto TaskInitErr;
	    if (!GetEle("AxisY", ref _eleAxisY, 123)) goto TaskInitErr;
		if (!GetEle("AxisZ", ref _eleAxisZ, 124)) goto TaskInitErr;

		task.TaskRunStat = TaskRunState.Inited;
		
		return iRet;
		
	    TaskInitErr:
        task.TaskRunStat = TaskRunState.Err;
        return iRet;
		
		return iRet;
	}
	public bool GetEle(string eleName, ref IEle ele, int id)
    {
        ele = ElesManager.Instance.GetEleByName(eleName);
        if (ele != null) return true;
         LogAdd(string.Format("{0} was not defined!",eleName), AlarmLv.Debug, id);
         MessageBox.Show(string.Format("{0} couldn't be found!",eleName));
        return false;
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
	public bool CheckDoor()
	{
		var val=DataInfoManager.Instance.DataInfoGetVal("DoorClosed");
		bool result=val!=null?(bool)val:false;
		return result;
	}
}

