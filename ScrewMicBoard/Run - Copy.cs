using System;
using System.Windows.Forms;
using System.Threading;
using Lead.Detect.Global;
using Lead.Detect.Interfaces;
using Lead.Detect.LogHelper;
using Lead.Detect.Interfaces.Communicate;
using Lead.Detect.BussinessModel;
using Lead.Detect.DataOutputCommon;
using System.Linq;
using System.IO;

public class RunState
{
	#region define IPrim
	private IPrim _primMotionCard = null;
	private IPrim _vpAssemble = null;
	private IPrim _vpDetector =null;
	private IPrim _superClient = null;
	#endregion
	#region define IEle
	private IEle _eleMotion =null;
	private IEle _eleAxisAbove=null;
	private IEle _eleAxisBelow=null;
	private IEle _eleAxisX=null;
	private IEle _eleAxisY=null;
	private IEle _eleAxisZ=null;
	
	private IEle _eleBtnStart = null;
	private IEle _eleBtnStop = null;
	private IEle _eleBtnReset = null;
	private IEle _eleBtnEMG1 = null;
	private IEle _eleBtnEMG2 = null;
	private IEle _eleBtnLock = null;
	private IEle _eleDidoor1 = null;
	private IEle _eleDidoor2 = null;
	private IEle _eleDiUplineWork = null;
	private IEle _eleDiUpLineCheck = null;
	private IEle _eleDiSuckerVac = null;
	private IEle _eleDiScrewWorking = null;
	private IEle _eleDiScrewDone = null;
	private IEle _eleDiAirPressure = null;	
	private IEle _eleDiScrewReady=null;
	
	private IEle _eleDoFLYellow = null;
	private IEle _eleDoFLGreen = null;
	private IEle _eleDoFLRed = null;
	private IEle _eleDoFLBuzzer = null;	
	private IEle _eleDoBtnStart = null;
	private IEle _eleDoBtnStop = null;
	private IEle _eleDoBtnReset = null;
	private IEle _eleDoLight = null;		
	
	private IEle _eleDoVaccum = null;
	//private IEle _eleDoBlow = null;
	private IEle _eleDoTrigCa1 = null;	
	private IEle _eleDoTrigCa2 = null;		
	private IEle _eleDoTrigLight1 = null;
	private IEle _eleDoTrigLight2 = null;
	private IEle _eleDoTrigScrewWork = null;	
	
	private IEle _eleCylScrewDown =null;
	private IEle _eleCylPush = null;
	#endregion
		
	private int ScrewNum = 4;
	
	private int PictureNum=1;
	
	private bool ResetDone=false;
	private bool SystemPause=false;
	private bool Running=false;
	private bool Stoping=false;
	private bool Pausing=false;
	private bool Alarming=false;
	private bool WaitReset=false;
	private bool WaitContinue=false;
	private bool AutoRunning=false;
	private bool Resetting=false;
	
	private bool btnStart=false;
	private bool btnPause=false;
	private bool btnReset=false;
	private bool btnEMG=false;
	private bool btnLock=false;
	
	private bool TrayReady = false;
	private bool DryRunMode = false;
	private bool StartFlag=false;
	private bool ResetFlag=false;
	
	private bool isScrewPicTaken = false;
	
	private IEleAxisLine[] AxisGroup;
	private IEleAxisLine[] AxisGroup1;
	private double[] HomeVelGroup;
	private double[] MoveVelGroup;
	private double[] HomeVelGroup1;
	private double[] MoveVelGroup1;

	private double[] WaitPos;
	private double[] PickScrew;
	private double[] ScrewCheck;
	private double[] ScrewPicPos;
	private double[] ScrewPos1;
	private double[] ScrewPos2;
	private double[] ScrewPos3;
	private double[] ScrewPos4;
	private double[] ScrewPos;
	private double[] ActualScrewPos1;
	private double[] ActualScrewPos2;
	private double[] ActualScrewPos3;
	private double[] ActualScrewPos4;
	private double[] ScrewThrowPos;
	
	private bool Detect = false;//视觉是否正常执行Detect
	private bool Capture = false;//视觉是否正常执行Capture

	private bool DetectCalcFinish = false;//Detect计算是否完成
	private bool CapCalcFinish = false;//Capture计算是否完成

	private bool DetectResult = false;//Detect的计算结果，表示是否有抓取Screw
	private double CapResultX1 = 0;//Capture的四个点位坐标值
	private double CapResultY1 = 0;
	private double CapResultX2 = 0;
	private double CapResultY2 = 0;
	private double CapResultX3 = 0;
	private double CapResultY3 = 0;
	private double CapResultX4 = 0;
	private double CapResultY4 = 0;
	private string ErrID = "na";
	private bool isWorkErr = false;
	private DateTime upPicTakeTime = DateTime.Now;
	
	private int count=0;
	private int ResetStep=0;
	private int CurStep=0;
	private int DryRunStep=0;
	
	private int UpCamErrCount=0;
	private int UpCamErrCountMax=3;
	private int DnCamErrCount=0;
	private int DnCamErrCountMax=3;
	private int UpCamEndErrCount=0;
	private int UpCamEndErrCountMax=3;
	
	private int Screw1TryTime=0;
	private int Screw2TryTime=0;
	private int Screw3TryTime=0;
	private int Screw4TryTime=0;
	private int ScrewDownTryTime=0;
	private double ZAxisSafeHeight=0;
	
	//private int CurStep=-1;
	private int LastStep=-1;
	private bool AutoRun=false;
	private bool TaskError=false;
	private bool firstflag=true;
	private string TaskName = "";
	public bool SocketEnable = false;
	public bool ConnState = false;
	
	#region CT relate
	DateTime[] ScrewStartTime=new DateTime[4];
	DateTime[] ScrewEndTime=new DateTime[4];
	double totalMoveTime=0;
	double lasttotalMoveTime=0;
	private FileStream CTLogfs;
	private StreamWriter CTLogsw;
	private string CTLogPath="D:\\ScrewCTFile.csv";
	private int UplineStep=0;
	#endregion
	private QualifyDataOutPutControl _qy = null;
	
	public int Exec(ITask task)
	{
		count=0;
		ResetStep=0;
		CurStep=0;
		ScrewDownTryTime=0;
		Screw1TryTime=0;
		Screw2TryTime=0;
		Screw3TryTime=0;
		Screw4TryTime=0;
		
		if(File.Exists(CTLogPath)==false)
		{
			CreateNewCSV(CTLogPath);
		}
		
		try
		{
			while(task.TaskRunStat!=TaskRunState.Stop)
			{
				Thread.Sleep(1);
				if(firstflag)
				{
					firstflag=false;
					
					if(CheckConnectState())
					{
						((ISuperClient)_superClient).SendStationState("station:"+ DevRecipeManager.Instance.StationName + ";UTaskStat:Running;StErr:0;StErrInfo: ;");
					}
					CurStep=0;
					PictureNum=1;
					SetStep(0,"Reset flag");
					if(task.RunMode== TaskRunMode.Qualify)
					{
						SetStep(3,"start Qualify");
					}
					((IVpAssemble) _vpAssemble).OnProcessFinished -= new UpdateDelegate(OnProcessFinished_Handle); 
			        ((IVpAssemble) _vpAssemble).OnProcessFinished += new UpdateDelegate(OnProcessFinished_Handle);
			
			        ((IVpAssemble) _vpAssemble).OnAlignResultRecieved -= new UpdateDelegate(OnAlignResultRecieved_Handle);
			        ((IVpAssemble) _vpAssemble).OnAlignResultRecieved += new UpdateDelegate(OnAlignResultRecieved_Handle);
			
			        ((IVpAssemble) _vpAssemble).OnRepeatResultRecieved -= new UpdateDelegate(OnRepeatResultRecieved_Handle);
			        ((IVpAssemble) _vpAssemble).OnRepeatResultRecieved += new UpdateDelegate(OnRepeatResultRecieved_Handle);
			        
			        ((IVpAssemble) _vpAssemble).OnStationOffsetRecieved -= new UpdateDelegate(OnStationOffsetRecieved_Handle);
					((IVpAssemble) _vpAssemble).OnStationOffsetRecieved += new UpdateDelegate(OnStationOffsetRecieved_Handle);
					
				}
				var val=DataInfoManager.Instance.DataInfoGetVal("TrayReady");
				if(val!=null)
				{
					TrayReady=(bool)val;
				}
				val=DataInfoManager.Instance.DataInfoGetVal("Alarming");
				if(val!=null)
				{
					Alarming=(bool)val;
				}
			    val=DataInfoManager.Instance.DataInfoGetVal("UplineStep");
				if(val!=null)
				{
					UplineStep=(Int32)val;
				}
				val=DataParamManager.Instance.GetDataParamByName("DryRunMode").DataVal;
				if(val!=null)
				{
					DryRunMode=(bool)val;
				}
				val=DataParamManager.Instance.GetDataParamByName("ZAxisSafeHeight").DataVal;
				if(val!=null)
				{
					ZAxisSafeHeight=(double)val;
				}
				else
				{
					ZAxisSafeHeight=-100;
				}

				switch(CurStep)
				{
					case 0:
						#region
						if(CheckAutoRun())
						{
							StartFlag=false;
							Running=true;
							Resetting=false;
							AutoRunning=true;
							isScrewPicTaken = false;
							ScrewDownTryTime=0;
							Capture = false;
							CapCalcFinish = false;
							Screw1TryTime=0;
							Screw2TryTime=0;
							Screw3TryTime=0;
							Screw4TryTime=0;
							SetStep(5,"CylScrew retract");
						}
						#endregion
						break;	
					case 3:   //Qualify Mode
						#region
						if(CheckAutoRun())
						{
							Capture = false;
							CapCalcFinish = false;
							if(task.RunMode== TaskRunMode.Qualify)
							{
								var val3=DataInfoManager.Instance.DataInfoGetVal("QyReady");
								bool QyReady=(val3!=null?(bool)val3:false);
								if(QyReady)
								{
									SetStep(5,"CylScrew retract");
								}
							}
						}
						break;
						#endregion
					case 5://Screwdriver retract into nozzle
						#region
						if(CheckAutoRun())
						{
							((IEleCyldLine)_eleCylScrewDown).CyldRetract();
							bool ret=Wait(()=>{ return ((IEleCyldLine)_eleCylScrewDown).ReadRestractDI()==1; },3000);
							if(ret)
							{
								SetStep(7,"AxisGroup jump to PickScrew");
							}
							else
							{
								SetTaskError(true,AlarmLv.Error,5,"ScrewDown restract failed",task,Exceptions.Cylinder);
								SetStep(-5,"CylScrew retract");
								goto RunStepErr;
							}
						}	
						#endregion										
						break;
					case 7://Check Vac is turned off
						#region
						if(CheckAutoRun())
						{
							if(((IEleCyldLine)_eleDoVaccum).ReadStretchDO()==1||((IEleCyldLine)_eleDoVaccum).ReadStretchDI()==1)
							{
								var ret=((IEleAxisGroup)_eleMotion).Jump(AxisGroup,ScrewThrowPos,MoveVelGroup,-100,100000,false,-1000);
								if(ret==0)
								{									
									SetStep(8,"Vac Restract");
								}
								else
								{
									SetTaskError(true,AlarmLv.Error,7,"AxisGroup jump to ScrewThrowPos failed",task,Exceptions.AxisMove);
									SetStep(-7);
									goto RunStepErr;
								}
							}
							else
							{
								SetStep(10,"AxisGroup jump to PickScrew");
							}
						}
						#endregion						
						break;
					case 8://Turn off Vac for screwthrow
						#region
						if(CheckAutoRun())
						{
							((IEleCyldLine)_eleDoVaccum).CyldRetract();
							Thread.Sleep(1000);							
							((IEleCyldLine)_eleDoVaccum).CyldStretch(false);
							bool ret=Wait(()=>{ return ((IEleCyldLine)_eleDoVaccum).ReadStretchDI()==1; },1000);
							if(ret)
							{	
								SetTaskError(true,AlarmLv.Error,8,"Throw the screw failed",task,Exceptions.Cylinder);
								MessageBox.Show("Throw the screw failed!Please check the vaccum,if there exsits a screw then take away it.");
								SetStep(-8);
								goto RunStepErr;
							}
							else
							{
								((IEleCyldLine)_eleDoVaccum).CyldRetract();
								SetStep(10,"ScrewDown Restract and AxisGroup jump to PickScrew");
							}
						}
						#endregion						
						break;
					case 10://ScrewDown Retract and AxisGroup jump to PickScrew 
						#region
						if(CheckAutoRun())
						{
							if(((IEleDI)_eleDiScrewReady).Read())
							{
								((IEleCyldLine)_eleCylScrewDown).CyldRetract();
								if(((IEleCyldLine)_eleCylScrewDown).ReadRestractDI()==1)
								{
									var ret=((IEleAxisGroup)_eleMotion).Jump(AxisGroup,PickScrew,MoveVelGroup,-60,100000,false,-1000);
									if(ret==0)
									{
										SetStep(30,"ScrewDown Cyld retract and Vac on");
									}
									else
									{
										SetTaskError(true,AlarmLv.Error,10,"AxisGroup jump to PickScrew failed",task,Exceptions.AxisMove);
										SetStep(-10);
										goto RunStepErr;
									}
								}							
							}
							else if(!((IEleDI)_eleDiScrewReady).Read())
							{
								SetTaskError(true,AlarmLv.Error,10,"Screw is not ready,please check!",Exceptions.Cylinder);
								Thread.Sleep(20);
								MessageBox.Show("Screw is not ready,please check!");
							}
						}	
						#endregion
						break;
					case 30://ScrewDown retract and vac on
						#region
						if(CheckAutoRun())
						{
							//((IEleCyldLine)_eleCylScrewDown).CyldRetract();
							if(((IEleCyldLine)_eleCylScrewDown).ReadRestractDI()==1)
							{
								if(!CheckDryRun())
								{
									((IEleCyldLine)_eleDoVaccum).CyldStretch(false);
									Thread.Sleep(500);
								}		
								SetStep(40,"AxisGroup move to ScrewCheck");							
							}
						}
						#endregion
						break;
					case 40://AxisGroup jump to ScrewCheck
						#region
						if(CheckAutoRun())
						{
							var ret=((IEleAxisGroup)_eleMotion).Jump(AxisGroup,ScrewCheck,MoveVelGroup,-20,100000,false,-1000);
							if(ret==0)
							{
								Detect = false;
								DataInfoManager.Instance.DataInfoSetVal("Detect",Detect);
								SetStep(50,"CCD Light2 on and check screw ");
							}	
							else
							{
								SetTaskError(true,AlarmLv.Error,40,"AxisGroup jump to ScrewCheck failed",task,Exceptions.AxisMove);
								SetStep(-40);
								goto RunStepErr;
							}
						}
						#endregion						
						break;
					case 50://CCDLight2 on，take picture to check screw ready
						#region
						if(CheckAutoRun())
						{
							((IEleDO)_eleDoTrigLight2).Write(true);
							Thread.Sleep(50);
							switch(count)
							{
								case 0:
									Screw1TryTime++;
									break;
								case 1:
									Screw2TryTime++;
									break;
								case 2:
									Screw3TryTime++;
									break;
								case 3:
									Screw4TryTime++;
									break;											
							}
							
							//if(((IVpDetector)_vpDetector).SetCapture(out Detect)==false)
							if(!CheckDryRun())
							{
								if(((IVpDetector)_vpDetector).SetCapture(out Detect,PictureNum++,"007","Station6")==false) //using VPdetector, output true/false
								{	
									//ScrewWorkErrSolution(50);
									SetStep(500);
								}
								else
								{
									((IEleDO)_eleDoTrigLight2).Write(false);
									SetStep(51,"processing data");
								}
							}
							else if(CheckDryRun())
							{
								((IVpDetector)_vpDetector).SetCapture(out Detect,PictureNum++,"007","Station6");
								Thread.Sleep(500);
								((IEleDO)_eleDoTrigLight2).Write(false);
								SetStep(70,"AxisGroup move to ScrewPicPos");
							}
							if(task.RunMode == TaskRunMode.Qualify)
							{
								if(Detect)
								{
									DataInfoManager.Instance.DataInfoSetVal("TaskResult",0);
								}
								else
								{
									DataInfoManager.Instance.DataInfoSetVal("TaskResult",-1);
								}
								DataInfoManager.Instance.DataInfoSetVal("Dector",Detect);
								DataInfoManager.Instance.DataInfoSetVal("QyReady",false);
								SetStep(3);
							}
						}			
						#endregion										
						break;
					case 500:
						#region 
						if(CheckAutoRun())
						{
							((IEleDO)_eleDoTrigLight2).Write(false);
							
			    			if(DnCamErrCount>=DnCamErrCountMax)
			    			{
			    				SetTaskError(true,AlarmLv.Error,500,string.Format("Down Camera Count Bigger Than {0}!Please check!",DnCamErrCountMax.ToString()),Exceptions.Photo);
								MessageBox.Show(string.Format("Down Camera Count Bigger Than {0}!Please check!",DnCamErrCountMax.ToString()));
								DnCamErrCount=0;
								SetStep(50);  				
			    			}
			    			else
			    			{
			    				DnCamErrCount++;
		    					SetStep(5,"ScrewDown retract");
			    			}
						}
						#endregion						
						break;
					case 51://Date get
						#region
						if(Detect) //screw detect
						{
							string Pic="Pic"+count.ToString();
							DataInfoManager.Instance.DataInfoSetVal(Pic,true);
							SetStep(70,"AxisGroup move to ScrewPicPos");
							DnCamErrCount=0;									
						}
						else
						{
							string Pic="Pic"+count.ToString();
							DataInfoManager.Instance.DataInfoSetVal(Pic,false);
							SetStep(510);
						}
						#endregion
						break;
					case 510:
						#region
						if(CheckAutoRun())
						{
							if(DnCamErrCount>=DnCamErrCountMax)
			    			{
			    				SetTaskError(true,AlarmLv.Error,510,"Get offeset failed Than {0}!Please check!",Exceptions.Offset);
								MessageBox.Show(string.Format("Get offeset failed Than {0}!Please check!",DnCamErrCountMax.ToString()));
								DnCamErrCount=0;
								SetStep(51);
			    			}
			    			else
			    			{
			    				DnCamErrCount++;
			    				var ret=((IEleAxisGroup)_eleMotion).Jump(AxisGroup,ScrewThrowPos,MoveVelGroup,-100,100000,false,-1000);
			    				if(ret==0)
			    				{
			    					SetStep(5);
			    				}
			    				else
			    				{
			    					if(DnCamErrCount>0)
			    					{
			    						DnCamErrCount=DnCamErrCount-1;
			    					}
			    					SetTaskError(true,AlarmLv.Error,510,"AxisGroup jump to ScrewThrowPos failed",task,Exceptions.AxisMove);
									SetStep(-510);
									goto RunStepErr;
			    				}
			    			}
						}
						
						#endregion
						break;
					case 70://AxisGroup move to ScrewPicPos
						#region
						if(CheckAutoRun())
						{
							if(isScrewPicTaken == false)
							{
								var ret=((IEleAxisGroup)_eleMotion).MoveAbs(AxisGroup1,new double[]{ScrewPicPos[0],ScrewPicPos[1]},MoveVelGroup1,100000,false);
								Capture = false;
								DataInfoManager.Instance.DataInfoSetVal("Capture", Capture);
								CapCalcFinish = false;
								DataInfoManager.Instance.DataInfoSetVal("CapCalcFinish", CapCalcFinish);	
								if(ret==0)
								{
									SetStep(80,"take picture");
								}
								else
								{
									SetTaskError(true,AlarmLv.Error,70,"AxisGroup move to ScrewPicPos failed",task,Exceptions.AxisMove);
									SetStep(-70);
									goto RunStepErr;
								}
							}
							else
							{
								SetStep(90,"AxisGroup move to work position");
							}													
						}					
						#endregion		
						break;
					case 80://CCDLight1 on，take picture to check product ready
						#region
						if(isScrewPicTaken == false)
						{
							if(TrayReady&&CheckAutoRun())
							{
								if(!CheckDryRun())
								{
									((IEleDO)_eleDoTrigLight1).Write(true);
									Thread.Sleep(50);
									if(((IVpAssemble)_vpAssemble).SetCapture(0,0,0,PictureNum++,"007","Station6")==false) //capture using clamp0, station0
									{
										SetStep(8000);
									}
									else
									{
										isScrewPicTaken = true;
										upPicTakeTime=DateTime.Now;
										SetStep(81,"Wait for offeset");
									}
								}
								else
								{
									((IEleDO)_eleDoTrigLight1).Write(true);
									Thread.Sleep(50);
									((IVpAssemble)_vpAssemble).SetCapture(0,0,0,PictureNum++,"007","Station6");
									Thread.Sleep(500);
									((IEleDO)_eleDoTrigLight1).Write(false);
									isScrewPicTaken = true;
									ActualScrewPos1=new double[]{ScrewPos1[0], ScrewPos1[1], ScrewPos1[2] - 5};
									ActualScrewPos2=new double[]{ScrewPos2[0], ScrewPos2[1], ScrewPos2[2] - 5};
									ActualScrewPos3=new double[]{ScrewPos3[0], ScrewPos3[1], ScrewPos3[2] - 5};
									ActualScrewPos4=new double[]{ScrewPos4[0], ScrewPos4[1], ScrewPos4[2] - 5};
									SetStep(90,"AxisGroup move to work position");
								}
							}
						}
						else
						{
							SetStep(90,"AxisGroup move to work position");
						}							
						#endregion
						break;
					case 81:  //Offset get CapResult filled in OnAlignRecievedHandle
						#region
						if(Capture && CapCalcFinish) //these are set by seperate events
						{
							((IEleDO)_eleDoTrigLight1).Write(false);
							ActualScrewPos1=new double[]{ScrewPos1[0] + CapResultX1, ScrewPos1[1] + CapResultY1, ScrewPos1[2]};
							ActualScrewPos2=new double[]{ScrewPos2[0] + CapResultX2, ScrewPos2[1] + CapResultY2, ScrewPos2[2]};
							ActualScrewPos3=new double[]{ScrewPos3[0] + CapResultX3, ScrewPos3[1] + CapResultY3, ScrewPos3[2]};
							ActualScrewPos4=new double[]{ScrewPos4[0] + CapResultX4, ScrewPos4[1] + CapResultY4, ScrewPos4[2]};
							UpCamErrCount=0;
							SetStep(90,"AxisGroup move to work position");
						}
						else 
						{
							if(isTimeOut(upPicTakeTime,3)==true)
							{
								SetStep(8000);
							}
						}
						#endregion
						break;
					case 8000:
						#region
						if(CheckAutoRun())
						{
							((IEleDO)_eleDoTrigLight1).Write(false);
		    				if(UpCamErrCount>=UpCamErrCountMax)
		    				{
		    					MessageBox.Show(string.Format("Up Camera photograph the product more Than {0} times! This product will be judged 'NG' and outflow!",UpCamErrCountMax.ToString()));
		    					UpCamErrCount=0;
		    					SetStep(1000);
		    				}
		    				else
		    				{
		    					DialogResult dr=MessageBox.Show("AutoRun 8000 Err:Up Camera photograph failed! retry？", "Information", MessageBoxButtons.RetryCancel, MessageBoxIcon.Question);
		    					if(dr==DialogResult.Retry)
		    					{
		    						isScrewPicTaken=false;
			    					UpCamErrCount++;
			    					SetStep(80);
		    					}
		    					else
		    					{
			    					MessageBox.Show("Up Camera photograph failed! This product will be judged 'NG' and outflow!");
		    						UpCamErrCount=0;
		    						SetStep(1000);
		    					}
		    				}
						}						
						#endregion
						break;
					case 90: //Get ScrewPos and move to ScrewPos
						#region
						switch(count)
						{
							case 0:
								ScrewPos=ActualScrewPos1;
								break;
							case 1:
								ScrewPos=ActualScrewPos2;
								break;
							case 2:
								ScrewPos=ActualScrewPos3;
								break;
							case 3:
								ScrewPos=ActualScrewPos4;
								break;
						}	
						if(CheckAutoRun()&&TrayReady&&((IEleCyldLine)_eleCylPush).ReadStretchDI()==1)
						{
							//var ret=_eleMotion.Jump(AxisGroup,ScrewPos,MoveVelGroup,-100,100000,false,-1000);
					    	var ret=((IEleAxisGroup)_eleMotion).MoveAbs(AxisGroup[2],ZAxisSafeHeight,MoveVelGroup[2],-1,false);
					    	if(ret!=0)
					    	{
					    		SetTaskError(true,AlarmLv.Error,90,"AxisGroup move to ScrewPicPos failed",task,Exceptions.AxisMove);
								SetStep(-90);
								goto RunStepErr;
					    	}
					    	ret=((IEleAxisGroup)_eleMotion).MoveAbs(AxisGroup1,new double[]{ScrewPos[0],ScrewPos[1]},MoveVelGroup1,100000,false);
					    	if(ret!=0)
					    	{
					    		SetTaskError(true,AlarmLv.Error,90,"AxisGroup move to ScrewPicPos failed",task,Exceptions.AxisMove);
								SetStep(-90);
								goto RunStepErr;
					    	}
					    	ret=((IEleAxisGroup)_eleMotion).MoveAbs(AxisGroup[2],ScrewPos[2],MoveVelGroup[2],-1,false);
					    	if(ret!=0)
					    	{
					    		SetTaskError(true,AlarmLv.Error,90,"AxisGroup move to ScrewPicPos failed",task,Exceptions.AxisMove);
								SetStep(-90);
								goto RunStepErr;
					    	}
					    	else if(ret==0&&CheckAutoRun())
							{
					    		SetStep(100,"Check screw ");
							}					    	
						}
						else if(TrayReady&&((IEleCyldLine)_eleCylPush).ReadStretchDI()==0)
				    	{
				    		MessageBox.Show("CyldPush is err position!");
				    		Thread.Sleep(1000);
				    	}
						#endregion
						break;
					case 100://check screw and if yes CyldScrewDown stretch //check vacuum and if vaccum is good continue
						#region
						if(CheckAutoRun())
						{
							if(((IEleCyldLine)_eleDoVaccum).ReadStretchDI()==0)
							{
								if(!CheckDryRun())
								{
									SetStep(5,"Maybe the screw is missing,Pick screw again");
								}	
								else	
								{
									SetStep(101,"ScrewDown cyld stract");
								}
							}
							else
							{
								SetStep(101,"ScrewDown cyld stract");
							}
						}
						#endregion
						break;
					case 101://ScrewDown stretch
						#region
						if(CheckAutoRun())
						{
							((IEleCyldLine)_eleCylScrewDown).CyldStretch(false);
							bool ret=Wait(()=>{ return ((IEleCyldLine)_eleCylScrewDown).ReadStretchDI()==1; },3000);
							if(ret)
							{
								SetStep(102,"Screw start work");
							}
							else
							{
								SetTaskError(true,AlarmLv.Error,101,"ScrewDown stretch failed",task,Exceptions.Cylinder);
								DialogResult dr=MessageBox.Show("AutoRun 101 Err:ScrewDown stretch failed! retry？", "Information", MessageBoxButtons.RetryCancel, MessageBoxIcon.Question);
								if(dr==DialogResult.Retry)
								{
									SetStep(1010,"ScrewDown retract");													
								}
								else
								{
									SetStep(1000,"AutoRun 101 messagebox click 'cancel' ,so this product judged 'NG' and outflow");
								}
							}
						}		
						#endregion					
						break;
					case 1010://ScrewDown retract
						#region
						if(CheckAutoRun())
						{
							((IEleCyldLine)_eleCylScrewDown).CyldRetract(false);
							bool ret=Wait(()=>{ return ((IEleCyldLine)_eleCylScrewDown).ReadRestractDI()==1; },3000);
							if(ret)
							{
								SetStep(1011);
							}
							else
							{
								SetTaskError(true,AlarmLv.Error,1010,"ScrewDown restract failed",task,Exceptions.Cylinder);
								SetStep(-1010,"CylScrew retract");
								goto RunStepErr;
							}
						}		
						#endregion					
						break;
					case 1011://ScrewDown retry times
						#region
						if(CheckAutoRun())
						{
							if(ScrewDownTryTime>=3)
							{
								ScrewDownTryTime=0;
								SetStep(1000);
							}
							else
							{
								ScrewDownTryTime++;
								SetStep(101);//retry
							}	
						}		
						#endregion					
						break;						
					case 102://Screw start work
						#region
						if(CheckAutoRun())
						{
							if(!CheckDryRun())
							{
								if(ScrewStartWork(true)==true)
								{
									if(WaitScrewDone(true)==true)
									{
										if(ScrewStopWork(true)==true)
										{
											((IEleCyldLine)_eleDoVaccum).CyldRetract();
											((IEleCyldLine)_eleCylScrewDown).CyldRetract();
											SetStep(110,"Check count ");
										}
										else
										{
											SetStep(1020);
										}
									}
									else
									{
										SetStep(1020);
									}
								}
								else
								{
									SetStep(1020);
								}
							}
							else if(CheckDryRun())
							{
								((IEleCyldLine)_eleCylScrewDown).CyldRetract();
								SetStep(110,"Check count ");
							}
						}		
						#endregion				
						break;
					case 1020:
						#region
						if(CheckAutoRun())
						{
							((IEleDO)_eleDoTrigScrewWork).Write(false);
			    			((IEleCyldLine)_eleCylScrewDown).CyldRetract();	
		    				bool ret=Wait(()=>{ return ((IEleCyldLine)_eleCylScrewDown).ReadRestractDI()==1; },3000);
							if(ret)
							{
								SetStep(1025,"AxisGroup first move to ScrewPicPos[2]-30...");
							}
							else
							{
								SetTaskError(true,AlarmLv.Error,1020,"ScrewDown restract failed",task,Exceptions.Cylinder);
								SetStep(-1020,"CylScrew retract");
								goto RunStepErr;
							}
						}
						#endregion
						break;
					case 1025:
						#region
						if(CheckAutoRun())
						{
							int ret=((IEleAxisGroup)_eleMotion).MoveAbs(AxisGroup[2],ScrewPos[2]-30,MoveVelGroup[2],-1,false);
					    	if(ret!=0)
					    	{
					    		SetTaskError(true,AlarmLv.Error,90,"AxisGroup move to ScrewPicPos[2]-30 failed",task,Exceptions.AxisMove);
								SetStep(-1025);
								goto RunStepErr;
					    	}
					    	else if(ret==0 && CheckAutoRun())
							{
					    		SetStep(1030,"Check _eleDoVaccum... ");
							}			
						}
						#endregion
						break;
					case 1030:
						#region
						if(CheckAutoRun())
						{
							if(((IEleCyldLine)_eleDoVaccum).ReadStretchDI()==1)
							{
								SetTaskError(true,AlarmLv.Error,1030,"AutoRun 1030 Err:Screw failed!",task,Exceptions.Cylinder);
								DialogResult dr=MessageBox.Show("AutoRun 1030 Err:Screw failed! retry？", "Information", MessageBoxButtons.RetryCancel, MessageBoxIcon.Question);
								if(dr==DialogResult.Retry)
								{
									SetStep(5,"Screw failed!Try again.");
								}
								else
								{
									IDataParam ff=DataParamManager.Instance.DataParamGetParam("IgnoreScrew");
									bool ignoreScrew=(val!=null?Convert.ToBoolean(ff.DataVal):false);
									if(!ignoreScrew)
									{
										SetStep(1000,"Screw failed!OP choose 'cancel' so that this product judged 'NG' and outflow.");
									}
									else
									{
										((IEleCyldLine)_eleDoVaccum).CyldRetract();
										((IEleCyldLine)_eleCylScrewDown).CyldRetract();
										SetStep(110,"Check count ");
									}
								}
							}
							else
							{
								IDataParam ff=DataParamManager.Instance.DataParamGetParam("IgnoreScrew");
								bool ignoreScrew=(val!=null?Convert.ToBoolean(ff.DataVal):false);
								if(!ignoreScrew)
								{
									SetStep(1000,"Screw failed!This product judged 'NG' and outflow.");
								}
								else
								{
									((IEleCyldLine)_eleDoVaccum).CyldRetract();
									((IEleCyldLine)_eleCylScrewDown).CyldRetract();
									SetStep(110,"Check count ");
								}
							}
						}
						#endregion
						break;
					case 110://Check cur-count 判断当前count，若为4，则置为0，且跳转至结束的case分支，如果小于4，则跳转至5
						#region
						count++; //move onto next screwpos
						ScrewDownTryTime=0;
						if(count >= ScrewNum)
						{
							count = 0;
							SetStep(111,"AxisGroup jump to screwpicpos");
						}
						else
						{
							SetStep(5,"Vac Check");		//loop back until all screws are finished
						}
						#endregion
						break;
					case 111://AxisGroup move to ScrewPicPos for final inspection
						#region
						if(CheckAutoRun())
						{
							if(((IEleCyldLine)_eleCylScrewDown).ReadRestractDI()==1)
							{
								var ret=((IEleAxisGroup)_eleMotion).MoveAbs(AxisGroup[2],WaitPos[2],MoveVelGroup[2],-1,false);
								if(ret!=0)
								{
									SetTaskError(true,AlarmLv.Error,111,"AxisGroup move to PicPos failed",task,Exceptions.AxisMove);
									SetStep(-111);
									goto RunStepErr;
								}
								ret=((IEleAxisGroup)_eleMotion).MoveAbs(AxisGroup1,new double[]{ScrewPicPos[0],ScrewPicPos[1]},MoveVelGroup1,100000,false);
								if(ret!=0)
								{
									SetTaskError(true,AlarmLv.Error,111,"AxisGroup move to PicPos failed",task,Exceptions.AxisMove);
									SetStep(-111);
									goto RunStepErr;
								}
								Capture = false;
								DataInfoManager.Instance.DataInfoSetVal("Capture", Capture);
								CapCalcFinish = false;
								DataInfoManager.Instance.DataInfoSetVal("CapCalcFinish", CapCalcFinish);										
								SetStep(112,"Take last picture");
							}
						}		
						#endregion				
						break;
					case 112://Detect the product
						#region
						if(CheckAutoRun())
						{
							if(!CheckDryRun())
							{
								((IEleDO)_eleDoTrigLight1).Write(true);
								Thread.Sleep(50);
								//if(((IVpAssemble)_vpAssemble).SetCapture(0,0,0)==false)
								if(((IVpAssemble)_vpAssemble).SetCapture(1,0,0,PictureNum++,"007","Station6")==false) //set capture using clamp1 station0
								{
									SetStep(1130);
								}
								else
								{
									upPicTakeTime=DateTime.Now;
									SetStep(113,"Wait for result");
								}
							}
							else if(CheckDryRun())
							{
								((IEleDO)_eleDoTrigLight1).Write(true);
								Thread.Sleep(50);
								((IVpAssemble)_vpAssemble).SetCapture(1,0,0,PictureNum++,"007","Station6");
								Thread.Sleep(500);
								((IEleDO)_eleDoTrigLight1).Write(false);
								SetStep(120,"AxisGroup move to wait position");
							}
						}
						#endregion
						break;						
					case 113://等待结果
						#region
						if(Capture && CapCalcFinish)
						{
							((IEleDO)_eleDoTrigLight1).Write(false);
							UpCamEndErrCount=0;
							
							//string Pic="Pic"+count.ToString();
							DataInfoManager.Instance.DataInfoSetVal("Pic5",true);
							SetStep(120,"AxisGroup move to wait position");
						}
						else
						{
							if(isTimeOut(upPicTakeTime,3)==true)
							{
								DataInfoManager.Instance.DataInfoSetVal("Pic5",false);
								SetStep(1130);
							}
						}
						#endregion
						break;
					case 1130:
						#region
						if(CheckAutoRun())
						{
							((IEleDO)_eleDoTrigLight1).Write(false);
		    				if(UpCamEndErrCount>=UpCamEndErrCountMax)
		    				{
		    					MessageBox.Show(string.Format("Up Camera detect failed more Than {0} times so that this product judged 'NG' and outflow.",UpCamEndErrCountMax.ToString()));
		    					UpCamEndErrCount=0;
		    					SetStep(1000);
		    				}
		    				else
		    				{
		    					DialogResult dr=MessageBox.Show("AutoRun 1130 Err:Up Camera detect failed! retry？", "Information", MessageBoxButtons.RetryCancel, MessageBoxIcon.Question);
		    					if(dr==DialogResult.Retry)
		    					{
			    					UpCamEndErrCount++;
			    					SetStep(112,"Up Camera detect failed so that photograh again.");
		    					}
		    					else
		    					{
		    						SetStep(1000,"Up Camera detect failed and OP choose 'cancel' so that this product judged 'NG' and outflow.");
		    					}
		    				}
						}
						#endregion
						break;
					case 120://轴系移动至等待位
						#region
						if(CheckAutoRun())
						{
							/*
							//var ret=_eleMotion.Jump(AxisGroup,WaitPos,MoveVelGroup,-100,100000,false,-1000);
							var ret=((IEleAxisGroup)_eleMotion).MoveAbs(AxisGroup1,new double[]{WaitPos[0],WaitPos[1]},MoveVelGroup1,100000,false);
							if(ret==0)
							{
								SetStep(130,"Task will over");
							}
							else
							{
								SetTaskError(true,AlarmLv.Error,120,"AxisGroup move to wait position failed",task);
								SetStep(-120);
								goto RunStepErr;
							}
							*/
							SetStep(130,"Task will over");
						}		
						#endregion																
						break;
					case 130://整体完成-Success
						#region
						SetStep(0,"Wait for next");
						TrayReady = false;
						PictureNum=1;
						isScrewPicTaken=false;
						DataInfoManager.Instance.DataInfoSetVal("Screw1TryTime",Screw1TryTime);
						DataInfoManager.Instance.DataInfoSetVal("Screw2TryTime",Screw2TryTime);
						DataInfoManager.Instance.DataInfoSetVal("Screw3TryTime",Screw3TryTime);
						DataInfoManager.Instance.DataInfoSetVal("Screw4TryTime",Screw4TryTime);
						Screw1TryTime=Screw2TryTime=Screw3TryTime=Screw4TryTime=0;
						count=0;
						DataInfoManager.Instance.DataInfoSetVal("TrayReady",TrayReady);
						DataInfoManager.Instance.DataInfoSetVal("TaskResult",0);
						string result="OK";
						DataInfoManager.Instance.DataInfoSetVal("FinallyResult",true);
						DataOutputManager.Instance.ClientDataOutputManagerUpdateState("Result", result.ToString());
						#endregion
						break;
					case 1000:
						#region
						if(CheckAutoRun())
						{
							((IEleCyldLine)_eleCylScrewDown).CyldRetract();
							if(((IEleCyldLine)_eleCylScrewDown).ReadRestractDI()==1)
							{
								var ret=((IEleAxisGroup)_eleMotion).Jump(AxisGroup,WaitPos,MoveVelGroup,-60,100000,false,-1000);
								if(ret==0)
								{
									SetStep(2000,"End the task");
								}
								else
								{
									SetTaskError(true,AlarmLv.Error,1000,"AxisGroup move to wait position failed",task,Exceptions.AxisMove);
									SetStep(-1000);
									goto RunStepErr;
								}
							}
						}		
						#endregion					
						break;
					case 2000: //failed
						#region
						SetStep(0,"Wait for next");
						PictureNum=1;
						TrayReady = false;
						isScrewPicTaken=false;
						DataInfoManager.Instance.DataInfoSetVal("Screw1TryTime",Screw1TryTime);
						DataInfoManager.Instance.DataInfoSetVal("Screw2TryTime",Screw2TryTime);
						DataInfoManager.Instance.DataInfoSetVal("Screw3TryTime",Screw3TryTime);
						DataInfoManager.Instance.DataInfoSetVal("Screw4TryTime",Screw4TryTime);
						Screw1TryTime=Screw2TryTime=Screw3TryTime=Screw4TryTime=0;
						count=0;
						DataInfoManager.Instance.DataInfoSetVal("TrayReady",TrayReady);
						DataInfoManager.Instance.DataInfoSetVal("TaskResult",10);
						string result1="NG";
						DataInfoManager.Instance.DataInfoSetVal("FinallyResult",false);
						DataOutputManager.Instance.ClientDataOutputManagerUpdateState("Result", result1.ToString());
						#endregion
						break;
				}
				RunStepErr:
					if(task.CurState==TaskMachineState.Continue&&CheckAutoRun())
					{
						task.CurState= TaskMachineState.Run;
						
						if(CheckConnectState())
						{
							((ISuperClient)_superClient).SendStationState("station:"+ DevRecipeManager.Instance.StationName + ";UTaskStat:Running;StErr:0;StErrInfo: ;");
						}
			
						if(CurStep<0)
						{
							CurStep=(short)-CurStep;
						}
					}
			}									
			return 0;
		}
		catch(Exception ex)
		{
			MessageBox.Show(ex.ToString());
			return -1;
		}
	}
	public int Init(ITask task)
	{
		int iRet = 0;
		TaskName = task.Name;
		//task.CurState = TaskMachineState.Run;
        
		Screw1TryTime=0;
		Screw2TryTime=0;
		Screw3TryTime=0;
		Screw4TryTime=0;
		#region get IPrim
		if(!GetPrim("LEIS0", ref _primMotionCard, 1))   goto TaskInitErr;   
		if(!GetPrim("VpAssemble0", ref _vpAssemble, 2))   goto TaskInitErr; 
		if(!GetPrim("VpDetector0", ref _vpDetector, 3))   goto TaskInitErr; 
		if(!GetPrim("SuperClient0", ref _superClient, 4)) goto TaskInitErr;
		#endregion
		#region get IEle
		if(!GetEle("Motion", ref _eleMotion, 1))   goto TaskInitErr; 
		if(!GetEle("Line1", ref _eleAxisAbove, 1))   goto TaskInitErr; 
		if(!GetEle("Line2", ref _eleAxisBelow, 1))   goto TaskInitErr; 
		if(!GetEle("AxisX", ref _eleAxisX, 1))   goto TaskInitErr; 
		if(!GetEle("AxisY", ref _eleAxisY, 1))   goto TaskInitErr; 
		if(!GetEle("AxisZ", ref _eleAxisZ, 1))   goto TaskInitErr; 
		
		if(!GetEle("Btn_Start", ref _eleBtnStart, 1))   goto TaskInitErr; 
		if(!GetEle("Btn_Stop", ref _eleBtnStop, 1))   goto TaskInitErr; 
		if(!GetEle("Btn_Reset", ref _eleBtnReset, 1))   goto TaskInitErr; 
		if(!GetEle("Btn_EMG1", ref _eleBtnEMG1, 1))   goto TaskInitErr; 
		if(!GetEle("Btn_EMG2", ref _eleBtnEMG2, 1))   goto TaskInitErr; 
		if(!GetEle("Btn_Lock", ref _eleBtnLock, 1))   goto TaskInitErr; 
		if(!GetEle("FUpDoor", ref _eleDidoor1, 1))  goto TaskInitErr; 
		if(!GetEle("BUpDoor", ref _eleDidoor2, 1))  goto TaskInitErr; 
		
		if(!GetEle("UpLineWork", ref _eleDiUplineWork, 1))   goto TaskInitErr; 
		if(!GetEle("UpLineCheck", ref _eleDiUpLineCheck, 1))   goto TaskInitErr; 
		//if(!GetEle("SuckerVac1", ref _eleDiSuckerVac, 1))   goto TaskInitErr; 
		if(!GetEle("ScrewWorking", ref _eleDiScrewWorking, 1))   goto TaskInitErr; 
		if(!GetEle("ScrewDone", ref _eleDiScrewDone, 1))   goto TaskInitErr; 
		//if(!GetEle("AirPressure", ref _eleDiAirPressure, 1))   goto TaskInitErr; 
		
		if(!GetEle("Vac1", ref _eleDoVaccum, 1))   goto TaskInitErr; 
		//if(!GetEle("Blow1", ref _eleDoBlow, 1))   goto TaskInitErr; 
		if(!GetEle("TrigCa1", ref _eleDoTrigCa1, 1))   goto TaskInitErr; 
		if(!GetEle("TrigCa2", ref _eleDoTrigCa2, 1))   goto TaskInitErr; 
		if(!GetEle("TrigLight1", ref _eleDoTrigLight1, 1))   goto TaskInitErr; 
		if(!GetEle("TrigLight2", ref _eleDoTrigLight2, 1))   goto TaskInitErr;
		if(!GetEle("ScrewWorkTrig", ref _eleDoTrigScrewWork, 1))   goto TaskInitErr; 	
		if(!GetEle("ScrewReady", ref _eleDiScrewReady,1))   goto TaskInitErr;
		
		if(!GetEle("CylScrew", ref _eleCylScrewDown, 1))   goto TaskInitErr;
		if(!GetEle("CylPush", ref _eleCylPush, 1))   goto TaskInitErr; 	
		#endregion
		AxisGroup=new[]{(IEleAxisLine)_eleAxisX,(IEleAxisLine)_eleAxisY,(IEleAxisLine)_eleAxisZ};
		HomeVelGroup=new[]{((IEleAxisLine)_eleAxisX).HomeVelocity,((IEleAxisLine)_eleAxisY).HomeVelocity,((IEleAxisLine)_eleAxisZ).HomeVelocity};
		MoveVelGroup=new[]{((IEleAxisLine)_eleAxisX).DefaultParams.Velocity,((IEleAxisLine)_eleAxisY).DefaultParams.Velocity,((IEleAxisLine)_eleAxisZ).DefaultParams.Velocity};
		
		AxisGroup1=new[]{(IEleAxisLine)_eleAxisX,(IEleAxisLine)_eleAxisY};
		HomeVelGroup1=new[]{((IEleAxisLine)_eleAxisX).HomeVelocity,((IEleAxisLine)_eleAxisY).HomeVelocity};
		MoveVelGroup1=new[]{((IEleAxisLine)_eleAxisX).DefaultParams.Velocity,((IEleAxisLine)_eleAxisY).DefaultParams.Velocity};
		#region PosList
		var pos = ElesManager.Instance.ListPos.FirstOrDefault(p => p.Name == "Wait");
        WaitPos=pos.Data();
        pos = ElesManager.Instance.ListPos.FirstOrDefault(p => p.Name == "PickScrew");
        PickScrew=pos.Data();
		pos = ElesManager.Instance.ListPos.FirstOrDefault(p => p.Name == "ScrewCheck");
        ScrewCheck=pos.Data();
        pos = ElesManager.Instance.ListPos.FirstOrDefault(p => p.Name == "ScrewPicPos");
        ScrewPicPos=pos.Data();
        pos = ElesManager.Instance.ListPos.FirstOrDefault(p => p.Name == "ScrewPos1");
        ScrewPos1=pos.Data();
        pos = ElesManager.Instance.ListPos.FirstOrDefault(p => p.Name == "ScrewPos2");
        ScrewPos2=pos.Data();
        pos = ElesManager.Instance.ListPos.FirstOrDefault(p => p.Name == "ScrewPos3");
        ScrewPos3=pos.Data();
        pos = ElesManager.Instance.ListPos.FirstOrDefault(p => p.Name == "ScrewPos4");
        ScrewPos4=pos.Data();
        pos = ElesManager.Instance.ListPos.FirstOrDefault(p => p.Name == "ScrewThrowPos");
        ScrewThrowPos=pos.Data();
		#endregion
		var val11=DataParamManager.Instance.DataParamGetParam("SocketEnable");
		SocketEnable=(val11!=null?(bool)val11.DataVal:false);
		task.TaskRunStat = TaskRunState.Inited;
		return iRet;
		TaskInitErr:
        	task.TaskRunStat = TaskRunState.Err;
        //goto TaskInitErr;
        return iRet;
	}
	public void LogAdd(string log,AlarmLv lv,int id,Exceptions exc)
	{
		var val = (int)exc;
		switch(lv)
		{
			case AlarmLv.Fatal:
                Log.Fatal(151000000 + id, log, LogClassification.Task, TaskName);
                break;
            case AlarmLv.Error:
               	Log.Error(251000000+id+val*100000,log,LogClassification.Task,TaskName);
               	ErrID = (251000000+id+val*100000).ToString();
				DataInfoManager.Instance.DataInfoSetVal("ErrID",ErrID);
                break;
            case AlarmLv.Warn:
                Log.Warn(351000000 + id, log, LogClassification.Task, TaskName);
                break;
            case AlarmLv.Info:
                Log.Info(451000000 + id, log, LogClassification.Task, TaskName);
                break;
            case AlarmLv.Debug:
                Log.Debug(551000000 + id, log, LogClassification.Task, TaskName);
                break;
            case AlarmLv.Trace:
                Log.Trace(651000000 + id, log, LogClassification.Task, TaskName);
                break;
		}
	}

	public enum Exceptions
    {
    	Cylinder = 1,
        Photo = 2,
        ImageDeal = 3,
        Offset = 4,
        AxisMove = 5,
        Detector = 6,
        Init = 7,
        MaterialLow = 8,
        Other = 9
    }
		
	public bool GetPrim(string primName, ref IPrim prim, int id)
    {
        prim = PrimsManager.Instance.GetPrimByName(primName);
        if (prim != null) return true;
        LogAdd(string.Format("{0} was not defined!",primName), AlarmLv.Debug, id,Exceptions.Init);
        MessageBox.Show(string.Format("{0} couldn't be found!",primName));
        return false;
    }
    public bool GetEle(string eleName, ref IEle ele, int id)
    {
        ele = ElesManager.Instance.GetEleByName(eleName);
        if (ele != null) return true;
         LogAdd(string.Format("{0} was not defined!",eleName), AlarmLv.Debug, id,Exceptions.Init);
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
    public bool CheckDryRun()
	{
		IDataParam objDry1 = DataParamManager.Instance.DataParamGetParam("DryRunMode");
    
        bool bDry1 = (objDry1 != null && objDry1.DataVal != null) ? (bool)objDry1.DataVal : false;
        
		var var = DataInfoManager.Instance.DataInfoGetVal("DryRunMode");
		bool bDry2 = (var != null)?(bool)var:false;
		
		DryRunMode = (bDry1 || bDry2);
		return DryRunMode;
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

		LogAdd(StepInfo, AlarmLv.Trace, (int)StepIndex,Exceptions.Other);
	}
	public void SetTaskError(bool errFlag, AlarmLv lv, int id, string errMsg,Exceptions exc)
	{
		if(errFlag)
		{
			TaskError = true;
			DataInfoManager.Instance.DataInfoSetVal("TaskError", TaskError);
			LogAdd(errMsg, lv, id,exc);
		}
		else
		{
			TaskError = false;
			DataInfoManager.Instance.DataInfoSetVal("TaskError", TaskError);
		}		
	}   
	public void SetTaskError(bool errFlag, AlarmLv lv, int id, string errMsg,ITask task,Exceptions exc)
	{
		if(errFlag)
		{
			task.CurState=TaskMachineState.Error;
			
			if(CheckConnectState())
			{
				((ISuperClient)_superClient).SendStationState("station:"+ DevRecipeManager.Instance.StationName + ";UTaskStat:Error;StErr:"+(25100000+id+(int)exc*1000) + ";StErrInfo:"+errMsg +";");
			}
			
			LogAdd(errMsg, lv, id,exc);
		}
		else
		{
			task.CurState=TaskMachineState.Other;
		}		
	}   		
	public bool ScrewStartWork(bool isCheck)
	{
		((IEleDO)_eleDoTrigScrewWork).Write(true);
		if(isCheck)
		{
			DateTime startTime = DateTime.Now;
			while(true)
			{
				if(((IEleDI)_eleDiScrewWorking).Read() == true)
					break;
				else
				{
					if(isTimeOut(startTime, 2))
						return false;
					else
						Thread.Sleep(50);
				}
			}
		}
		return true;
	}	
	public bool ScrewStopWork(bool isCheck)
	{
		((IEleDO)_eleDoTrigScrewWork).Write(false);
		if(isCheck)
		{
			DateTime startTime = DateTime.Now;
			while(true)
			{
				if(((IEleDI)_eleDiScrewWorking).Read() == false)
					break;
				else
				{
					if(isTimeOut(startTime, 2))
						return false;
					else
						Thread.Sleep(50);
				}
			}
		}
		return true;
	}	
	
	public bool WaitScrewDone(bool isCheck)
	{
		if(((IEleDO)_eleDoTrigScrewWork).Read()==false)
			return true;
		
		if(isCheck)
		{
			DateTime startTime=DateTime.Now;
			while(true)
			{
				if(((IEleDI)_eleDiScrewDone).Read()==true)
					break;
				else
				{
					if(isTimeOut(startTime,10)==true)
					{
						((IEleDO)_eleDoTrigScrewWork).Write(false);
						return false;
					}
					else
						Thread.Sleep(50);
				}
			}
		}
		return true;
	}	
	public bool isTimeOut(DateTime startTime, int TimeOut)
	{
		if( (DateTime.Now - startTime).TotalSeconds >=TimeOut )
			return true;
		else
			return false;
	}
	
	/// <summary>
	/// Wait function return true
	/// </summary>
	/// <param name="excuteFunc">need excute function</param>
	/// <param name="Timeout">timeout,it`s uint is millisecond</param>
	/// <returns>if function return true then return true,otherwise,return false</returns>
	public bool Wait(Func<bool> excuteFunc,int Timeout)
	{
		DateTime startTime=DateTime.Now;
		while(true)
		{
			if(excuteFunc()==true)
			{
				return true;
			}
			if((DateTime.Now - startTime).TotalMilliseconds>=Timeout)
			{
				return false;
			}
			Thread.Sleep(10);
		}
	}
	
	//需要根据具体的抓螺丝算法修改 //check that camera photo was taken
	public void OnProcessFinished_Handle(object sender)
    {
        ProcessInfo processInfo = (ProcessInfo)sender;
        if (!processInfo.result)
        {
            Capture = false;
            DataInfoManager.Instance.DataInfoSetVal("Capture", Capture);
            return;
        }
        else
        {
            Capture = true;
            DataInfoManager.Instance.DataInfoSetVal("Capture", Capture);
            return;
        }
        //processInfo.clampIndex 工具
        //processInfo.stationIndex 工站
        //processInfo.result 结果
    }

    public void OnRepeatResultRecieved_Handle(object sender)
    {
        VpAlignResult vpAlignResult = (VpAlignResult)sender;
        //vpAlignResult.clampIndex 工具
        //vpAlignResult.Offset 偏移值
        //VpAlignEnum.VpAlign_Err 错误
        //vpAlignEnum.VpAlign_Continue 多次处理，继续
        //VpAlignEnum.VpAlign_Finish 完成
    }

    public void OnAlignResultRecieved_Handle(object sender) //handles info for both station 0 captures, but will it handle station 1 captures? Why is station 1 not being used?
    {
    	if(CurStep==81)
    	{
    		if(sender==null)
    		{
    			SystemPause=true;
    			if(UpCamErrCount>=UpCamErrCountMax)
    			{
    				MessageBox.Show(string.Format("Null Offset! Up Camera Count Bigger Than {0}!",UpCamErrCountMax.ToString()));
    				UpCamErrCount=0;
    				CurStep=120;
    			}
    			else
    			{
    				UpCamErrCount++;
    				MessageBox.Show("Null Offset!Ready To Retry!");
    				Capture = false;
    				DataInfoManager.Instance.DataInfoSetVal("Capture", Capture);
    				CapCalcFinish = false;
    				DataInfoManager.Instance.DataInfoSetVal("CapCalcFinish", CapCalcFinish);
    				isScrewPicTaken =false;
    				CurStep=70;
    			}
    			return;
    		}
    		else
    		{
    			VpAlignResult[] vpAlignResultArray = (VpAlignResult[])sender;
    			bool tempCapCalcFinish = true;
    			for(int i=0;i<ScrewNum;i++)
    			{
    				if (vpAlignResultArray[i].Result == VpAlignEnum.VpAlign_Finish)
        			{
            			switch(i)
            			{
            				case 0:
            					CapResultX1 = vpAlignResultArray[i].Offset.X;
            					CapResultY1 = vpAlignResultArray[i].Offset.Y;
            					DataInfoManager.Instance.DataInfoSetVal("CapResultX1", CapResultX1);
            					DataInfoManager.Instance.DataInfoSetVal("CapResultY1", CapResultY1);
            					break;
            				case 1:
            					CapResultX2 = vpAlignResultArray[i].Offset.X;
            					CapResultY2 = vpAlignResultArray[i].Offset.Y;
            					DataInfoManager.Instance.DataInfoSetVal("CapResultX2", CapResultX2);
            					DataInfoManager.Instance.DataInfoSetVal("CapResultY2", CapResultY2);
            					break;
            				case 2:
            					CapResultX3 = vpAlignResultArray[i].Offset.X;
            					CapResultY3 = vpAlignResultArray[i].Offset.Y;
            					DataInfoManager.Instance.DataInfoSetVal("CapResultX3", CapResultX3);
            					DataInfoManager.Instance.DataInfoSetVal("CapResultY3", CapResultY3);
            					break;
            				case 3:
            					CapResultX4 = vpAlignResultArray[i].Offset.X;
            					CapResultY4 = vpAlignResultArray[i].Offset.Y;
            					DataInfoManager.Instance.DataInfoSetVal("CapResultX4", CapResultX4);
            					DataInfoManager.Instance.DataInfoSetVal("CapResultY4", CapResultY4);
            					break;
            			}
        			}
        			else
        			{
            			tempCapCalcFinish = false;
            			break;
        			}
    			}
    			CapCalcFinish = tempCapCalcFinish;
    			DataInfoManager.Instance.DataInfoSetVal("CapCalcFinish", CapCalcFinish);
    		}
    	}
    	
    	
    	if(CurStep==113)
    	{
    		if(sender==null)
    		{
    			SystemPause=true;
    			if(UpCamEndErrCount>=UpCamEndErrCountMax)
    			{
    				MessageBox.Show(string.Format("Null Offset! Up Camera End Error Count Bigger Than {0}!",UpCamEndErrCountMax.ToString()));
    				UpCamEndErrCount=0;
    				CurStep=120;
    			}
    			else
    			{
    				UpCamEndErrCount++;
    				MessageBox.Show("Null Offset!Ready To Retry!");
    				Capture = false;
    				DataInfoManager.Instance.DataInfoSetVal("Capture", Capture);
    				CapCalcFinish = false;
    				DataInfoManager.Instance.DataInfoSetVal("CapCalcFinish", CapCalcFinish);
    				isScrewPicTaken =false;
    				CurStep=111;
    			}
    			return;
    		}
    		else
    		{
    			VpAlignResult[] vpAlignResultArray = (VpAlignResult[])sender;
    			bool tempCapCalcFinish = true;
    			for(int i=0;i<ScrewNum;i++)
    			{
    				if (vpAlignResultArray[i].Result != VpAlignEnum.VpAlign_Finish)
        			{
            			tempCapCalcFinish = false;
            			break;
        			}
    			}
    			CapCalcFinish = tempCapCalcFinish;
    			DataInfoManager.Instance.DataInfoSetVal("CapCalcFinish", CapCalcFinish);
    		}
    	}

        //vpAlignResult.clampIndex 工具
        //vpAlignResult.Offset 偏移值
        //VpAlignEnum.VpAlign_Err 错误
        //vpAlignEnum.VpAlign_Continue 多次处理，继续
        //VpAlignEnum.VpAlign_Finish 完成
    }
    
    public void OnStationOffsetRecieved_Handle(object sender)
	{
		//
	}
	

    //错误处理方法
    private void ScrewWorkErrSolution(int Index)
    {
    	switch(Index)
    	{
    		case 40:
    			SystemPause = true;
    			var result=MessageBox.Show("AutoRun 40 Err:Vacum Suck Error! Continue？", "Information", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Question);
    			if (result == DialogResult.Retry||result== DialogResult.Ignore)
    			{
    				CurStep = 40;
    			}
    			else if(result == DialogResult.Abort)
    			{
    				CurStep=1000;
    			}
    			break; 	
    		case 50:
    			SystemPause = true;
    			((IEleDO)_eleDoTrigLight2).Write(false);
    			if(DnCamErrCount>=DnCamErrCountMax)
    			{
    				SetTaskError(true,AlarmLv.Error,50,"Down Camera Count Bigger Than {0}!Please check!",Exceptions.Photo);
					MessageBox.Show(string.Format("Down Camera Count Bigger Than {0}!Please check!",DnCamErrCountMax.ToString()));
					DnCamErrCount=0;
					CurStep=50;   				
    			}
    			else
    			{
    				DnCamErrCount++;
    				var ret=((IEleAxisGroup)_eleMotion).Jump(AxisGroup,ScrewThrowPos,MoveVelGroup,-100,100000,false,-1000);
    				//VacumBlow(false);
    				if(ret==0)
    				{
    					((IEleCyldLine)_eleDoVaccum).CyldRetract();
    					((IEleCyldLine)_eleDoVaccum).WriteRestractDO(true);
    					Thread.Sleep(500);
    					((IEleCyldLine)_eleDoVaccum).WriteRestractDO(false);
    					CurStep = 10;
    				}
    			}
    			break;
    		case 51:
    			SystemPause = true;
    			if(DnCamErrCount>=DnCamErrCountMax)
    			{
    				SetTaskError(true,AlarmLv.Error,51,"Get offeset failed Than {0}!Please check!",Exceptions.Offset);
					MessageBox.Show(string.Format("Get offeset failed Than {0}!Please check!",DnCamErrCountMax.ToString()));
					DnCamErrCount=0;
					CurStep=51;   				
    			}
    			else
    			{
    				DnCamErrCount++;
    				var ret=((IEleAxisGroup)_eleMotion).Jump(AxisGroup,ScrewThrowPos,MoveVelGroup,-100,100000,false,-1000);
    				//VacumBlow(false);
    				if(ret==0)
    				{
    					((IEleCyldLine)_eleDoVaccum).CyldRetract();
    					((IEleCyldLine)_eleDoVaccum).WriteRestractDO(true);
    					Thread.Sleep(500);
    					((IEleCyldLine)_eleDoVaccum).WriteRestractDO(false);
    					CurStep = 10;
    				}
    			}
    			break;
    		case 80:
    			SystemPause = true;
    			((IEleDO)_eleDoTrigLight1).Write(false);
    			if (MessageBox.Show("AutoRun 80 Err:Failed Detector Up!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK)
    			{
    				if(UpCamErrCount>=UpCamErrCountMax)
    				{
    					MessageBox.Show(string.Format("Up Camera Count Bigger Than {0}!",UpCamErrCountMax.ToString()));
    					UpCamErrCount=0;
    					CurStep=120;
    				}
    				else
    				{
    					UpCamErrCount++;
    					CurStep=70;
    					MessageBox.Show("Ready To Retry!");
    				}
    			}
    			else
    			{
    				CurStep=2000;
    			}
    			break;
    		case 81:
    			SystemPause = true;
    			((IEleDO)_eleDoTrigLight1).Write(false);
    			if (MessageBox.Show("AutoRun 81 Err:Up Capture Result TimeOut!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK)
    			{
    				if(UpCamErrCount>=UpCamErrCountMax)
    				{
    					MessageBox.Show(string.Format("Up Camera Count Bigger Than {0}!",UpCamErrCountMax.ToString()));
    					UpCamErrCount=0;
    					CurStep=1000;
    				}
    				else
    				{
    					UpCamErrCount++;
    					Capture = false;
    					CapCalcFinish = false;
    					DataInfoManager.Instance.DataInfoSetVal("Capture", Capture);
    					DataInfoManager.Instance.DataInfoSetVal("CapCalcFinish", CapCalcFinish);
    					isScrewPicTaken = false;
    					CurStep = 70;
    					MessageBox.Show("Ready To Retry!");
    				}
    			}
    			else
    			{
    				CurStep=1000;
    			}
    			break;
    		case 90:
    			SystemPause = true;
    			if (MessageBox.Show("AutoRun 90 Err:Move To ScrewPos Error! Move To Throw Pos？", "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
    			{
    				var ret=((IEleAxisGroup)_eleMotion).Jump(AxisGroup,ScrewThrowPos,MoveVelGroup,-100,100000,false,-1000);
    				//VacumBlow(false);
    				((IEleCyldLine)_eleDoVaccum).CyldRetract();
    				((IEleCyldLine)_eleDoVaccum).WriteRestractDO(true);
					Thread.Sleep(500);
					((IEleCyldLine)_eleDoVaccum).WriteRestractDO(false);
    				CurStep = 10;
    			}
    			else
    			{
    				CurStep=100;
    			}
    			break;
    		case 100:
    			SystemPause = true;
    			if (MessageBox.Show("AutoRun 100 Err:No Screw! Pick Another One？", "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
    			{
    				((IEleCyldLine)_eleCylScrewDown).CyldRetract();
    				var ret=((IEleAxisGroup)_eleMotion).Jump(AxisGroup,ScrewThrowPos,MoveVelGroup,-100,100000,false,-1000);
    				((IEleCyldLine)_eleDoVaccum).CyldRetract();
    				((IEleCyldLine)_eleDoVaccum).WriteRestractDO(true);
					Thread.Sleep(500);
					((IEleCyldLine)_eleDoVaccum).WriteRestractDO(false);
    				CurStep = 10;
    			}
    			else
    			{
    				CurStep = 1000;
    			}
    			break;
    		case 101:
    			SystemPause = true;
    			((IEleCyldLine)_eleCylScrewDown).CyldRetract();
    			if(MessageBox.Show("AutoRun 101 Err:ScrewDown Cylinder Cannot Stretch Out! Pick Another One？", "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
    			{
    				var ret=((IEleAxisGroup)_eleMotion).Jump(AxisGroup,ScrewThrowPos,MoveVelGroup,-100,100000,false,-1000);
    				((IEleCyldLine)_eleDoVaccum).CyldRetract();
    				((IEleCyldLine)_eleDoVaccum).WriteRestractDO(true);
					Thread.Sleep(500);
					((IEleCyldLine)_eleDoVaccum).WriteRestractDO(false);
    				CurStep = 10;
    			}
    			else
    			{
    				CurStep = 1000;
    			}
    			break;
    		case 102:
    			SystemPause = true;
    			((IEleDO)_eleDoTrigScrewWork).Write(false);
    			((IEleCyldLine)_eleCylScrewDown).CyldRetract();
    			if (MessageBox.Show("AutoRun 102 Err:Can not Stop Screw Working! Move To Throw Pos？", "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
    			{
    				var ret=((IEleAxisGroup)_eleMotion).Jump(AxisGroup,ScrewThrowPos,MoveVelGroup,-100,100000,false,-1000);
    				((IEleCyldLine)_eleDoVaccum).CyldRetract();
    				((IEleCyldLine)_eleDoVaccum).WriteRestractDO(true);
					Thread.Sleep(500);
					((IEleCyldLine)_eleDoVaccum).WriteRestractDO(false);
    				CurStep = 10;
    			}
    			else
    			{
    				CurStep=1000;
    			}
    			break;
    		case 103:
    			SystemPause = true;
    			((IEleDO)_eleDoTrigScrewWork).Write(false);
    			((IEleCyldLine)_eleCylScrewDown).CyldRetract();
    			if (MessageBox.Show("AutoRun 103 Err:Screw Work Time Too Long! Move To Throw Pos？", "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
    			{
    				var ret=((IEleAxisGroup)_eleMotion).Jump(AxisGroup,ScrewThrowPos,MoveVelGroup,-100,100000,false,-1000);
    				((IEleCyldLine)_eleDoVaccum).CyldRetract();
    				((IEleCyldLine)_eleDoVaccum).WriteRestractDO(true);
					Thread.Sleep(500);
					((IEleCyldLine)_eleDoVaccum).WriteRestractDO(false);
    				CurStep = 10;
    			}
    			else
    			{
    				CurStep=1000;
    			}
    			break;
    		case 104:
    			SystemPause = true;
    			((IEleDO)_eleDoTrigScrewWork).Write(false);
    			((IEleCyldLine)_eleCylScrewDown).CyldRetract();
    			if (MessageBox.Show("AutoRun 104 Err:Screw Can Not Start Work! Move To Throw Pos？", "Information", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
    			{
    				var ret=((IEleAxisGroup)_eleMotion).Jump(AxisGroup,ScrewThrowPos,MoveVelGroup,-100,100000,false,-1000);
    				((IEleCyldLine)_eleDoVaccum).CyldRetract();
    				((IEleCyldLine)_eleDoVaccum).WriteRestractDO(true);
					Thread.Sleep(500);
					((IEleCyldLine)_eleDoVaccum).WriteRestractDO(false);
    				CurStep = 10;
    			}
    			else
    			{
    				CurStep=1000;
    			}
    			break;
    		case 112:
    			SystemPause = true;
    			((IEleDO)_eleDoTrigLight1).Write(false);
    			if (MessageBox.Show("AutoRun 112 Err:Failed Detector Up!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK)
    			{
    				if(UpCamEndErrCount>=UpCamEndErrCountMax)
    				{
    					MessageBox.Show(string.Format("Up Camera End Error Count Bigger Than {0}!",UpCamEndErrCountMax.ToString()));
    					UpCamEndErrCount=0;
    					CurStep=120;
    				}
    				else
    				{
    					UpCamEndErrCount++;
    					CurStep=111;
    					MessageBox.Show("Ready To Retry!");
    				}
    			}
    			break;
    		case 113:
    			SystemPause = true;
    			((IEleDO)_eleDoTrigLight1).Write(false);
    			if (MessageBox.Show("AutoRun 113 Err:Failed Detector Up!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK)
    			{
    				if(UpCamEndErrCount>=UpCamEndErrCountMax)
    				{
    					MessageBox.Show(string.Format("Up Camera End Error Count Bigger Than {0}!",UpCamEndErrCountMax.ToString()));
    					UpCamEndErrCount=0;
    					CurStep=120;
    				}
    				else
    				{
    					UpCamEndErrCount++;
    					CurStep=111;
    					MessageBox.Show("Ready To Retry!");
    				}
    			}
    			break;
    			
    	}
    	return;
    }
    
    private void WriteCTLog(DateTime[] startTime,DateTime[] endTime,double TotalMoveTime)
    {
    	if(startTime.Length!=4 || endTime.Length!=4)
    		return;
    	
		CTLogfs = new FileStream(CTLogPath, System.IO.FileMode.Append, System.IO.FileAccess.Write);
		CTLogsw = new StreamWriter(CTLogfs, System.Text.Encoding.Default);
        CTLogsw.AutoFlush = true;
		
        string tempStr="";
        tempStr=GetDateString(DateTime.Now)+",";
        
        for(int i=0;i<startTime.Length;i++)
        {
        	tempStr=tempStr+(endTime[i]-startTime[i]).TotalSeconds.ToString("#.000")+",";
        }
        
        tempStr=tempStr+TotalMoveTime.ToString("#.000");
    	CTLogsw.WriteLine(tempStr);
    	CTLogsw.Close();
    	CTLogfs.Close();
    }
    
    private string GetDateString(DateTime myTime)
	{
		string tempstring="";

    	tempstring=myTime.Year.ToString()+"-"+((myTime.Month<10)?("0"+myTime.Month.ToString()):(myTime.Month.ToString()))+"-"+((myTime.Day<10)?("0"+myTime.Day.ToString()):(myTime.Day.ToString()));
    	tempstring=tempstring+"-";
    	tempstring=tempstring+((myTime.Hour<10)?("0"+myTime.Hour.ToString()):(myTime.Hour.ToString()))+":"+((myTime.Minute<10)?("0"+myTime.Minute.ToString()):(myTime.Minute.ToString()))+":"+
    		((myTime.Second<10)?("0"+myTime.Second.ToString()):(myTime.Second.ToString()))+"."+myTime.Millisecond.ToString();
		
    	return tempstring;
	}
    
    private void CreateNewCSV(string filePath)
	{
		CTLogfs = new FileStream(filePath, System.IO.FileMode.Append, System.IO.FileAccess.Write);
		CTLogsw = new StreamWriter(CTLogfs, System.Text.Encoding.Default);
        CTLogsw.AutoFlush = true;

        string tempstr="RecordTime,ScrewTime1,ScrewTime2,ScrewTime3,ScrewTime4,TotalMoveTime";
    	CTLogsw.WriteLine(tempstr);
    	
    	CTLogsw.Close();
    	CTLogfs.Close();
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


