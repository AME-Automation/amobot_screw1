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

public class RunState
{
	#region Prims define
	//private IPrim _mySql = null;
	private IPrim _superClient = null;
	#endregion
	#region Elements define
	private IEle _eleBtnLock = null;
	
	private IEle _eleDiUplineEntry = null;
	private IEle _eleDiUplineExit = null;
	private IEle _eleDiUplineWork = null;
	private IEle _eleDiUpLineCheck = null;
	
	private IEle _eleDiUpEntryReq = null;
	private IEle _eleDiUpExitReq = null;
	private IEle _eleDiDnEntryReq = null;
	private IEle _eleDiDnExitReq = null;

	private IEle _eleDoUpEntryReq = null;
	private IEle _eleDoUpExitReq = null;
	private IEle _eleDoDnEntryReq = null;
	private IEle _eleDoDnExitReq = null;
	
	private IEle  _eleAxisAbove = null;
	private IEle  _eleAxisBelow = null;
	private IEle _eleAxisX = null;
	private IEle _eleAxisY = null;
	private IEle _eleAxisZ = null;
	private IEle _eleAxisU = null;
	private IEle _eleCylBlock = null;
	private IEle _eleCylRaise = null;
	private IEle _eleCylPush = null;
	#endregion
	#region Limit
	private string above_speed="na";
	private string below_speed="na";
	private string x_speed_max="na";
	private string x_speed_min="na";
	private string y_speed_max="na";
	private string y_speed_min="na";
	private string z_speed_max="na";
	private string z_speed_min="na";
	private string r_speed_max="na";
	private string r_speed_min="na";
	private string top_conveyer_speed_max="na";
	private string top_conveyer_speed_min="na";
	private string bottom_conveyer_speed_max="na";
	private string bottom_conveyer_speed_min="na";
	private string x_offset_robot_max="na";
	private string x_offset_robot_min="na";
	private string y_offset_robot_max="na";
	private string y_offset_robot_min="na";
	private string r_offset_robot_max="na";
	private string r_offset_robot_min="na";
	private string delta_1_x_for_screw_station_max="na";
	private string delta_1_x_for_screw_station_min="na";
	private string delta_1_y_for_screw_station_max="na";
	private string delta_1_y_for_screw_station_min="na";
	private string delta_2_x_for_screw_station_max="na";
	private string delta_2_x_for_screw_station_min="na";
	private string delta_2_y_for_screw_station_max="na";
	private string delta_2_y_for_screw_station_min="na";
	private string delta_3_x_for_screw_station_max="na";
	private string delta_3_x_for_screw_station_min="na";
	private string delta_3_y_for_screw_station_max="na";
	private string delta_3_y_for_screw_station_min="na";
	private string delta_4_x_for_screw_station_max="na";
	private string delta_4_x_for_screw_station_min="na";
	private string delta_4_y_for_screw_station_max="na";
	private string delta_4_y_for_screw_station_min="na";
	private string screw_torque_max="na";
	private string screw_torque_min="na";
	private string screw_1_try_time_max="na";
	private string screw_1_try_time_min="na";
	private string screw_2_try_time_max="na";
	private string screw_2_try_time_min="na";
	private string screw_3_try_time_max="na";
	private string screw_3_try_time_min="na";
	private string screw_4_try_time_max="na";
	private string screw_4_try_time_min="na";
	private string press_time_max="na";
	private string press_time_min="na";
	private string press_value_max="na";
	private string press_value_min="na";
	#endregion
	#region Public variables define
	public bool ErrFlag = false;
	public bool AutoRun = false;
	public bool Continue = false;
	
	// public bool SqlCheckErr=false;
	
	public bool TaskError = false;
	public short TaskErrLv = -1;
	public string TaskErrMsg = "";
	public short CurStep = -1;
	public short LastStep = -1;
	public short TargetStep = -1;
	
	public bool TrayReady = false;
	public bool TaskCancel = false;
	public int TaskResult = -1;
	
	public string CurRep = "";
	
	public bool SocketEnable = false;
	public bool ConnState = false;
	#endregion
	#region Private variables define
	private bool _firstFlag = true;
	private bool _recipeSel = false;
	private bool _ngFlag = false;
	
	private bool DryRunMode=false;
	private bool firstStart=true;
	private bool Capture=false;
	

	private DialogResult _mBox1Result = new DialogResult();
	private List<Tray> TrayList=null;
	private Tray  _curTray = null;
	private Unit _curUnit = null;
	private long _st = -1;
	private long _end = -1;
	private ITask _lastTask = null;
	private int nextIndex=-1;
	
	private double CapResultX1 = 0;//Capture的四个点位坐标值
	private double CapResultY1 = 0;
	private double CapResultX2 = 0;
	private double CapResultY2 = 0;
	private double CapResultX3 = 0;
	private double CapResultY3 = 0;
	private double CapResultX4 = 0;
	private double CapResultY4 = 0;
	private double CapResultX5 = 0;
	private double CapResultY5 = 0;	
	
	/*
	// Machine captures 4 screws in one capture
	// Need to capture this differently
	private bool Capture1=false;
	private bool Capture2=false;
	private bool Capture3=false;
	private bool Capture4=false;
	private bool Capture5=false;
	*/
	
	/*
	// No longer needed. Remove later.
	private double CapResultX=0;
	private double CapResultY=0;
	private double CapResultR=0;
	private bool Dector=false;
	*/
	
	private DateTime _trayInTime=DateTime.Now;
	private DateTime _trayOutTime=DateTime.Now;
	private DateTime _trayDockTime=DateTime.Now;
	private DateTime _trayReleaseTime=DateTime.Now;
	private DateTime _autoEndTime=DateTime.Now;
	
	private DateTime _trayDockStart=DateTime.Now;
	private DateTime _trayReleaseStart=DateTime.Now;
	private DateTime _trayStartAgainTime = DateTime.Now;
	
	private double _delayTime=0;
	private double _downTime=0;
	private double _unConveyorTime=0;
	private double _feedTime=0;
	private double _scheduleTime=0;
	private double _unScheduleTime=0;
	private double delayTime=0;
	
	private FileStream CTLogfs,CTTimefs,AllDatafs,templatefs;
	private StreamWriter CTLogsw,CTTimesw,AllDatasw;
	private StreamReader templatesr, barcodesr;
	
	private string TaskName = "";
	private string AutoTaskName = "";
	private string ModeString="";
	
	private string CTLogPath="D:\\TotalCTFile.txt";
	private string CTTimePath="D:\\TotalTimeFile.txt";
	private string TemplateBarcodePath="D:\\AllCSV\\";
	private string AllRunDataPath=@"\\172.25.1.10\shareDoc\TestLog\Galiote\MAPS\";
	// private string AllRunDataPath="D:\\AllCSV\\";
	private string AllRunDataFullPath="";
	private string CurFileDate="";
	private string AllDataString="";
	private string ErrID="na";
	
	private int Screw1TryTime=0;
	private int Screw2TryTime=0;
	private int Screw3TryTime=0;
	private int Screw4TryTime=0;
	private int Screw5TryTime=0;
	#endregion
	#region LM
	private LineConfig lineConfig=null;
	private string DSN = "na";
	private string TrayID = "na";
	private string FactoryID = "PL126";
	private string ProjectName = "galiote";
	private string BuildConfig = "proto4";
	private string OperatorName = "na";
	private string StartTime = "na";
	private string CurStationName = "screw1";
	private string Pic1="";
	private string Pic2="";
	private string Pic3="";
	private string Pic4="";
	private string Pic6="";
	private string VppRunDataFullPath="";
	#endregion
	#region DownTime
	private bool AutoRunning=false;
	private bool Once=true;
	private long Down_Start=0;
	private long Down_End=0;
	private long Down_All=0;
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
				
				firstStart=true;
				//get velocities
				DataOutputManager.Instance.ClientDataOutputManagerUpdateVar("UpLine_Vc",((IEleAxisLine)_eleAxisAbove).DefaultParams.Velocity.ToString());
				DataOutputManager.Instance.ClientDataOutputManagerUpdateVar("DnLine_Vc",((IEleAxisLine)_eleAxisBelow).DefaultParams.Velocity.ToString());
				DataOutputManager.Instance.ClientDataOutputManagerUpdateVar("AxisX_Vc",((IEleAxisLine)_eleAxisX).DefaultParams.Velocity.ToString());
				DataOutputManager.Instance.ClientDataOutputManagerUpdateVar("AxisY_Vc",((IEleAxisLine)_eleAxisY).DefaultParams.Velocity.ToString());
				DataOutputManager.Instance.ClientDataOutputManagerUpdateVar("AxisZ_Vc",((IEleAxisLine)_eleAxisZ).DefaultParams.Velocity.ToString());
			
				var val = DataParamManager.Instance.GetDataParamByName("UpSpeed1");
				int UpSpeed1 = (val != null ? (int)val.DataVal : 80);
				above_speed=UpSpeed1.ToString();
				
				var val1 = DataParamManager.Instance.GetDataParamByName("DownSpeed");
				int DownSpeed = (val1 != null ? (int)val1.DataVal : 300);
				below_speed=DownSpeed.ToString();
				
				DataInfoManager.Instance.DataInfoSetVal("TrayReady", false);
				DataInfoManager.Instance.DataInfoSetVal("TaskCancel", false);
				DataInfoManager.Instance.DataInfoSetVal("TaskResult", -1);
				
				((IEleDO)_eleDoUpEntryReq).Write(false); //reset entry and exit requests
				((IEleDO)_eleDoUpExitReq).Write(false);
				((IEleAxisLine)_eleAxisAbove).MC_Stop();
				
				((IEleCyldLine)_eleCylPush).CyldRetract(); //reset all holding cylinders
				((IEleCyldLine)_eleCylBlock).CyldRetract();
				((IEleCyldLine)_eleCylRaise).CyldRetract();
				
				SetStep(1, "Make sure it is the discharge status...");
			}
			if(AutoRunning)
			{
				if(!CheckAutoRun()&&Once)
				{
					Once=false;
					Down_Start=DateTime.Now.Ticks/10000;
				}
				if(CheckAutoRun()&&!Once) //if auto run and not startup, record downtime
				{
					Once=true;
					Down_End=DateTime.Now.Ticks/10000;
					Down_All=Down_All+(Down_End-Down_Start)/1000;
					_downTime=Down_All;
					DataInfoManager.Instance.DataInfoSetVal("Down_Time",Down_All.ToString());
				}
			}
			else
			{
				Down_All=0;
			}
		RunStep1:  //check Cur_state 
			#region
			if (CheckAutoRun() && (CurStep == 1)) 
			{
				if (((IEleDI)_eleDiUplineWork).Read() && ((IEleDI)_eleDiUplineExit).Read()) //if work sensor is high and exit sensor, there is a tray needs discharge
				{
					LogAdd("We need first to discharge the product...", AlarmLv.Info,1);
					
					if (true)//((IEleDI)_eleBtnLock).Read()) //if Btn_lock, which is the key
					{
					    if(((IEleDI)_eleDiUpExitReq).Read()) //if next station exit request true, turn on output request
						{
					    	((IEleDO)_eleDoUpExitReq).Write(true);
				 		}
						else
						{
							((IEleDO)_eleDoUpExitReq).Write(false); // else do not request to outflow
						}
					}	
				}
				else
				{
					if (true)//((IEleDI)_eleBtnLock).Read()) // else if just btnlock, exit sensor not reading. Note: if we set key to lock, get an error message
					{
						if(((((IEleDI)_eleDiUplineEntry).Read()||(((IEleDI)_eleDiUpLineCheck).Read()&&!((IEleDI)_eleDiUplineWork).Read()))&&firstStart)) // this if does nothing
						{
							//SetStep(222,"AxisAbove run");
						}
						else if(((IEleDI)_eleDiUplineWork).Read()&&firstStart) // if work position sensor and this the first cycle
						{
							/*SetTaskError(true,AlarmLv.Error,01,"Work Place is not empty,please move it!",task);
							Thread.Sleep(1000);
							MessageBox.Show("Work Place is not empty,please move it!");
							goto RunStep1;*/
						}
						else
						{
							((IEleDO)_eleDoUpEntryReq).Write(true); //request inflow from previous station
						}					
					}
					// SqlCheckErr=false;
					SetStep(2,"Waiting for trigger signal...");
				}
			}
			#endregion					
		RunStep2: //check Lock or not
			#region
			if (CurStep == 2)
			{
				DataOutputManager.Instance.ClientDataOutputManagerUpdateState("Result", "");
				if (((IEleDI)_eleBtnLock).Read() && (((IEleDI)_eleDiUpEntryReq).Read()||(((IEleDI)_eleDiUplineEntry).Read()&&firstStart)))		
					//Mode of 'Lock', and receive the signal of 'UpEntryReq' note that this is bypassed if the sensor is read and first cycle
				{
					firstStart=false; //no longer first start
					if(((IEleDI)_eleBtnLock).Read()) //if in lock mode
					{
						LineConfigManager.Instance.GetCurrentLineConfig(out lineConfig); //call line manager
					}
					LogAdd("Mode of 'Lock', and receive the signal of 'UpEntryReq'", AlarmLv.Info,2);
					SetStep(222,"Mode of 'Lock', and UpLine will be started!");
				}
				
				if (!((IEleDI)_eleBtnLock).Read() && ((IEleDI)_eleDiUplineEntry).Read())		//Mode of 'UnLock', and receive the signal of 'UpLineEntry'
				{
					LogAdd("Mode of 'UnLock', and receive the signal of 'UpLineEntry'", AlarmLv.Info,2);
					SetStep(202,"Mode of 'UnLock', and UpLine will be started!");
				}
			}
			#endregion					
		RunStep2010:
			#region
			if (CheckAutoRun() && (CurStep == 2010))
			{
				if (_mBox1Result == DialogResult.Retry) 
				{
				SetStep(405,"Retry to fetch data from the recipe.");
				}
				else
				{
					//SqlCheckErr=true;
					DataInfoManager.Instance.DataInfoSetVal("TaskCancel", true);
					SetStep(5, "UpConvey will move...");
				}	
			}
			#endregion		
		RunStep202: //if unlock, Start "SubTask) //gets station ready?
			#region
			if (CheckAutoRun() && (CurStep == 202))
			{
				Action startSubTask = () =>
				{
					IDataParam obj = DataParamManager.Instance.DataParamGetParam("SubTask");
					string subTask = (obj == null || string.IsNullOrEmpty((string)obj.DataVal))? "Auto":(string)obj.DataVal;
					ITask taskT = TasksManager.Instance.GetTaskByName(subTask);
					if (taskT != null && _lastTask != taskT) 
					{
						_lastTask = taskT;
						DataOutputManager.Instance.ClientDataOutputManagerUpdateState("UpTaskName", taskT.Name);
						LogAdd("Start" + taskT.Name ,AlarmLv.Info,1001);
						taskT.TaskRunStat = TaskRunState.Running;
						taskT.RunMode = TaskRunMode.Auto;
						Thread.Sleep(200);
						AutoTaskName = taskT.Name;
						taskT.ITaskRun();
					}
				};
				startSubTask.BeginInvoke(null,null);
				
				var val = DataParamManager.Instance.GetDataParamByName("UpSpeed1");
				int UpSpeed1 = (val != null ? (int)val.DataVal : 80);
				((IEleAxisLine)_eleAxisAbove).MC_MoveJog(true, UpSpeed1);
				((IEleCyldLine)_eleCylBlock).CyldStretch(); //move out the blocking cylinder
				SetStep(222,"Waiting for the signal of '_eleDiUplineEntry'...");
			}
			#endregion	
		RunStep222: //AxisAbove run, lock mode and inflow request high
			#region
			if (CheckAutoRun() && (CurStep == 222))
			{
				if(((IEleAxisLine)_eleAxisAbove).IsStop) //if belt is stopped
				{
					var val = DataParamManager.Instance.GetDataParamByName("UpSpeed1");
					int UpSpeed1 = (val != null ? (int)val.DataVal : 80);
					((IEleAxisLine)_eleAxisAbove).MC_MoveJog(true,UpSpeed1); //move belt
					((IEleCyldLine)_eleCylBlock).CyldStretch(); //raise blocking cylinder
				}
				if (((IEleDI)_eleDiUplineEntry).Read()) //if tray enters
				{
					AutoRunning=true;
					_trayInTime=DateTime.Now;
					((IEleDO)_eleDoUpEntryReq).Write(false);
					SetStep(3,"Waiting for DiUpLineCheck, and UpConvey will be started at UpSpeed2...");
				}
			}
			#endregion		
		RunStep3: //AxisAbove vel change, entry sensor has detected a tray
			#region
			if (CheckAutoRun() && (CurStep == 3))
			{
				if(((IEleAxisLine)_eleAxisAbove).IsStop) //if line is stopped
				{
					var val = DataParamManager.Instance.GetDataParamByName("UpSpeed1");
					int UpSpeed1 = (val != null ? (int)val.DataVal : 80);
					((IEleAxisLine)_eleAxisAbove).MC_MoveJog(true,UpSpeed1); //move slowly
				}
				
				if (((IEleDI)_eleDiUpLineCheck).Read()) //if tray entry sensor
				{
					var val = DataParamManager.Instance.GetDataParamByName("UpSpeed2");
					int UpSpeed2 = (val != null ? (int)val.DataVal : 50);
					val = DataParamManager.Instance.GetDataParamByName("UpDelay");
					int UpDelay = (val != null ? (int)val.DataVal : 500);
					
					Thread.Sleep(UpDelay);
					((IEleAxisLine)_eleAxisAbove).MC_ChangeVel(UpSpeed2); //move faster
					
					SetStep(4, "Waiting for DiUpLineCheck disappeared, and UpConvey will stop, CyldIntercept will retract, CyldRaise will stretch...");
				}
				
				if (((IEleDI)_eleDiUplineWork).Read()) //if work position sensor goes high, stop line
				{
					Thread.Sleep(1000);
					((IEleAxisLine)_eleAxisAbove).MC_Stop(); //stop line
					SetStep(4, "Waiting for DiUpLineCheck disappeared, and UpConvey will stop, CyldIntercept will retract, CyldRaise will stretch...");
				}
			}
			#endregion		
		RunStep4: //Tray is in pos,"_eleDoUpEntryReq" make false, from step 3, tray has been detected at entry and line is moving to bring it in
			#region
			if (CheckAutoRun() && (CurStep == 4))
			{
				if(((IEleAxisLine)_eleAxisAbove).IsStop) //if line is stopped, run line
				{
					var val = DataParamManager.Instance.GetDataParamByName("UpSpeed2");
					int UpSpeed2 = (val != null ? (int)val.DataVal : 50);
					((IEleAxisLine)_eleAxisAbove).MC_MoveJog(true,UpSpeed2); 
				}
				
				if (((IEleDI)_eleDiUpLineCheck).Read() && ((IEleDI)_eleDiUplineWork).Read()) //if work position sensors are high (check is another sensor in the tray work position
				{
					if (((IEleDI)_eleBtnLock).Read()) //if lock mode
					{
						((IEleDO)_eleDoUpEntryReq).Write(false); //turn off upline inflow request
					}
					
					var val = DataParamManager.Instance.GetDataParamByName("UpLineStopDelay");
					int UpLineStopDelay = (val != null ? (int)val.DataVal : 500);
					Thread.Sleep(UpLineStopDelay);
					
					((IEleAxisLine)_eleAxisAbove).MC_Stop(); //stop conveyor
					_trayDockStart=DateTime.Now;
					SetStep(401, "'_eleCylBlock' will retract...");
				}
			}
			#endregion
		RunStep401: //if CylBlock retract and "DoUpEntryReq" make false //check state of tray blocking cylinder. lower Blocking cylinder
			#region
			if (CheckAutoRun() && (CurStep == 401))
			{
				if(((IEleCyldLine)_eleCylBlock).CyldRetract(true, true) != 0) //lower blocking cylinder
				{
					SetTaskError(true, AlarmLv.Error, 401, "'_eleCylBlock' retracted failed!", task);
					_mBox1Result = MessageBox.Show("'_eleCylBlock' retracted failed! retry?");
					SetStep(401, "'_eleCylBlock' will retract...");
				}
				else
				{
					if (((IEleDI)_eleBtnLock).Read()) 
					{
						((IEleDO)_eleDoUpEntryReq).Write(false); //if locked, turn off entry request again (repeat of step 4)
					}
					Thread.Sleep(50);
					SetStep(402, "'_eleCylRaise' will stretch...");	
				}
			}
			#endregion
		RunStep402:  //CylRaise stretch
			#region
			if (CheckAutoRun() && (CurStep == 402))
			{
				if(((IEleCyldLine)_eleCylRaise).CyldStretch(true, false) != 0) // raise tray and lock
				{
					SetTaskError(true, AlarmLv.Error, 402, "'_eleCylRaise' stretch failed!", task);
					_mBox1Result = MessageBox.Show("'_eleCylRaise' stretched failed! retry?");
					SetStep(402, "'_eleCylRaise' will stretch...");
				}
				else
				{
					SetStep(403, "'_eleCylRaise' will retract...");
				}
			}
			#endregion		
		RunStep403:  //CyldPush stretch //press down on MLB before screwdown
			#region
			if(CheckAutoRun() && (CurStep == 403))
			{
				if(((IEleCyldLine)_eleCylPush).CyldStretch(true, false) != 0)
				{
					SetTaskError(true, AlarmLv.Error, 403, "'_eleCylPush' stretch failed!", task);
					_mBox1Result = MessageBox.Show("'_eleCylPush' stretched failed! retry?");
					SetStep(403, "'_eleCylRaise' will stretch...");
				}
				else
				{
					SetStep(404, "Judge the 'ClearMode'...");
				}
			}
			#endregion
		RunStep404:  //check ClearMode or not ,if not ,send "TrayReady" true
			#region
			if (CheckAutoRun() && (CurStep == 404))
			{
				var val = DataParamManager.Instance.GetDataParamByName("ClearMode");
				bool ClearMode = (val != null ? (bool)val.DataVal : false);
				
				if (!ClearMode)  //what??? guess this station is in clearmode
				{
					//DataInfoManager.Instance.DataInfoSetVal("TrayReady", true);
					_trayDockTime=DateTime.Now;
					//SetStep(5, "The feeding action is completed, and the assembly action can be performed.");
					SetStep(405,"Check MySql");
				}
				else
				{
					SetStep(6, "ClearMode, UpConvey will move..."); //runstep 6 doesn't exist???
				}
			}
			#endregion	
		 RunStep405:  //Sql check-removed
			#region
			if (CheckAutoRun() && (CurStep == 405))
			{	
				/* #region
				if(((IEleDI)_eleBtnLock).Read() && SocketEnable) //if btnlock, if not, send tray read
				{
					DataTable dtCurrep =  ((ISql)_mySql).GetTableAllData("currecipe");
					if(dtCurrep == null)
					{
						SetTaskError(true, AlarmLv.Error,201," Server Error: currecipe dt is null!",task);
						
						_mBox1Result = MessageBox.Show("Server Error: currecipe dt is null!","Error!", MessageBoxButtons.RetryCancel);
						SetStep(-2010);
						goto RunStepErr;
					}
					CurRep = (string)dtCurrep.Rows[0][0];
					int iDry = (int)dtCurrep.Rows[0][1];
					if(iDry == 1)
					{
						DryRunMode = true;
						DataInfoManager.Instance.DataInfoSetVal("DryRunMode",true);
					}
					else
					{
						DryRunMode = false;
						DataInfoManager.Instance.DataInfoSetVal("DryRunMode",false);
					}
					DataOutputManager.Instance.ClientDataOutputManagerUpdateState("RepName",CurRep);
					
					 DataTable dtRep = ((ISql)_mySql).GetTableAllData(CurRep);
					
					if((dtRep.Rows.Count) < 1)
					{
						
					} 
					else
					{
						string stationNameRep = "";
						string stationName = DevRecipeManager.Instance.StationName;
						string stationTaskRep = "";
						int devIdxRep = -1;
						int devIdx = DevRecipeManager.Instance.StationIdx;
						int curRepRow = -1;
						
						for (int i = 0; i < dtRep.Rows.Count; i++) 
						{
							stationNameRep = dtRep.Rows[i][1].ToString();
							devIdxRep =  (int)dtRep.Rows[i][2];
							stationTaskRep = dtRep.Rows[i][3].ToString();
							if (stationName == stationNameRep) 
							{
								curRepRow = i;
								break;
							}
						}
						if (curRepRow == -1) 
						{
							SetTaskError(true, AlarmLv.Error,201,"The station is not defined in the recipe.",task);
							
							_mBox1Result = MessageBox.Show("The station is not defined in the recipe!","Error!", MessageBoxButtons.RetryCancel);
							SetStep(-2010);
							goto RunStepErr;
						}
						
						ITask taskT = TasksManager.Instance.GetTaskByName(stationTaskRep);
						if (taskT == null) 
						{
							SetTaskError(true, AlarmLv.Error,202,"The task is not defined in the station.",task);
							
							_mBox1Result = MessageBox.Show("The task is not defined in the station!","Error!", MessageBoxButtons.RetryCancel);
							SetStep(-2010);
							goto RunStepErr;
						}
						if(_lastTask != taskT)
						{
							DataOutputManager.Instance.ClientDataOutputManagerUpdateState("RepStep", devIdxRep.ToString());
							DataOutputManager.Instance.ClientDataOutputManagerUpdateState("RepTask", stationTaskRep);
							DataOutputManager.Instance.ClientDataOutputManagerUpdateState("UpTaskName", taskT.Name);
							LogAdd("Start RepTask - " + taskT.Name, AlarmLv.Info, 315);
							_lastTask = taskT;
							taskT.TaskRunStat = TaskRunState.Running;
							taskT.RunMode = TaskRunMode.Auto;
							Thread.Sleep(200);
							AutoTaskName = taskT.Name;
							taskT.ITaskRun();
							
							SetLLimitToDB("station6",CurRep);
							SetULimitToDB("station6",CurRep);
							SetUnitToDB("station6",CurRep);
								
							if (CheckConnectState()) 
							{
								((ISuperClient)_superClient).SendStationState("station:"+ DevRecipeManager.Instance.StationName + ";UTaskName:"+taskT.Name+";");
							}
						}
						else
						{
							taskT.TaskRunStat = TaskRunState.Running;
						}
						
					}
					
					var val = DataParamManager.Instance.GetDataParamByName("FactoryName");
					string factoryName = (val != null ? (string)val.DataVal : "Lead");
					
					if (TrayManager.Instance.GetTrayByTargetStation(DevRecipeManager.Instance.StationName.ToString(),out TrayList) != 0 || TrayList == null) 
					{
						SetTaskError(true, AlarmLv.Error,201,"Error occured when GetTrayByTargetStation.",task);
						
						_mBox1Result = MessageBox.Show("Error occured when GetTrayByTargetStation(CurStation: " + DevRecipeManager.Instance.StationName.ToString() +")!","Error!", MessageBoxButtons.RetryCancel);
						SetStep(-2010);
						goto RunStepErr;
					}
					_curTray=TrayList[0];
					DataInfoManager.Instance.DataInfoSetVal("CurTrayID", _curTray.Name);
					
					if(UnitManager.Instance.GetUnitByTargetStation(DevRecipeManager.Instance.StationName.ToString(), _curTray.Name,out _curUnit) != 0 || _curUnit == null)
					{
						SetTaskError(true, AlarmLv.Error,201,"Error occured when GetUnitByTargetStation.",task);
						
						_mBox1Result = MessageBox.Show("Error occured when GetUnitByTargetStation!","Error!", MessageBoxButtons.RetryCancel);
						SetStep(-2010);
						goto RunStepErr;
					}
					bool IsStart=false;
					//((IEleDO)_eleDoUpExitReq).Write(false);
					if(_curUnit.IsStarted(DevRecipeManager.Instance.StationName.ToString()))
					{
						IsStart=true;
					}
					else
					{
						IsStart=false;
					}
					_curUnit.StartAtStep(factoryName,"");
					string lmRet = UnitManager.Instance.GetUnitResultOfSourceStation(DevRecipeManager.Instance.StationName.ToString(),_curUnit.Name);
					
					if (lmRet.ToLower() == "ng"||IsStart)
					{
						_ngFlag = true;
						IsStart=false;
						
						DataInfoManager.Instance.DataInfoSetVal("TaskCancel", true);
						SetStep(5, "CurUnit NG, UpLine will move...");
						goto RunStep5;
					}
					DataInfoManager.Instance.DataInfoSetVal("TrayReady", true);						
				}
				#endregion*/
		   		// else
		   		// {
					   
		   			DataInfoManager.Instance.DataInfoSetVal("TrayReady", true); //send tray ready bool
		   		// }
		   		SetStep(5,"Waiting for the signal of '_eleDiUplineEntry'...");
		   	}		
			#endregion	 
		RunStep555:
			#region
			if (CheckAutoRun() && (CurStep == 555))
			{
				if(((IEleAxisLine)_eleAxisAbove).IsStop)
				{
					var val = DataParamManager.Instance.GetDataParamByName("UpSpeed1");
					int UpSpeed1 = (val != null ? (int)val.DataVal : 200);
					((IEleAxisLine)_eleAxisAbove).MC_MoveJog(true,UpSpeed1);
				}
				
				_trayInTime=DateTime.Now;
				_trayDockTime=DateTime.Now;
				SetStep(5, "CurUnit NG, write result.");
			}
			#endregion		
		RunStep5:  //wait autoTask end
			#region
			if (CheckAutoRun() && (CurStep == 5))
			{
				TaskCancel = (bool)DataInfoManager.Instance.DataInfoGetVal("TaskCancel");
				if (TaskCancel) //check if main state machine has thrown an error
				{
					DataOutputManager.Instance.ClientDataOutputManagerUpdateState("Result", "NG");
					
					_autoEndTime = DateTime.Now;
					
					DataInfoManager.Instance.DataInfoSetVal("TotalAutoTime", (_autoEndTime-_trayDockTime).TotalSeconds);
					
					TaskResult = -1;
					
					SetStep(601, "Task Aborted, UpLine will go forward...");
					goto RunStep601;
				}
				
				TrayReady = (bool)DataInfoManager.Instance.DataInfoGetVal("TrayReady");
				if (!TrayReady) //if not tray ready. tray ready is set here, but maybe tray ready is unset in main state machine
				{
					_autoEndTime = DateTime.Now;
					
					DataInfoManager.Instance.DataInfoSetVal("TotalAutoTime", (_autoEndTime-_trayDockTime).TotalSeconds);
					
					TaskResult = CheckDryRun() ? 0 : (int)DataInfoManager.Instance.DataInfoGetVal("TaskResult");
					
					SetStep(601, "Assembly action completed, CylRaise will retract...");
				}
			}
			#endregion	
		RunStep601:  //CylBlock Retract
			#region
			if (CheckAutoRun() && (CurStep == 601))
			{
				
				if(((IEleCyldLine)_eleCylBlock).CyldRetract(true, false) != 0) //this block was retracted earlier but double check
				{
					_trayReleaseStart=DateTime.Now;
					SetTaskError(true, AlarmLv.Error, 601, "'_eleCylBlock' retract failed!", task);
					_mBox1Result = MessageBox.Show("'_eleCylBlock' retracted failed!","Error!", MessageBoxButtons.RetryCancel);
					SetStep(6010);
				}
				else
				{
					SetStep(602, "'_eleCylRaise' will retract...");
				}
			}
			#endregion
		RunStep6010:
			#region
			if (CheckAutoRun() && (CurStep == 6010))
			{
				if (_mBox1Result == DialogResult.Retry)
				{
					SetStep(601, "'_eleCylBlock' will retract...");
				}
				else
				{
					SetTaskError(true, AlarmLv.Error, 601, "Error...", task);
					goto RunStepErr;
				}
			}
			#endregion	
		RunStep602: //CylPush Retract
			#region
			if (CheckAutoRun() && (CurStep == 602))
			{
				if(((IEleCyldLine)_eleCylBlock).ReadRestractDI() == 1)
				{
					if(((IEleCyldLine)_eleCylPush).CyldRetract(true, false) != 0)
					{
						SetTaskError(true, AlarmLv.Error, 602, "'_eleCylPush' retract failed!", task);
						_mBox1Result = MessageBox.Show("'_eleCylPush' retracted failed!","Error!", MessageBoxButtons.RetryCancel);
						SetStep(6020);
					}
					else
					{
						SetStep(603, "'_eleCylRaise' will retract...");
					}
				}
				else
				{
					SetStep(601, "Ensure the signal of '_eleCylBlock'...");
				}			
			}
			#endregion			
		RunStep6020:
			#region
			if (CheckAutoRun() && (CurStep == 6020))
			{
				if (_mBox1Result == DialogResult.Retry)
				{
					SetStep(602, "'_eleCylPush' will retract...");
				}
				else
				{
					SetTaskError(true, AlarmLv.Error, 602, "Error...", task);
					goto RunStepErr;
				}
			}
			#endregion		
		RunStep603: //CylRaise Retract
			#region
			if (CheckAutoRun() && (CurStep == 603))
			{
				if(((IEleCyldLine)_eleCylBlock).ReadRestractDI() == 1)
				{
					if(((IEleCyldLine)_eleCylRaise).CyldRetract(true, false) != 0)
					{
						SetTaskError(true, AlarmLv.Error, 603 ,"'_eleCylRaise' retract failed!", task);
						_mBox1Result = MessageBox.Show("'_eleCylRaise' retracted failed!","Error!", MessageBoxButtons.RetryCancel);
						SetStep(6030);
					}
					else
					{
						
						_trayReleaseTime = DateTime.Now;
						SetStep(7, "Waiting for end...");
					}
				}
				else
				{
					SetStep(601, "Ensure the signal of '_eleCylBlock'...");
				}			
			}
			#endregion		
		RunStep6030:
			#region
			if (CheckAutoRun() && (CurStep == 6030))
			{
				if (_mBox1Result == DialogResult.Retry)
				{
					SetStep(603, "'_eleCylRaise' will retract...");
				}
				else
				{
					SetTaskError(true, AlarmLv.Error, 603, "Error...", task);
					goto RunStepErr;
				}
			}
			#endregion		
		RunStep7: //step description is waiting for end, does nothing in unlock mode
			#region
			if (CheckAutoRun() && (CurStep == 7))
			{
				if (((IEleDI)_eleBtnLock).Read() && ((IEleDI)_eleDiUpExitReq).Read() )		//Mode of 'Lock', and receive the signal of 'UpExitReq'
				{
					if(TrayManager.Instance.IsTargetStationFree(CurRep,DevRecipeManager.Instance.StationName.ToString()))
					{
						/* if(((IEleDI)_eleBtnLock).Read()&&!SqlCheckErr)
						{
							if (TaskResult == 0) 
							{
								_curTray.CompleteTrayStep(CurRep);
								_curUnit.CompleteAtStep(string.Empty,"OK",null,out nextIndex);
								DataOutputManager.Instance.ClientDataOutputManagerUpdateState("Result", "OK");
							}
							else
							{
								_curTray.CompleteTrayStep(CurRep);
								_curUnit.CompleteAtStep(string.Empty,"NG",null,out nextIndex);
								DataOutputManager.Instance.ClientDataOutputManagerUpdateState("Result", "NG");
							}
						} */
						LogAdd("Mode of 'Lock', and receive the signal of 'UpExitReq'", AlarmLv.Info,7);
						SetStep(8,"Mode of 'Lock', and UpLine will be started!");	
					}	
					else
					{
						MessageBox.Show("TargetStation is not free,please check it");
						LogAdd("TargetStation is not free,please check it", AlarmLv.Info,7);
						Thread.Sleep(100);
					}
				}			
				if (!((IEleDI)_eleBtnLock).Read() && ((IEleDI)_eleDiUpExitReq).Read())		//Mode of 'UnLock', and receive the signal of 'UpLineEntry'
				{
					LogAdd("Mode of 'UnLock', and receive the signal of 'UpLineEntry'", AlarmLv.Info,7);
					SetStep(8,"Mode of 'UnLock', and UpLine will be started!");
				}
			}
			#endregion	
		RunStep8:
			#region
			if (CheckAutoRun() && (CurStep == 8))
			{	
				//check that all cylinders are out of the way
				if ((((IEleCyldLine)_eleCylPush).ReadRestractDI() == 1) && (((IEleCyldLine)_eleCylBlock).ReadRestractDI() == 1) && (((IEleCyldLine)_eleCylRaise).ReadRestractDI() == 1))
				{
					if (((IEleDI)_eleBtnLock).Read()) //if lock mode sleep, why?
					{
						Thread.Sleep(200);
					}
					
					((IEleDO)_eleDoUpExitReq).Write(true);
					_trayStartAgainTime = DateTime.Now; //record exit time
					_delayTime=(DateTime.Now-_trayReleaseTime).TotalSeconds;
					
					var val = DataParamManager.Instance.GetDataParamByName("UpSpeed1");
					int UpSpeed1 = (val != null ? (int)val.DataVal : 80); //outflow tray
					((IEleAxisLine)_eleAxisAbove).MC_MoveJog(true, UpSpeed1);
					
					SetStep(9, "Wait for DiUplineExit...");
				}
			}
			#endregion	
		RunStep9:
			#region
			if (CheckAutoRun() && (CurStep == 9))
			{
				if(((IEleAxisLine)_eleAxisAbove).IsStop) //if not moving, move
				{
					var val = DataParamManager.Instance.GetDataParamByName("UpSpeed1");
					int UpSpeed1 = (val != null ? (int)val.DataVal : 80);
					((IEleAxisLine)_eleAxisAbove).MC_MoveJog(true,UpSpeed1);
				}
				
				if (((IEleDI)_eleDiUplineExit).Read()) //if exit sensor reads, move to next step
				{
					
					//SqlCheckErr=false;
					SetStep(10, "Wait for DiUplineExit disappeared..");
				}
			}
			#endregion	
		RunStep10: 
			#region
			if (CheckAutoRun() && (CurStep == 10))
			{
				if(((IEleAxisLine)_eleAxisAbove).IsStop) //if stopped, move
				{
					var val = DataParamManager.Instance.GetDataParamByName("UpSpeed1");
					int UpSpeed1 = (val != null ? (int)val.DataVal : 80);
					((IEleAxisLine)_eleAxisAbove).MC_MoveJog(true,UpSpeed1);
				}
				
				if (!((IEleDI)_eleDiUplineExit).Read()&&!((IEleDI)_eleDiUplineWork).Read()) //if work and exit sensors are low
				{
					Thread.Sleep(1000);
					if(!((IEleDI)_eleDiUplineExit).Read()&&!((IEleDI)_eleDiUplineWork).Read()) //this statement is identical to the one above
					{
						if (((IEleDI)_eleBtnLock).Read()) //if lock
						{
							((IEleDO)_eleDoUpEntryReq).Write(false); //turn off entry requirement
						}
						((IEleDO)_eleDoUpEntryReq).Write(false); //turn off entry requirement
						SetStep(100, "UpConvey will be stop, then current task will be stop.");
					}	
				}
			}
			#endregion			
		RunStep100:
			#region
			if (CheckAutoRun() && (CurStep == 100))
			{
				// Stop upline and record tray out time
				((IEleAxisLine)_eleAxisAbove).MC_Stop();
				_trayOutTime=DateTime.Now;

				// Turn off exit request
				AutoRunning=false;
				((IEleDO)_eleDoUpExitReq).Write(false);
				
				// Calculate time elapsed
				double _blockTime1 = (_trayDockTime - _trayDockStart).TotalSeconds;
				double _blockTime2 = ( _trayReleaseTime - _autoEndTime).TotalSeconds;
				double _transferTime1 = (_trayDockStart - _trayInTime).TotalSeconds;
				double _transferTime2 = ( _trayOutTime - _trayStartAgainTime ).TotalSeconds;
				string _ct = (_autoEndTime-_trayDockTime).TotalSeconds.ToString("0.000");
				string _carrierBlockTime = (_blockTime1 + _blockTime2).ToString("0.000");
				string _transferTime = (_transferTime1 + _transferTime2).ToString("0.000");
				string _strDownTime = _downTime.ToString("0.000");
				
				/*
				string capResultX1 = "";
	 			string capResultY1 = "";
	 			string capResultX2 = "";
	 			string capResultY2 = "";
	 			string capResultX3 = "";
	 			string capResultY3 = "";
	 			string capResultX4 = "";
	 			string capResultY4 = "";
				string capResultX5 = "";
	 			string capResultY5 = "";
	 			string screw1TryTime = "";
		 		string screw2TryTime = "";
		 		string screw3TryTime = "";
		 		string screw4TryTime = "";
				string screw5TryTime = "";
				*/
				
				//record data
				#region
				
				string taskRet = "";
				if (TaskResult == 0) 
				{
					taskRet = "PASS";
				}
				else
				{
					taskRet = "FAIL";
				}			

		 		if(CheckDryRun())
		 		{
		 			ModeString = "DryRun";
		 		}
		 		else
		 		{
		 			ModeString = "Auto";		 		
		 		}
		 		
				// Try read barcode and get data from DataInfo
				try
				{
					// Read barcode from BarCode.txt - generated by vpp
					barcodesr = new StreamReader("D:\\AllCSV\\BarCode.txt");
					while(!barcodesr.EndOfStream)
					{
						// May simplify to remove while loop
						TrayID = barcodesr.ReadLine();
					}
					barcodesr.Close();
					
					// ---- Data collection ----
					// 1. CSV title gathered from machine specific template
					// 2. Logged data ordered in csvdata list (see code below)
					// Need more robust code to reduce work during machine retrofit
					// Preferably a standard RunStep100 across machines

					// Get info from CPrim interface on image capture result
					var val = DataInfoManager.Instance.DataInfoGetVal("Capture");
					Capture = (val != null)?Convert.ToBoolean(val):false;

					val = DataInfoManager.Instance.DataInfoGetVal("CapResultX1");
					CapResultX1 = (val != null)?Convert.ToDouble(val):0;
					// capResultX1 = CapResultX1.ToString("#.000");

					val = DataInfoManager.Instance.DataInfoGetVal("CapResultY1");
					CapResultY1 = (val != null)?Convert.ToDouble(val):0;
					// capResultY1 = CapResultY1.ToString("#.000");

					val = DataInfoManager.Instance.DataInfoGetVal("CapResultX2");
					CapResultX2 = (val != null)?Convert.ToDouble(val):0;
					// capResultX2 = CapResultX2.ToString("#.000");

					val = DataInfoManager.Instance.DataInfoGetVal("CapResultY2");
					CapResultY2 = (val != null)?Convert.ToDouble(val):0;
					// capResultY2 = CapResultY2.ToString("#.000");

					val = DataInfoManager.Instance.DataInfoGetVal("CapResultX3");
					CapResultX3 = (val != null)?Convert.ToDouble(val):0;
					// capResultX3 = CapResultX3.ToString("#.000");

					val = DataInfoManager.Instance.DataInfoGetVal("CapResultY3");
					CapResultY3 = (val != null)?Convert.ToDouble(val):0;				
					// capResultY3 = CapResultY3.ToString("#.000");

					val = DataInfoManager.Instance.DataInfoGetVal("CapResultX4");
					CapResultX4 = (val != null)?Convert.ToDouble(val):0;
					// capResultX4 = CapResultX4.ToString("#.000");

					val = DataInfoManager.Instance.DataInfoGetVal("CapResultY4");
					CapResultY4 = (val != null)?Convert.ToDouble(val):0;
					// capResultY4 = CapResultY4.ToString("#.000")

					val = DataInfoManager.Instance.DataInfoGetVal("CapResultX5");
					CapResultX5 = (val != null)?Convert.ToDouble(val):0;
					// capResultX5 = CapResultX5.ToString("#.000")

					val = DataInfoManager.Instance.DataInfoGetVal("CapResultY5");
					CapResultY5 = (val != null)?Convert.ToDouble(val):0;
					// capResultY5 = CapResultY5.ToString("#.000");		
					
					val =DataInfoManager.Instance.DataInfoGetVal("Screw1TryTime");
					Screw1TryTime=(val!=null)?Convert.ToInt32(val):0;
					// screw1TryTime = Screw1TryTime.ToString();

					val =DataInfoManager.Instance.DataInfoGetVal("Screw2TryTime");
					Screw2TryTime=(val!=null)?Convert.ToInt32(val):0;
					// screw2TryTime = Screw2TryTime.ToString();

					val =DataInfoManager.Instance.DataInfoGetVal("Screw3TryTime");
					Screw3TryTime=(val!=null)?Convert.ToInt32(val):0;
					// screw3TryTime = Screw3TryTime.ToString();

					val =DataInfoManager.Instance.DataInfoGetVal("Screw4TryTime");
					Screw4TryTime=(val!=null)?Convert.ToInt32(val):0;
					// screw4TryTime = Screw4TryTime.ToString();

					val =DataInfoManager.Instance.DataInfoGetVal("Screw5TryTime");
					Screw5TryTime=(val!=null)?Convert.ToInt32(val):0;
					// screw5TryTime = Screw5TryTime.ToString();

					val= DataInfoManager.Instance.DataInfoGetVal("ErrID");
					ErrID=(val!=null)?(string)val:"na";

					
				}
				
				catch(Exception e)
				{
					MessageBox.Show("Barcode read or DataInfoGetVal error: "+ e.ToString());
				}
				
				// Try to build CSV strings and write to CSV
				try
				{
					// Build a list of process data that goes into CSV
					List<string> csvData = new List<string>();
						
					if(((IEleDI)_eleBtnLock).Read())
					{	
						// Line manager level data available if in Lock mode 				
						csvData.Add(DSN);
						csvData.Add(TrayID);
						csvData.Add(lineConfig.FactoryID);
						csvData.Add(lineConfig.ProjectName);
						csvData.Add(lineConfig.BuildConfiguration);
						csvData.Add(lineConfig.StartTime);
						csvData.Add(CurStationName);
						csvData.Add(AutoTaskName);
					}
					else
					{
						// Otherwise, get these information elsewhere
						csvData.Add("na");
						csvData.Add(TrayID);
						csvData.Add(FactoryID);
						csvData.Add(ProjectName);
						csvData.Add(BuildConfig);
						csvData.Add(DateTime.Now.ToString("yyyyMMdd_HHmmss"));
						csvData.Add(CurStationName);
						csvData.Add(AutoTaskName);
					}
						
					// Cycle time, delay time, and timestamp at each tray stage
					csvData.Add((_autoEndTime-_trayDockTime).TotalSeconds.ToString());
					csvData.Add((_trayReleaseTime-_autoEndTime).TotalSeconds.ToString());
					csvData.Add(_trayInTime.ToString("yyyyMMdd_HHmmss"));
					csvData.Add(_trayDockTime.ToString("yyyyMMdd_HHmmss"));
					csvData.Add(_trayReleaseTime.ToString("yyyyMMdd_HHmmss"));
					csvData.Add(_trayOutTime.ToString("yyyyMMdd_HHmmss"));
						
					// Conveyor speeds
					csvData.Add(above_speed);
					csvData.Add(below_speed);
					
					// Axes speeds			 				
					csvData.Add(((IEleAxisLine)_eleAxisX).DefaultParams.Velocity.ToString());
					csvData.Add(((IEleAxisLine)_eleAxisY).DefaultParams.Velocity.ToString());
					csvData.Add(((IEleAxisLine)_eleAxisZ).DefaultParams.Velocity.ToString());

					// image capture OK/NG
					//bool part_inspect= DataParamManager.Instance.GetDataParamByName("Dector");
					csvData.Add(Dector.ToString()); //part_inspect
					csvData.Add(Capture.ToString()); //host_inspect


					// Screw 1
					//csvData.Add(Capture1.ToString()); // Whether screw exist
					csvData.Add(Screw1TryTime.ToString()); // Try Screw1TryTime, whether need to be in string or int32 is ok
					csvData.Add(CapResultX1.ToString("#.000"));
					csvData.Add(CapResultY1.ToString("#.000"));

					// Screw 2
					//csvData.Add(Capture2.ToString());
					csvData.Add(Screw2TryTime.ToString()); // Try Screw2TryTime, whether need to be in string or int32 is ok
					csvData.Add(CapResultX2.ToString("#.000"));
					csvData.Add(CapResultY2.ToString("#.000"));

					// Screw 3
					//csvData.Add(Capture3.ToString());
					csvData.Add(Screw3TryTime.ToString()); // Try Screw3TryTime, whether need to be in string or int32 is ok
					csvData.Add(CapResultX3.ToString("#.000"));
					csvData.Add(CapResultY3.ToString("#.000"));

					// Screw 4
					//csvData.Add(Capture4.ToString());
					csvData.Add(Screw4TryTime.ToString()); // Try Screw4TryTime, whether need to be in string or int32 is ok
					csvData.Add(CapResultX4.ToString("#.000"));
					csvData.Add(CapResultY4.ToString("#.000"));

					// Screw 5
					// csvData.Add(Capture5.ToString());
					csvData.Add(Screw5TryTime.ToString()); // Try Screw5TryTime, whether need to be in string or int32 is ok
					csvData.Add(CapResultX5.ToString("#.000"));
					csvData.Add(CapResultY5.ToString("#.000"));
				
					// Overall status
					csvData.Add(taskRet);
					csvData.Add(ErrID.ToString());
						
					// Combine csvData into comma separated string
					AllDataString = string.Join(",", csvData);

					// Update WriteAllData to remove VisionProString input argument
					WriteAllData(AllDataString);

					// Set error id as "na" on DataInfo (Reset ErrID)
					DataInfoManager.Instance.DataInfoSetVal("ErrID","na");
				}
				
				catch(Exception e)
				{
					MessageBox.Show("Error building CSV string or writing to CSV: "+ e.ToString());
				}
				
				if(TaskCancel)
				{
					TaskCancel = false;
					DataInfoManager.Instance.DataInfoSetVal("TaskCancel", false);
					((IEleCyldLine)_eleCylPush).CyldRetract();
					((IEleCyldLine)_eleCylBlock).CyldRetract();
					((IEleCyldLine)_eleCylRaise).CyldRetract();
					Thread.Sleep(500);
					//task.CurState = TaskMachineState.Error;
					
				}
				_downTime=0;
				
				#endregion
				SetStep(1, "Make sure it is the discharge status...");
				goto RunStep1;
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
		#region Get Prims
		//if (!GetPrim("MySql0", ref _mySql, 5)) goto TaskInitErr;
		if (!GetPrim("SuperClient0", ref _superClient, 6)) goto TaskInitErr;
		#endregion

		#region Get Eles
		if (!GetEle("Btn_Lock", ref _eleBtnLock, 100)) goto TaskInitErr;
		if (!GetEle("UpLineEntry", ref _eleDiUplineEntry, 101)) goto TaskInitErr;
		if (!GetEle("UpLineExit", ref _eleDiUplineExit, 102)) goto TaskInitErr;
		if (!GetEle("UpLineWork", ref _eleDiUplineWork, 103)) goto TaskInitErr;
		if (!GetEle("UpLineCheck", ref _eleDiUpLineCheck, 104)) goto TaskInitErr;
		if (!GetEle("UpEntryReq", ref _eleDiUpEntryReq, 105)) goto TaskInitErr;
		if (!GetEle("UpExitReq", ref _eleDiUpExitReq, 106)) goto TaskInitErr;
		if (!GetEle("DnEntryReq", ref _eleDiDnEntryReq, 107)) goto TaskInitErr;
		if (!GetEle("DnExitReq", ref _eleDiDnExitReq, 108)) goto TaskInitErr;
		if (!GetEle("UpEntryReqO", ref _eleDoUpEntryReq, 109)) goto TaskInitErr;
		if (!GetEle("UpExitReqO", ref _eleDoUpExitReq, 110)) goto TaskInitErr;
		if (!GetEle("DnEntryReqO", ref _eleDoDnEntryReq, 111)) goto TaskInitErr;
		if (!GetEle("DnExitReqO", ref _eleDoDnExitReq, 112)) goto TaskInitErr;

		if (!GetEle("Line1", ref _eleAxisAbove, 120)) goto TaskInitErr;
		if (!GetEle("Line2", ref _eleAxisBelow, 121)) goto TaskInitErr;
		if (!GetEle("AxisX", ref _eleAxisX, 122)) goto TaskInitErr;
		if (!GetEle("AxisY", ref _eleAxisY, 123)) goto TaskInitErr;
		if (!GetEle("AxisZ", ref _eleAxisZ, 124)) goto TaskInitErr;
		//if (!GetEle("AxisU", ref _eleAxisU, 125)) goto TaskInitErr;
		if (!GetEle("CylBlock", ref _eleCylBlock, 126)) goto TaskInitErr;
		if (!GetEle("CylRaise", ref _eleCylRaise, 127)) goto TaskInitErr;
		if (!GetEle("CylPush", ref _eleCylPush, 128)) goto TaskInitErr;
		#endregion
		
		
		#region Others
		
		#endregion
		IDataParam valSocket = DataParamManager.Instance.GetDataParamByName("SocketEnable");
		SocketEnable = (valSocket != null ? (bool)valSocket.DataVal : false);

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
				Log.Fatal(151000000+id,log,LogClassification.Task,TaskName);
				break;
			case AlarmLv.Error:
				Log.Error(251000000+id,log,LogClassification.Task,TaskName);
				break;
			case AlarmLv.Warn:
				Log.Warn(351000000+id,log,LogClassification.Task,TaskName);
				break;
			case AlarmLv.Info:
				Log.Info(451000000+id,log,LogClassification.Task,TaskName);
				break;
			case AlarmLv.Debug:
				Log.Debug(551000000+id,log,LogClassification.Task,TaskName);
				break;
			case AlarmLv.Trace:
				Log.Trace(651000000+id,log,LogClassification.Task,TaskName);
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
		// CurFileDate="Screw1-"+GetDayString(DateTime.Now);
		CurFileDate=CurStationName+"_"+TrayID+"_"+DateTime.Now.ToString("yyyyMMdd_HHmmss")+".csv";
		AllRunDataFullPath=AllRunDataPath + CurFileDate;
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
	private void WriteAllVisionData(string AutoString = "")
	{
		string VisionFileDate="Vpp_Screw1-"+GetDayString(DateTime.Now);
		VppRunDataFullPath=AllRunDataPath+VisionFileDate;
		if(File.Exists(VppRunDataFullPath)==false)
		{
			CreateNewVisionCSV(VppRunDataFullPath);
		}
		
		AllDatafs = new FileStream(VppRunDataFullPath, System.IO.FileMode.Append, System.IO.FileAccess.Write);
		AllDatasw = new StreamWriter(AllDatafs, System.Text.Encoding.Default);
        AllDatasw.AutoFlush = true;
        if(AutoString != "")
        	AllDatasw.WriteLine(AutoString);
    	
    	AllDatasw.Close();
    	AllDatafs.Close();
	}

	private void CreateNewCSV(string filePath)
	{
		// Create new CSV, which will have same header as template.csv
		// 
		// If the directory path does not exist, create it
        if(!System.IO.Directory.Exists(TemplateBarcodePath))
			System.IO.Directory.CreateDirectory(TemplateBarcodePath);
		
		        // Open new file stream for template.csv
        templatefs = new FileStream(TemplateBarcodePath+"template.csv", System.IO.FileMode.Open, System.IO.FileAccess.Read);
        
        // Open new filestream for new csv
        AllDatafs = new FileStream(filePath, System.IO.FileMode.Append, System.IO.FileAccess.Write);
		AllDatasw = new StreamWriter(AllDatafs, System.Text.Encoding.Default);
        AllDatasw.AutoFlush = true;

        try
        {
            // Create an instance of StreamReader to read from template.csv.
            // The using statement also closes the StreamReader.
            using (StreamReader templatesr = new StreamReader(TemplateBarcodePath+"template.csv"))
            {
                string srline;
                string[] arrayline;
                // Read until end of file
                while (!templatesr.EndOfStream)
                {
                    srline = templatesr.ReadLine();
                	AllDatasw.WriteLine(srline);
                	
                    // string is separated by comma store each into array
                    arrayline = srline.Split(',');
                }
            }
        }
        catch (Exception e)
        {
            // Display what went wrong.
            MessageBox.Show("The file (template.csv) could not be read:");
            MessageBox.Show(e.Message);
        }

        templatefs.Close();    	
    	AllDatasw.Close();
    	AllDatafs.Close();
	}
	private void CreateNewVisionCSV(string filePath)
	{
		AllDatafs = new FileStream(filePath, System.IO.FileMode.Append, System.IO.FileAccess.Write);
		AllDatasw = new StreamWriter(AllDatafs, System.Text.Encoding.Default);
        AllDatasw.AutoFlush = true;

        string tempstr="No,TrayID,Time,Mode,Station,Pic1,Pic2,Pic3,Pic4,Pic5 X1,Pic5 Y1,Pic5 X2,Pic5 Y2,Pic5 X3,Pic5 Y3,Pic5 X4,Pic5 Y4,Pic 6,Pass or Fail";
    	AllDatasw.WriteLine(tempstr);
    	
    	AllDatasw.Close();
    	AllDatafs.Close();
	}
	public string DictionaryToString(Dictionary<string,string> resultItems)
	{
		if(resultItems.Count < 1)
			return null;
		StringBuilder sb = new StringBuilder();
		foreach(var item in resultItems)
		{
			sb.Append(item.Key + ",");
			sb.Append(item.Value);
			sb.Append(";");
		}
		return sb.ToString();
	}
	
	public string DictToString_Socket(Dictionary<string,string> resultItems)
	{
		if(resultItems.Count < 1)
			return null;
		StringBuilder sb = new StringBuilder();
		foreach(var item in resultItems)
		{
			sb.Append(item.Key + ":");
			sb.Append(item.Value);
			sb.Append(";");
		}
		return sb.ToString();
	}
	
	public bool CheckConnectState()
    {
		object obj = DataInfoManager.Instance.DataInfoGetVal("ConnState");
		ConnState = (obj != null) ? (bool)obj : false;

		if(SocketEnable && ConnState)
		{
			return true;
		}
		return false;
    }
	public void SetULimitToDB(string stepName,string curRep)
	{
       	Dictionary<string,string> resultItems = new Dictionary<string, string>();
		//resultItems.Add("DSN",DSN);
		resultItems.Add("station_id","na");
		resultItems.Add("no_of_parts","na");
		resultItems.Add("part_name","na");
		resultItems.Add("task_id","na");
		resultItems.Add("cycle_time","na");
		resultItems.Add("delay_time","na");
		resultItems.Add("part_barcode","na");
		resultItems.Add("tray_enterance_time","na");
		resultItems.Add("tray_dock_time","na");
		resultItems.Add("tray_release_time","na");
		resultItems.Add("tray_exit_time", "na");
		resultItems.Add("run_mode", "na");
		resultItems.Add("top_conveyer_speed",top_conveyer_speed_max);
		resultItems.Add("bottom_conveyer_speed",bottom_conveyer_speed_max);
		resultItems.Add("x_axis_speed",	x_speed_max);
		resultItems.Add("y_axis_speed",y_speed_max);
		resultItems.Add("z_axis_speed",z_speed_max);
		resultItems.Add("r_axis_speed",r_speed_max);
		resultItems.Add("capture_the_part","na");
		resultItems.Add("capture_the_host","na");
		resultItems.Add("capture_finally","na");
		resultItems.Add("part_x_offset","na");
		resultItems.Add("part_y_offset","na");
		resultItems.Add("part_r_offset","na");
		resultItems.Add("host_x_offset","na");
		resultItems.Add("host_y_offset","na");
		resultItems.Add("host_r_offset","na");
		resultItems.Add("x_offset_to_robot",x_offset_robot_max);
		resultItems.Add("y_offset_to_robot",y_offset_robot_max);
		resultItems.Add("r_offset_to_robot",r_offset_robot_max);
		resultItems.Add("x_offset_result","na");
		resultItems.Add("y_offset_result","na");
		resultItems.Add("r_offset_result","na");
		resultItems.Add("critical_errors_id","na");
		resultItems.Add("screw_torque",screw_torque_max);
		resultItems.Add("pressure_value",press_value_max);
		resultItems.Add("press_time",press_time_max);
		resultItems.Add("1_delta_x_for_screw_station",delta_1_x_for_screw_station_max);
		resultItems.Add("1_delta_y_for_screw_station",delta_1_y_for_screw_station_max);
		resultItems.Add("2_delta_x_for_screw_station",delta_2_x_for_screw_station_max);
		resultItems.Add("2_delta_y_for_screw_station",delta_2_y_for_screw_station_max);
		resultItems.Add("3_delta_x_for_screw_station",delta_3_x_for_screw_station_max);
		resultItems.Add("3_delta_y_for_screw_station",delta_3_y_for_screw_station_max);
		resultItems.Add("4_delta_x_for_screw_station",delta_4_x_for_screw_station_max);
		resultItems.Add("4_delta_y_for_screw_station",delta_4_y_for_screw_station_max);
		resultItems.Add("1_screw_try_time",screw_1_try_time_max);
		resultItems.Add("2_screw_try_time",screw_2_try_time_max);
		resultItems.Add("3_screw_try_time",screw_3_try_time_max);
		resultItems.Add("4_screw_try_time",screw_4_try_time_max);
		resultItems.Add("ok_ng_name","na");
		resultItems.Add("arbitrary_variable_1","na");
		resultItems.Add("arbitrary_variable_2","na");
		UnitManager.Instance.WriteResultItem(null,stepName,resultItems,"itemulimit",curRep);
	}
	public void SetLLimitToDB(string stepName,string curRep)
	{
       	Dictionary<string,string> resultItems = new Dictionary<string, string>();
		//resultItems.Add("DSN",DSN);
		resultItems.Add("station_id","na");
		resultItems.Add("no_of_parts","na");
		resultItems.Add("part_name","na");
		resultItems.Add("task_id","na");
		resultItems.Add("cycle_time","na");
		resultItems.Add("delay_time","na");
		resultItems.Add("part_barcode","na");
		resultItems.Add("tray_enterance_time","na");
		resultItems.Add("tray_dock_time","na");
		resultItems.Add("tray_release_time","na");
		resultItems.Add("tray_exit_time", "na");
		resultItems.Add("run_mode", "na");
		resultItems.Add("top_conveyer_speed",top_conveyer_speed_min);
		resultItems.Add("bottom_conveyer_speed",bottom_conveyer_speed_min);
		resultItems.Add("x_axis_speed",	x_speed_min);
		resultItems.Add("y_axis_speed",y_speed_min);
		resultItems.Add("z_axis_speed",z_speed_min);
		resultItems.Add("r_axis_speed",r_speed_min);
		resultItems.Add("capture_the_part","na");
		resultItems.Add("capture_the_host","na");
		resultItems.Add("capture_finally","na");
		resultItems.Add("part_x_offset","na");
		resultItems.Add("part_y_offset","na");
		resultItems.Add("part_r_offset","na");
		resultItems.Add("host_x_offset","na");
		resultItems.Add("host_y_offset","na");
		resultItems.Add("host_r_offset","na");
		resultItems.Add("x_offset_to_robot",x_offset_robot_min);
		resultItems.Add("y_offset_to_robot",y_offset_robot_min);
		resultItems.Add("r_offset_to_robot",r_offset_robot_min);
		resultItems.Add("x_offset_result","na");
		resultItems.Add("y_offset_result","na");
		resultItems.Add("r_offset_result","na");
		resultItems.Add("critical_errors_id","na");
		resultItems.Add("screw_torque",screw_torque_min);
		resultItems.Add("pressure_value",press_value_min);
		resultItems.Add("press_time",press_time_min);
		resultItems.Add("1_delta_x_for_screw_station",delta_1_x_for_screw_station_min);
		resultItems.Add("1_delta_y_for_screw_station",delta_1_y_for_screw_station_min);
		resultItems.Add("2_delta_x_for_screw_station",delta_2_x_for_screw_station_min);
		resultItems.Add("2_delta_y_for_screw_station",delta_2_y_for_screw_station_min);
		resultItems.Add("3_delta_x_for_screw_station",delta_3_x_for_screw_station_min);
		resultItems.Add("3_delta_y_for_screw_station",delta_3_y_for_screw_station_min);
		resultItems.Add("4_delta_x_for_screw_station",delta_4_x_for_screw_station_min);
		resultItems.Add("4_delta_y_for_screw_station",delta_4_y_for_screw_station_min);
		resultItems.Add("1_screw_try_time",screw_1_try_time_min);
		resultItems.Add("2_screw_try_time",screw_2_try_time_min);
		resultItems.Add("3_screw_try_time",screw_3_try_time_min);
		resultItems.Add("4_screw_try_time",screw_4_try_time_min);
		resultItems.Add("ok_ng_name","na");
		resultItems.Add("arbitrary_variable_1","na");
		resultItems.Add("arbitrary_variable_2","na");
		UnitManager.Instance.WriteResultItem(null,stepName,resultItems,"itemllimit",curRep);
	}
	public void SetUnitToDB(string stepName,string curRep)
	{
      	Dictionary<string,string> resultItems = new Dictionary<string, string>();
		//resultItems.Add("DSN",DSN);
		resultItems.Add("station_id","na");
		resultItems.Add("no_of_parts","na");
		resultItems.Add("part_name","na");
		resultItems.Add("task_id","na");
		resultItems.Add("cycle_time","s");
		resultItems.Add("delay_time","s");
		resultItems.Add("part_barcode","na");
		resultItems.Add("tray_enterance_time","Date_time");
		resultItems.Add("tray_dock_time","Date_time");
		resultItems.Add("tray_release_time","Date_time");
		resultItems.Add("tray_exit_time", "Date_time");
		resultItems.Add("run_mode", "na");
		resultItems.Add("top_conveyer_speed","mm/s");
		resultItems.Add("bottom_conveyer_speed","mm/s");
		resultItems.Add("x_axis_speed",	"mm/s");
		resultItems.Add("y_axis_speed","mm/s");
		resultItems.Add("z_axis_speed","mm/s");
		resultItems.Add("r_axis_speed","mm/s");
		resultItems.Add("capture_the_part","na");
		resultItems.Add("capture_the_host","na");
		resultItems.Add("capture_finally","na");
		resultItems.Add("part_x_offset","mm");
		resultItems.Add("part_y_offset","mm");
		resultItems.Add("part_r_offset","mm");
		resultItems.Add("host_x_offset","mm");
		resultItems.Add("host_y_offset","mm");
		resultItems.Add("host_r_offset","mm");
		resultItems.Add("x_offset_to_robot","mm");
		resultItems.Add("y_offset_to_robot","mm");
		resultItems.Add("r_offset_to_robot","mm");
		resultItems.Add("x_offset_result","mm");
		resultItems.Add("y_offset_result","mm");
		resultItems.Add("r_offset_result","mm");
		resultItems.Add("critical_errors_id","na");
		resultItems.Add("screw_torque","N/m");
		resultItems.Add("pressure_value","N");
		resultItems.Add("press_time","s");
		resultItems.Add("1_delta_x_for_screw_station","mm");
		resultItems.Add("1_delta_y_for_screw_station","mm");
		resultItems.Add("2_delta_x_for_screw_station","mm");
		resultItems.Add("2_delta_y_for_screw_station","mm");
		resultItems.Add("3_delta_x_for_screw_station","mm");
		resultItems.Add("3_delta_y_for_screw_station","mm");
		resultItems.Add("4_delta_x_for_screw_station","mm");
		resultItems.Add("4_delta_y_for_screw_station","mm");
		resultItems.Add("1_screw_try_time","na");
		resultItems.Add("2_screw_try_time","na");
		resultItems.Add("3_screw_try_time","na");
		resultItems.Add("4_screw_try_time","na");
		resultItems.Add("ok_ng_name","na");
		resultItems.Add("arbitrary_variable_1","na");
		resultItems.Add("arbitrary_variable_2","na");
		UnitManager.Instance.WriteResultItem(null,stepName,resultItems,"itemunit",curRep);
	}
}
