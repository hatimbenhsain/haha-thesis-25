using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FMODUnity;
using Obi;
using UnityEngine;

public class MusicBeat : MonoBehaviour
{
    public class TimelineInfo{
        public int currentBar=0;
        public int currentBeat = 0;
        public int beatPosition = 0;
        public float currentTempo = 0;
        public int currentTime=0;
        public FMOD.StringWrapper lastMarker=new FMOD.StringWrapper();

    }

    public TimelineInfo timelineInfo;
    GCHandle timelineHandle;

    private FMOD.Studio.EventInstance musicInstance;
    FMOD.Studio.EVENT_CALLBACK beatCallback;

    public static bool newBeat;

    void Start()
    {
        musicInstance=GetComponent<StudioEventEmitter>().EventInstance;
        timelineInfo=new TimelineInfo();

        beatCallback=new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);

        timelineHandle=GCHandle.Alloc(timelineInfo);
        musicInstance.setUserData(GCHandle.ToIntPtr(timelineHandle));

        musicInstance.setCallback(beatCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
    }

    void Update()
    {
        newBeat=false;
        musicInstance.getTimelinePosition(out timelineInfo.currentTime);
    }

    void LateUpdate()
    {
        
    }

    void OnGUI()
    {
        GUILayout.Box(String.Format("Current Bar = {0}, Last Marker = {1}", timelineInfo.currentBar, (string)timelineInfo.lastMarker));
    }

    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    static FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr){
        FMOD.Studio.EventInstance instance=new FMOD.Studio.EventInstance(instancePtr);

        IntPtr timelineInfoPtr;
        FMOD.RESULT result=instance.getUserData(out timelineInfoPtr);
        if(result!=FMOD.RESULT.OK){
            Debug.LogError("Timeline Callback error: " + result);
        }else if (timelineInfoPtr != IntPtr.Zero){
            // Get the object to store beat and marker details
            GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
            TimelineInfo timelineInfo = (TimelineInfo)timelineHandle.Target;


            switch(type){
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                {
                    var parameter=(FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
                    timelineInfo.currentBar = parameter.bar;
                    timelineInfo.currentBeat = parameter.beat;
                    timelineInfo.beatPosition = parameter.position;
                    timelineInfo.currentTempo = parameter.tempo;
                    MusicBeat.newBeat=true;
                    break;
                }
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                {
                    var parameter = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));
                    timelineInfo.lastMarker = parameter.name;
                    break;
                }          
                case FMOD.Studio.EVENT_CALLBACK_TYPE.DESTROYED:
                {
                    // Now the event has been destroyed, unpin the timeline memory so it can be garbage collected
                    timelineHandle.Free();
                    break;
                }      
            }        
        }
        return FMOD.RESULT.OK;
    }
}
