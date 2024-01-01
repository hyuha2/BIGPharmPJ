using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

struct PlayData
{
    public long companymoney;
    public int qcemployee;
    public int rdemployee;
    public int mfgemployee;
    public int ademployee;

    public PlayData(long companymoney, int qcemployee, int rdemployee, int mfgemployee, int ademployee)
    {
        this.companymoney = companymoney;
        this.qcemployee = qcemployee;
        this.rdemployee = rdemployee;
        this.mfgemployee = mfgemployee;
        this.ademployee = ademployee;
    }

}
