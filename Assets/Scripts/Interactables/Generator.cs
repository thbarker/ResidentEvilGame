using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class Generator : Lockable
{
    public ElectronicallyLockedSafe safe;
    public AudioSource startupSource;
    public AudioSource loopSource;

    public override void Use()
    {
        safe.Unlock(); 
        if(locked){
            StartCoroutine(PlayAudio());
        }
        messageHandler.QueueMessage("You have turned on the power.");
    }

    IEnumerator PlayAudio(){
        startupSource.PlayOneShot(startupSource.clip);
        yield return new WaitForSeconds(startupSource.clip.length);
        loopSource.Play();
    }
}
