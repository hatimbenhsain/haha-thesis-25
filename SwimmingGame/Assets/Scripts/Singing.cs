using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class Singing : MonoBehaviour
{
    [Tooltip("Name of folder inside of FMOD events (Swimmer, NPC1, etc).")]
    public string name="NPC1";
    public Dictionary<string,EventInstance> events;
    [Tooltip("Keys that player can play.")]
    public List<string> keys;
    [Tooltip("Currently playable notes to load at the start.")]
    public List<string> possibleNotes;
    [Tooltip("Current note being sung.")]
    public string singingNote;
    public float singingVolume;
    public float maxSingingVolume=1f;
    public void SingingStart()
    {
        for(int i=0;i<possibleNotes.Count;i++){
            if(!keys.Contains(possibleNotes[i])){
                keys.Add(possibleNotes[i]);
            }
        }
        events=new Dictionary<string,EventInstance>();
        foreach(string key in keys){
            EventInstance instance=FMODUnity.RuntimeManager.CreateInstance("event:/Singing/"+name+"/"+key.ToUpper());
            events.Add(key,instance);
        }
    }

    public void SingingUpdate()
    {
        if(singingNote!=""){
            SetVolume(singingNote,singingVolume*maxSingingVolume);
        }
    }

    
    public void StopAllNotes(){
        foreach(var note in possibleNotes){
            StopNote(note);
        }
    }

    public void PlayNote(string note){
        EventInstance instance=events[note];
        instance.start();
    }

    public void SetVolume(string note,float v){
        EventInstance instance=events[note];
        instance.setParameterByName("Volume",v);
    }

    public void StopNote(string note){
        EventInstance instance=events[note];
        instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void ToggleNote(string note){
        if(!IsPlaying(note)){
            PlayNote(note);
        }else{
            StopNote(note);
        }
    }

    public bool IsPlaying(string note){
        EventInstance instance=events[note];
        PLAYBACK_STATE state;
        instance.getPlaybackState(out state);
        if(state==PLAYBACK_STATE.STOPPED){
            return false;
        }else{
            return true;
        }
    }

    private void OnDestroy() {
        foreach(var e in events.Values){
            e.release();
        }
    }
}

[System.Serializable]
public struct MusicalEvent{
    public string musicNote;
    public float length;
}