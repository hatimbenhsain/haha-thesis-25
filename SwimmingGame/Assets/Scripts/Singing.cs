using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class Singing : Sound
{
    [Tooltip("Name of folder inside of FMOD events (Swimmer, NPC1, etc).")]
    public string name="NPC1";
    [Tooltip("If true, sing a single voice event as opposed to different events depending on notes.")]
    public bool singleVoiceMode=false;
    private string voiceLabel="";
    [HideInInspector]
    public EventInstance voice;
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
    
        if(singleVoiceMode){
            voice=FMODUnity.RuntimeManager.CreateInstance("event:/Singing/"+name);
        }else{
            events=new Dictionary<string,EventInstance>();
            foreach(string key in keys){
                EventInstance instance=FMODUnity.RuntimeManager.CreateInstance("event:/Singing/"+name+"/"+key.ToUpper());
                events.Add(key,instance);
            }
        }
        
    }

    public void SingingUpdate()
    {
        if(singingNote!=""){
            SetVolume(singingNote,singingVolume*maxSingingVolume);
        }
    }

    
    public void StopAllNotes(){
        if(singleVoiceMode){
            StopNote("");
        }else{        
            foreach(var note in possibleNotes){
                StopNote(note);
            }
        }
    }

    public void PlayNote(string note){
        if(singleVoiceMode){
            if(!IsPlaying(note)) voice.start();
            voiceLabel=note;
            voice.setParameterByNameWithLabel("Note",voiceLabel.Substring(0,voiceLabel.Length-1));
        }
        else{
            EventInstance instance=events[note];
            instance.start();
        }
    }

    public void SetVolume(string note,float v){
        EventInstance instance;
        if(!singleVoiceMode) instance=events[note];
        else instance=voice;
        instance.setParameterByName("Volume",v);
    }

    public void StopNote(string note){
        if(singleVoiceMode && (note=="" || possibleNotes.Contains(note))){
            voice.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }else if(!singleVoiceMode && events.ContainsKey(note)){
            EventInstance instance=events[note];
            instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    public void ToggleNote(string note){
        if(!IsPlaying(note)){
            PlayNote(note);
        }else{
            StopNote(note);
        }
    }

    public bool IsPlaying(string note){
        EventInstance instance;
        if(singleVoiceMode) instance=voice;
        else instance=events[note];
        PLAYBACK_STATE state;
        instance.getPlaybackState(out state);
        if(state==PLAYBACK_STATE.STOPPED){
            return false;
        }else{
            if(!singleVoiceMode || voiceLabel==note) return true;
            else return false;
        }
    }

    private void OnDestroy() {
        if(events!=null && events.Count>0){
            foreach(var e in events.Values){
                e.release();
            }
        }
        if(singleVoiceMode) voice.release();
    }

    private void OnDisable() {
        StopAllNotes();
    }
}

[System.Serializable]
public struct MusicalEvent{
    public string musicNote;
    public float length;
}