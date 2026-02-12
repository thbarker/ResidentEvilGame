using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MansionDoor : Lockable
{
    public RecordsManager records;
    public WinManager winManager;
    public override void Use()
    {
        records = GameObject.Find("Records Manager")?.GetComponent<RecordsManager>();
        winManager = GameObject.Find("WinCanvas")?.GetComponent<WinManager>();
        records.Win();
        winManager.WinGame();
    }
}
