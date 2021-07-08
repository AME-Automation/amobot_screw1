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
	private IPrim _mySql = null;
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
	
	public bool SqlCheckErr=false;
	
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
	private string TaskName = "";
	private string AutoTaskName = "";
	
	private bool _recipeSel = false;
	
	private bool firstStart=true;
	
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
	
	private bool Capture1=false;
	private bool Capture2=false;
	private double CapResultX=0;
	private double CapResultY=0;
	private double CapResultR=0;
	private bool Dector=false;
	
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
	
	private bool DryRunMode=false;
	private string ModeString="";
	private double delayTime=0;
	private FileStream CTLogfs,CTTimefs,AllDatafs;
	private StreamWriter CTLogsw,CTTimesw,AllDatasw;
	private string CTLogPath="D:\\TotalCTFile.txt";
	private string CTTimePath="D:\\TotalTimeFile.txt";
	private string AllRunDataPath="D:\\AllData\\";
	private string AllRunDataFullPath="";
	private string CurFileDate="";
	private int Screw1TryTime=0;
	private int Screw2TryTime=0;
	private int Screw3TryTime=0;
	private int Screw4TryTime=0;
	private bool _ngFlag = false;
	private string ErrID="na";
	#endregion
	
	#region LM
	private LineConfig lineConfig=null;
	
	private string DSN = "na";
	private string TrayID = "na";
	private string FactoryID = "na";
	private string ProjectName = "na";
	private string BuildConfig = "na";
	private string OperatorName = "na";
	private string StartTime = "na";
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
					
					if(((IEleDI)_eleDiUpExitReq).Read()) //if next station exit request true, turn on output request
					{
					   	((IEleDO)_eleDoUpExitReq).Write(true);
				 	}
					else
					{
						((IEleDO)_eleDoUpExitReq).Write(false); // else do not request to outflow
					}
					
					/*if (((IEleDI)_eleBtnLock).Read()) //if Btn_lock, which is the key
					{
					    if(((IEleDI)_eleDiUpExitReq).Read()) //if next station exit request true, turn on output request
						{
					    	((IEleDO)_eleDoUpExitReq).Write(true);
				 		}
						else
						{
							((IEleDO)_eleDoUpExitReq).Write(false); // else do not request to outflow
						}
					}*/	
				}
				else
				{
					//if (((IEleDI)_eleBtnLock).Read()) // else if just btnlock, exit sensor not reading. Note: if we set key to lock, get an error message
					//{
						//if(((((IEleDI)_eleDiUplineEntry).Read()||(((IEleDI)_eleDiUpLineCheck).Read()&&!((IEleDI)_eleDiUplineWork).Read()))&&firstStart)) // this if does nothing
						//{
							//SetStep(222,"AxisAbove run");
						//} else
						if(((IEleDI)_eleDiUplineWork).Read()&&firstStart) // if work position sensor and this the first cycle
						{
							SetTaskError(true,AlarmLv.Error,01,"Work Place is not empty,please move it!",task);
							Thread.Sleep(1000);
							MessageBox.Show("Work Place is not empty,please move it!");
							goto RunStep1;
						}
						else
						{
							((IEleDO)_eleDoUpEntryReq).Write(true); //no tray, request inflow from previous station
						}					
					//}
					SqlCheckErr=false;
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
				
				if (!((IEleDI)_eleBtnLock).Read() && (((IEleDI)_eleDiUplineEntry).Read() || ((IEleDI)_eleDiUpEntryReq).Read()))		
					//Mode of 'UnLock', and receive the signal of 'UpLineEntry' //added condition that unlock mode can continue if inflow request is active
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
					SqlCheckErr=true;
					DataInfoManager.Instance.DataInfoSetVal("TaskCancel", true);
					SetStep(5, "UpConvey will move...");
				}	
			}
			#endregion		
		RunStep202: //if unlock, Start "SubTask) //gets station ready in abscence of the network
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
				((IEleAxisLine)_eleAxisAbove).MC_MoveJog(true, UpSpeed1); //bring in new tray
				((IEleCyldLine)_eleCylBlock).CyldStretch(); //move out the blocking cylinder
				SetStep(222,"Waiting for the signal of '_eleDiUplineEntry'...");
			}
			#endregion	
		RunStep222: //AxisAbove run, lock mode and inflow request high, also from step 202, with belt moving
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
					_trayInTime=DateTime.Now; //record time
					((IEleDO)_eleDoUpEntryReq).Write(false); //set inflow request inactive
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
					((IEleAxisLine)_eleAxisAbove).MC_MoveJog(true,UpSpeed1); //move
				}
				
				if (((IEleDI)_eleDiUpLineCheck).Read()) //if tray reaches the middle optical sensor, slow down the speed
				{
					var val = DataParamManager.Instance.GetDataParamByName("UpSpeed2");
					int UpSpeed2 = (val != null ? (int)val.DataVal : 50);
					val = DataParamManager.Instance.GetDataParamByName("UpDelay");
					int UpDelay = (val != null ? (int)val.DataVal : 500);
					
					Thread.Sleep(UpDelay);
					((IEleAxisLine)_eleAxisAbove).MC_ChangeVel(UpSpeed2); //move slower
					
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
				
				if (((IEleDI)_eleDiUpLineCheck).Read() && ((IEleDI)_eleDiUplineWork).Read()) //if work position sensors are high (check is sensor at middle of machine)
				{
					/*if (((IEleDI)_eleBtnLock).Read()) //if lock mode
					{
						((IEleDO)_eleDoUpEntryReq).Write(false); //turn off upline inflow request
					}*/
					
					((IEleDO)_eleDoUpEntryReq).Write(false); //turn off upline inflow request, tray is in work position
					
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
					/*if (((IEleDI)_eleBtnLock).Read()) 
					{
						((IEleDO)_eleDoUpEntryReq).Write(false); //if locked, turn off entry request again (repeat of step 4)
					}*/
					((IEleDO)_eleDoUpEntryReq).Write(false); //turn off entry request again (repeat of step 4)
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
				
				if (!ClearMode)  //what??? guess this station is never in clearmode
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
		RunStep405:  //Sql check does nothing in unlock mode except set tray ready
			#region
			if (CheckAutoRun() && (CurStep == 405))
			{	
				#region
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
				#endregion
		   		else
		   		{
		   			DataInfoManager.Instance.DataInfoSetVal("TrayReady", true); //send tray ready bool
		   		}
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
				//wait here
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
				//wait here for next station to be ready
				#region
				if (((IEleDI)_eleBtnLock).Read() && ((IEleDI)_eleDiUpExitReq).Read() )		//Mode of 'Lock', and receive the signal of 'UpExitReq'
				{
					if(TrayManager.Instance.IsTargetStationFree(CurRep,DevRecipeManager.Instance.StationName.ToString()))
					{
						if(((IEleDI)_eleBtnLock).Read()&&!SqlCheckErr)
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
						}
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
				#endregion				
				if (!((IEleDI)_eleBtnLock).Read() && ((IEleDI)_eleDiUpExitReq).Read())		//Mode of 'UnLock', and check downline request for new tray
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
					
					((IEleDO)_eleDoUpExitReq).Write(true); //turn on outflow request
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
						/*if (((IEleDI)_eleBtnLock).Read()) //if lock
						{
							((IEleDO)_eleDoUpEntryReq).Write(false); //turn off entry requirement
						}*/
						((IEleDO)_eleDoUpEntryReq).Write(false); //turn off inflow request, this shouldn't have been on anyway
						
						SetStep(100, "UpConvey will be stop, then current task will be stop.");
					}	
				}
			}
			#endregion			
		RunStep100:
			#region
			if (CheckAutoRun() && (CurStep == 100))
			{
				((IEleAxisLine)_eleAxisAbove).MC_Stop(); //stop conveyor
				AutoRunning=false;	
				_trayOutTime=DateTime.Now; //record time
				
				double _blockTime1 = (_trayDockTime - _trayDockStart).TotalSeconds;
				double _blockTime2 = ( _trayReleaseTime - _autoEndTime).TotalSeconds;
				double _transferTime1 = (_trayDockStart - _trayInTime).TotalSeconds;
				double _transferTime2 = ( _trayOutTime - _trayStartAgainTime ).TotalSeconds;
				string _ct = (_autoEndTime-_trayDockTime).TotalSeconds.ToString("0.000");
				string _carrierBlockTime = (_blockTime1 + _blockTime2).ToString("0.000");
				string _transferTime = (_transferTime1 + _transferTime2).ToString("0.000");
				string _strDownTime = _downTime.ToString("0.000");
				
				string capResultX1 = "";
	 			string capResultY1 = "";
	 			string capResultX2 = "";
	 			string capResultY2 = "";
	 			string capResultX3 = "";
	 			string capResultY3 = "";
	 			string capResultX4 = "";
	 			string capResultY4 = "";
	 			string screw1TryTime = "";
		 		string screw2TryTime = "";
		 		string screw3TryTime = "";
		 		string screw4TryTime = "";
	 			
				/*if (((IEleDI)_eleBtnLock).Read()) 
				{
					((IEleDO)_eleDoUpExitReq).Write(false); //turn off exit request
				}*/
				
				((IEleDO)_eleDoUpExitReq).Write(false); //turn off exit request
				//record data
				#region
				
				string taskRet = "";
				if (TaskResult == 0) 
				{
					taskRet = "OK";
				}
				else
				{
					taskRet = "NG";
				}			
				string strCt = string.Format("Last work CT: {0}s", _ct);
				LogAdd(strCt, AlarmLv.Info, 999);
				
		 		WriteCTLog((_trayOutTime-_trayInTime).TotalSeconds,(_autoEndTime-_trayDockTime).TotalSeconds);
		 		WriteCTTime(_trayInTime,_trayOutTime,_trayDockTime,_trayReleaseTime);
		 		
		 		var val = DataInfoManager.Instance.DataInfoGetVal("Capture1");
		 		Capture1 = (val != null)?Convert.ToBoolean(val):false;
		 		val = DataInfoManager.Instance.DataInfoGetVal("Capture2");
		 		Capture2 = (val != null)?Convert.ToBoolean(val):false;
		 		val = DataInfoManager.Instance.DataInfoGetVal("Dector");
		 		Dector = (val != null)?Convert.ToBoolean(val):false;
		 		val = DataInfoManager.Instance.DataInfoGetVal("CapResultX1");
		 		CapResultX1 = (val != null)?Convert.ToDouble(val):0;
		 		val = DataInfoManager.Instance.DataInfoGetVal("CapResultY1");
		 		CapResultY1 = (val != null)?Convert.ToDouble(val):0;
		 		val = DataInfoManager.Instance.DataInfoGetVal("CapResultX2");
		 		CapResultX2 = (val != null)?Convert.ToDouble(val):0;
		 		val = DataInfoManager.Instance.DataInfoGetVal("CapResultY2");
		 		CapResultY2 = (val != null)?Convert.ToDouble(val):0;
		 		val = DataInfoManager.Instance.DataInfoGetVal("CapResultX3");
		 		CapResultX3 = (val != null)?Convert.ToDouble(val):0;
		 		val = DataInfoManager.Instance.DataInfoGetVal("CapResultY3");
		 		CapResultY3 = (val != null)?Convert.ToDouble(val):0;
		 		val = DataInfoManager.Instance.DataInfoGetVal("CapResultX4");
		 		CapResultX4 = (val != null)?Convert.ToDouble(val):0;
		 		val = DataInfoManager.Instance.DataInfoGetVal("CapResultY4");
		 		CapResultY4 = (val != null)?Convert.ToDouble(val):0;		 		
				val = DataInfoManager.Instance.DataInfoGetVal("QyPartNo");
		 		int _qy_PartNO = (val != null)?Convert.ToInt32(val):0;		 		
				val = DataInfoManager.Instance.DataInfoGetVal("QyPartName");
		 		string _qy_PartName = (val != null)?Convert.ToString(val):"";
		 		val =DataInfoManager.Instance.DataInfoGetVal("Screw1TryTime");
				Screw1TryTime=(val!=null)?Convert.ToInt32(val):0;
				val =DataInfoManager.Instance.DataInfoGetVal("Screw2TryTime");
				Screw2TryTime=(val!=null)?Convert.ToInt32(val):0;
				val =DataInfoManager.Instance.DataInfoGetVal("Screw3TryTime");
				Screw3TryTime=(val!=null)?Convert.ToInt32(val):0;
				val =DataInfoManager.Instance.DataInfoGetVal("Screw4TryTime");
				Screw4TryTime=(val!=null)?Convert.ToInt32(val):0;
				val= DataInfoManager.Instance.DataInfoGetVal("ErrID");
				ErrID=(val!=null)?(string)val:"na";
				string CurStationName=DevRecipeManager.Instance.StationName;
			
		 		val = DataInfoManager.Instance.DataInfoGetVal("CurTrayID");
		 		TrayID = (val != null)?(string)val:"NULL";
		 		
		 		if(CheckDryRun())
		 		{
		 			ModeString = "DryRun";
		 		}
		 		else
		 		{
		 			ModeString = "Auto";		 		
		 		}		 		
		 		
		 		capResultX1 = CapResultX1.ToString("#.000");
		 		capResultX2 = CapResultX2.ToString("#.000");
		 		capResultX3 = CapResultX3.ToString("#.000");
		 		capResultX4 = CapResultX4.ToString("#.000");
		 		capResultY1 = CapResultY1.ToString("#.000");
		 		capResultY2 = CapResultY2.ToString("#.000");
		 		capResultY3 = CapResultY3.ToString("#.000");
		 		capResultY4 = CapResultY4.ToString("#.000");
		 		
		 		if (Screw1TryTime == 0) 
		 		{
		 			screw1TryTime = "na";
		 			screw2TryTime = "na";
					screw3TryTime = "na";
					screw4TryTime = "na";
		 			capResultX1 = "na";
					capResultY1 = "na";
					capResultX2 = "na";
					capResultY2 = "na";
		 			capResultX3 = "na";
					capResultY3 = "na";
					capResultX4 = "na";
					capResultY4 = "na";			
		 		}
		 		
		 		if (Screw2TryTime == 0) 
		 		{
		 			screw2TryTime = "na";
					screw3TryTime = "na";
					screw4TryTime = "na";
					capResultX2 = "na";
					capResultY2 = "na";
		 			capResultX3 = "na";
					capResultY3 = "na";
					capResultX4 = "na";
					capResultY4 = "na";	 			
		 		}
		 		
		 		if (Screw3TryTime == 0) 
		 		{
					screw3TryTime = "na";
					screw4TryTime = "na";
		 			capResultX3 = "na";
					capResultY3 = "na";
					capResultX4 = "na";
					capResultY4 = "na"; 			
		 		}
		 		
		 		if (Screw4TryTime == 0) 
		 		{
					screw4TryTime = "na";
					capResultX4 = "na";
					capResultY4 = "na"; 			 			
		 		}
		 		
		 		screw1TryTime = Screw1TryTime.ToString();
		 		screw2TryTime = Screw2TryTime.ToString();
		 		screw3TryTime = Screw3TryTime.ToString();
		 		screw4TryTime = Screw4TryTime.ToString();
		 		
		 		if (_ngFlag)
		 		{
		 			_ngFlag = false;
		 			capResultX1 = "na";
					capResultY1 = "na";
					capResultX2 = "na";
					capResultY2 = "na";
		 			capResultX3 = "na";
					capResultY3 = "na";
					capResultX4 = "na";
					capResultY4 = "na";
					screw1TryTime = "na";
					screw2TryTime = "na";
					screw3TryTime = "na";
					screw4TryTime = "na";
		 		}
		 		string ctStr = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8}",TrayID, _ct, System.DateTime.Now.ToString("HH:mm:ss"),taskRet,_carrierBlockTime,_transferTime,_strDownTime,
		 		                             capResultX1+","+capResultX2+","+capResultX3+","+capResultX4,
		 		                             capResultY1+","+capResultY2+","+capResultY3+","+capResultY4);
		 		DataOutputManager.Instance.ClientDataOutputManagerUpdateState("CT", _ct.ToString());
				DataOutputManager.Instance.ClientAddPartInfo("1", ctStr);
					 		
		 		try
		 		{
		 			string AutoTaskString = capResultX1 + "," + capResultY1 + "," + capResultX2 + "," + capResultY2 + "," + capResultX3 + "," + capResultY3 + "," + capResultX4 + "," + capResultY4 + ","
		 				+ screw1TryTime + "," + screw2TryTime + "," + screw3TryTime + "," + screw4TryTime;
		 			string AllDataString = "";
		 			if(((IEleDI)_eleBtnLock).Read())
		 			{
		 				 AllDataString = DSN+","+TrayID+","+lineConfig.FactoryID+","+lineConfig.ProjectName+","+lineConfig.BuildConfiguration+","
		 				 			   + lineConfig.OperatorName+","+_curUnit.Name+","+lineConfig.StartTime+","+CurStationName+",na,"
		 				 			   +"na," + AutoTaskName +","+(_autoEndTime-_trayDockTime).TotalSeconds.ToString("#.000")+","+(_trayReleaseTime - _autoEndTime).TotalSeconds.ToString("#.000")+",na,"
		 				 			   +_trayInTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + _trayDockTime.ToString("yyyy-MM-dd HH:mm:ss")+","+_trayReleaseTime.ToString("yyyy-MM-dd HH:mm:ss")+","+_trayOutTime.ToString("yyyy-MM-dd HH:mm:ss")+","+ModeString+","
		 				 			   +above_speed+","+below_speed+","+((IEleAxisLine)_eleAxisX).DefaultParams.Velocity.ToString()+","+((IEleAxisLine)_eleAxisY).DefaultParams.Velocity.ToString()+","+((IEleAxisLine)_eleAxisZ).DefaultParams.Velocity.ToString()+"," 
		 				 			   +"na,na,na,na,na,"
		 				 			   +"na,na,na,na,na,"
		 				 			   +"na,na,na,na,na,"
		 				 			   +"na,"+ErrID.ToString()+",na,na,na,"
		 				 			   +AutoTaskString+","+taskRet+",na,na";
		 			}
		 			else
		 			{
		 				 AllDataString = DSN+","+TrayID+",na,na,na,"
		 				 			   + "na,na,na,"+CurStationName+",na,"
		 				 			   + "na," + AutoTaskName + ","+(_autoEndTime-_trayDockTime).TotalSeconds.ToString("#.000")+","+(_trayReleaseTime - _autoEndTime).TotalSeconds.ToString("#.000")+",na,"
		 				 			   + _trayInTime.ToString("yyyy-MM-dd HH:mm:ss") + ","+ _trayDockTime.ToString("yyyy-MM-dd HH:mm:ss")+","+_trayReleaseTime.ToString("yyyy-MM-dd HH:mm:ss")+","+  _trayOutTime.ToString("yyyy-MM-dd HH:mm:ss")+","+ModeString+","
		 				 			   + above_speed+","+below_speed+","+((IEleAxisLine)_eleAxisX).DefaultParams.Velocity.ToString()+","+((IEleAxisLine)_eleAxisY).DefaultParams.Velocity.ToString()+","+((IEleAxisLine)_eleAxisZ).DefaultParams.Velocity.ToString()+","
		 				 			   +"na,na,na,na,na,"
		 				 			   +"na,na,na,na,na,"
		 				 			   +"na,na,na,na,na,"
		 				 	           +"na,"+ErrID.ToString()+",na,na,na,"
		 				 		       +AutoTaskString+","+taskRet+",na,na";
		 			}
		 			WriteAllData(AllDataString);
		 			
		 		
	 				var pic=DataInfoManager.Instance.DataInfoGetVal("Pic0");
	 				bool flag=(pic!=null?(bool)pic:false);
	 				Pic1=(flag!=false?"OK":"NG");
	 				
	 			    pic=DataInfoManager.Instance.DataInfoGetVal("Pic1");
	 				flag=pic!=null?(bool)pic:false;
	 				Pic2=flag!=false?"OK":"NG";
	 				
	 				pic=DataInfoManager.Instance.DataInfoGetVal("Pic2");
	 				flag=pic!=null?(bool)pic:false;
	 				Pic3=flag!=false?"OK":"NG";
	 				
	 				pic=DataInfoManager.Instance.DataInfoGetVal("Pic3");
	 				flag=pic!=null?(bool)pic:false;
	 				Pic4=flag!=false?"OK":"NG";
	 				
	 				pic=DataInfoManager.Instance.DataInfoGetVal("Pic5");
	 				flag=pic!=null?(bool)pic:false;
	 				Pic6=flag!=false?"OK":"NG";
	 				
	 				string PassOrFail="";
	 				if(Pic6=="OK")
	 				{
	 					PassOrFail="Pass";
	 				}
	 				else
	 				{
	 					PassOrFail="Fail";
	 				}
		 			int index=0;
					index++;		 			
		 			string VppDataString=index+","+TrayID+","+System.DateTime.Now+","+ModeString+","+CurStationName+","+Pic1+","+Pic2+","+Pic3+","+Pic4+","
		 				+CapResultX1+","+CapResultY1+","+CapResultX2+","+CapResultY2+","+CapResultX3+","+CapResultY3+","+CapResultX4+","+CapResultY4+","
		 				+Pic6+","+PassOrFail;
		 			WriteAllVisionData(VppDataString);
		 		}
		 		catch(Exception ex)
		 		{}
		 		
				string strAll = "";
				
				Dictionary<string,string> resultItems = new Dictionary<string, string>();
				//resultItems.Add("DSN",DSN);
				resultItems.Add("station_id",CurStationName);
				resultItems.Add("no_of_parts","na");
				resultItems.Add("part_name","na");
				resultItems.Add("task_id",AutoTaskName);
				resultItems.Add("cycle_time",(_autoEndTime-_trayDockTime).TotalSeconds.ToString("#.000"));
				resultItems.Add("delay_time",(_trayStartAgainTime - _trayReleaseTime).TotalSeconds.ToString("#.000"));
				resultItems.Add("part_barcode","na");
				resultItems.Add("tray_enterance_time",_trayInTime.ToString("yyyy-MM-dd HH:mm:ss"));
				resultItems.Add("tray_dock_time",_trayDockTime.ToString("yyyy-MM-dd HH:mm:ss"));
				resultItems.Add("tray_release_time",_trayReleaseTime.ToString("yyyy-MM-dd HH:mm:ss"));
				resultItems.Add("tray_exit_time", _trayOutTime.ToString("yyyy-MM-dd HH:mm:ss"));
				resultItems.Add("run_mode", ModeString);
				resultItems.Add("top_conveyer_speed",((IEleAxisLine)_eleAxisAbove).JogVelocity.ToString());
				resultItems.Add("bottom_conveyer_speed",((IEleAxisLine)_eleAxisBelow).JogVelocity.ToString());
				resultItems.Add("x_axis_speed",	((IEleAxisLine)_eleAxisX).DefaultParams.Velocity.ToString());
				resultItems.Add("y_axis_speed",((IEleAxisLine)_eleAxisY).DefaultParams.Velocity.ToString());
				resultItems.Add("z_axis_speed",((IEleAxisLine)_eleAxisZ).DefaultParams.Velocity.ToString());
				resultItems.Add("r_axis_speed","na");
				resultItems.Add("capture_the_part","na");
				resultItems.Add("capture_the_host","na");
				resultItems.Add("capture_finally","na");
				resultItems.Add("part_x_offset","na");
				resultItems.Add("part_y_offset","na");
				resultItems.Add("part_r_offset","na");
				resultItems.Add("host_x_offset","na");
				resultItems.Add("host_y_offset","na");
				resultItems.Add("host_r_offset","na");
				resultItems.Add("x_offset_to_robot","na");
				resultItems.Add("y_offset_to_robot","na");
				resultItems.Add("r_offset_to_robot","na");
				resultItems.Add("x_offset_result","na");
				resultItems.Add("y_offset_result","na");
				resultItems.Add("r_offset_result","na");
				resultItems.Add("critical_errors_id",ErrID.ToString());
				resultItems.Add("screw_torque","na");
				resultItems.Add("pressure_value","na");
				resultItems.Add("press_time","na");
				resultItems.Add("1_delta_x_for_screw_station",capResultX1);
				resultItems.Add("1_delta_y_for_screw_station",capResultY1);
				resultItems.Add("2_delta_x_for_screw_station",capResultX2);
				resultItems.Add("2_delta_y_for_screw_station",capResultY2);
				resultItems.Add("3_delta_x_for_screw_station",capResultX3);
				resultItems.Add("3_delta_y_for_screw_station",capResultY3);
				resultItems.Add("4_delta_x_for_screw_station",capResultX4);
				resultItems.Add("4_delta_y_for_screw_station",capResultY4);
				resultItems.Add("1_screw_try_time",screw1TryTime);
				resultItems.Add("2_screw_try_time",screw2TryTime);
				resultItems.Add("3_screw_try_time",screw3TryTime);
				resultItems.Add("4_screw_try_time",screw4TryTime);
				resultItems.Add("ok_ng_name",taskRet);
				resultItems.Add("arbitrary_variable_1","na");
				resultItems.Add("arbitrary_variable_2","na");
				StationManager.Instance.WriteResultItem(CurStationName,resultItems,_trayInTime.ToString("yyyy-MM-dd HH:mm:ss"),_trayOutTime.ToString("yyyy-MM-dd HH:mm:ss"),TrayID);
					
		 		if(((IEleDI)_eleBtnLock).Read())
				{
						Thread.Sleep(100);
						//write CompleteTrayStep
						
					strAll = DictionaryToString(resultItems);
						//if(strAll != null)
						//	strAll = strLine + strAll;
					DataOutputManager.Instance.ClientDataOutputManagerUpdateState("DelayTime", _delayTime.ToString());
					DataOutputManager.Instance.ClientDataOutputManagerUpdateState("Result", TaskResult == 0 ? "OK" : "NG");
					if(!SqlCheckErr)
					{
						_curUnit.WriteResultItem(CurStationName,resultItems,"result",CurRep);
						//((ISuperClient)_superClient).SendResultInfo(strAll);
						SqlCheckErr=false;
					}
				}
				int _curOK = -1;
		 		int _curNG = -1;
		 		string _curUph = "";
		 		StationManager.Instance.GetOKAndNG(out _curOK,out _curNG,out _curUph,"","");
		 		
		 		DataOutputManager.Instance.ClientDataOutputManagerUpdateVar("OK",_curOK);
		 		DataOutputManager.Instance.ClientDataOutputManagerUpdateVar("NG",_curNG);
		 		DataOutputManager.Instance.ClientDataOutputManagerUpdateVar("UPH",_curUph);
				if (CheckConnectState()) 
				{
					Dictionary<string,string> resultItems1 = new Dictionary<string, string>();
					resultItems1.Add("StRet",taskRet);
					resultItems1.Add("StPartId","na");
					resultItems1.Add("StTrayId",TrayID);
					resultItems1.Add("StCt",(_autoEndTime-_trayDockTime).TotalSeconds.ToString("#.000"));
					resultItems1.Add("StationOK",_curOK.ToString());
					resultItems1.Add("StationNG",_curNG.ToString());
					resultItems1.Add("StationTotal",(_curOK + _curNG).ToString());
					resultItems1.Add("UPH",_curUph);
					resultItems1.Add("DownTime",_downTime.ToString("#.000"));

					strAll=DictToString_Socket(resultItems1);
					((ISuperClient)_superClient).SendStationState("station:"+ DevRecipeManager.Instance.StationName + ";"+strAll);
					
					resultItems1.Clear();
					resultItems1.Add("CT",_ct);
					resultItems1.Add("CarrierBlockTime",_carrierBlockTime);
					resultItems1.Add("TransferTime",_transferTime);
					resultItems1.Add("DelayTime",_delayTime.ToString("#.000"));
					resultItems1.Add("IdleTime",_strDownTime);
					strAll=DictToString_Socket(resultItems1);
					((ISuperClient)_superClient).SendStationTime("station:"+ DevRecipeManager.Instance.StationName + ";"+strAll);
				}
				
				DataInfoManager.Instance.DataInfoSetVal("ErrID","na");
				/*
				DataInfoManager.Instance.DataInfoSetVal("CapResultX1",0);
				DataInfoManager.Instance.DataInfoSetVal("CapResultY1",0);
				DataInfoManager.Instance.DataInfoSetVal("CapResultX2",0);
				DataInfoManager.Instance.DataInfoSetVal("CapResultY2",0);
				DataInfoManager.Instance.DataInfoSetVal("CapResultX3",0);
				DataInfoManager.Instance.DataInfoSetVal("CapResultY3",0);
				DataInfoManager.Instance.DataInfoSetVal("CapResultX4",0);
				DataInfoManager.Instance.DataInfoSetVal("CapResultY4",0);
				DataInfoManager.Instance.DataInfoSetVal("Screw1TryTime",0);
				DataInfoManager.Instance.DataInfoSetVal("Screw2TryTime",0);
				DataInfoManager.Instance.DataInfoSetVal("Screw3TryTime",0);
				DataInfoManager.Instance.DataInfoSetVal("Screw4TryTime",0);
				*/
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
		if (!GetPrim("MySql0", ref _mySql, 5)) goto TaskInitErr;
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
		CurFileDate="Screw1-"+GetDayString(DateTime.Now);
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
		if(!System.IO.Directory.Exists(AllRunDataPath))
			System.IO.Directory.CreateDirectory(AllRunDataPath);
		AllDatafs = new FileStream(filePath, System.IO.FileMode.Append, System.IO.FileAccess.Write);
		AllDatasw = new StreamWriter(AllDatafs, System.Text.Encoding.Default);
        AllDatasw.AutoFlush = true;
        
        string strLine = " , ";
        AllDatasw.WriteLine(strLine);
        AllDatasw.WriteLine(strLine);
        string limitMax="";
        string limitMin="";
        string limitUnit="";
        limitMax="na,na,na,na,na,"
        		+"na,na,na,na,na,"
        		+"na,na,na,na,na," 
        		+"na,na,na,na,na,"
        		+top_conveyer_speed_max+","+bottom_conveyer_speed_max+","+x_speed_max+","+y_speed_max+","+z_speed_max+","
        		+r_speed_max+","+"na,na,na,na,"
        		+"na,na,na,na,na,"
        		+x_offset_robot_max+","+y_offset_robot_max+","+r_offset_robot_max+","+"na,na,"
        		+"na,na,"+screw_torque_max+","+press_value_max+","+press_time_max+","
        		+delta_1_x_for_screw_station_max+","+delta_1_y_for_screw_station_max+","+delta_2_x_for_screw_station_max+","+delta_2_y_for_screw_station_max+","+delta_3_x_for_screw_station_max+","
        		+delta_3_y_for_screw_station_max+","+delta_4_x_for_screw_station_max+","+delta_4_y_for_screw_station_max+","+screw_1_try_time_max+","+screw_2_try_time_max+","
        		+screw_3_try_time_max+","+screw_4_try_time_max+","+"na,na,na";
        
        AllDatasw.WriteLine(limitMax);
        
        limitMin="na,na,na,na,na,"
        		+"na,na,na,na,na,"
        		+"na,na,na,na,na,"
        		+"na,na,na,na,na,"
        		+top_conveyer_speed_min+","+bottom_conveyer_speed_min+","+x_speed_min+","+y_speed_min+","+z_speed_min+","
        		+r_speed_min+","+"na,na,na,na,"
        		+"na,na,na,na,na,"
        		+x_offset_robot_min+","+y_offset_robot_min+","+r_offset_robot_min+","+"na,na,"
        		+"na,na,"+screw_torque_min+","+press_value_min+","+press_time_min+","
        		+delta_1_x_for_screw_station_min+","+delta_1_y_for_screw_station_min+","+delta_2_x_for_screw_station_min+","+delta_2_y_for_screw_station_min+","+delta_3_x_for_screw_station_min+","
        		+delta_3_y_for_screw_station_min+","+delta_4_x_for_screw_station_min+","+delta_4_y_for_screw_station_min+","+screw_1_try_time_min+","+screw_2_try_time_min+","
        		+screw_3_try_time_min+","+screw_4_try_time_min+","+"na,na,na";
        
        AllDatasw.WriteLine(limitMin);  
        
        limitUnit="na,na,na,na,na,"
        		 +"na,na,Date_time,na,na,"
        	     +"na,na,s,s,na,"
        		 +"Date_time,Date_time,Date_time,Date_time,na,"
        	     +"mm/s,mm/s,mm/s,mm/s,mm/s,"
        		 +"mm/s,na,na,na,mm,"
        	     +"mm,mm,mm,mm,mm,"
        		 +"mm,mm,mm,mm,mm,"
        		 +"mm,na,N/m,N,s,"
        		 +"mm,mm,mm,mm,mm,"
        		 +"mm,mm,mm,na,na,"
        		 +"na,na,na,na,na";
        
        AllDatasw.WriteLine(limitUnit);
        
        string tempstr="dsn,tray_id,factory_id,project_name,build_configuration," 
        	+"operator_name,unit_id,start_time,station_id,no_of_parts,"
        	+"part_name,task_id,cycle_time,delay_time,part_barcode," 
        	+"tray_enterance_time,tray_dock_time,tray_release_time,tray_exit_time,run_mode,"
        	+"top_conveyer_speed,bottom_cnoveyer_speed,x_axis_speed,y_axis_speed,z_axis_speed,"
        	+"r_axis_speed,capture_the_part,capture_the_host,capture_finally,part_x_offset,"
        	+"part_y_offset,part_r_offset,host_x_offset,host_y_offset,host_r_offset,"
        	+"x_offset_to_robot,y_offset_to_robot,r_offset_to_robot,x_offset_result,y_offset_result,"
        	+"r_offset_result,critical_errors_id,screw_torque,pressure_value,press_time,"
        	+"1_delta_x_for_screw_station,1_delta_y_for_screw_station,2_delta_x_for_screw_station,2_delta_y_for_screw_station,3_delta_x_for_screw_station,"
        	+"3_delta_y_for_screw_station,4_delta_x_for_screw_station,4_delta_y_for_screw_station,1_screw_try_time,2_screw_try_time,"
        	+"3_screw_try_time,4_screw_try_time,ok_ng_name,arbitrary_variable_1,arbitrary_variable_2";

        AllDatasw.WriteLine(tempstr);
    	
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
