using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MigrationScreenBorders : ScreenBorders
{
    private Migration migration;

    public int phase=0;


    protected override void Start()
    {
        base.Start();
        migration=FindObjectOfType<Migration>();
    }

    protected virtual void Update()
    {
        base.Update();

        phase=Mathf.FloorToInt(8*(migration.progress/migration.colors.Length));

        if(phase>=1){
            ActivateBorder("migration 1",true);
        }
        if(phase>=2){
            ActivateBorder("migration 2",true);
        }
        if(phase>=3){
            ActivateBorder("migration 2",false);
        }
        if(phase>=4){
            ActivateBorder("migration 3",true);
        }
        if(phase>=5){
            ActivateBorder("migration 3",false);
        }
        if(phase>=6){
            ActivateBorder("migration 1",false);
        }

    }
}
