using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GameFabric.Processors
{
    public enum RequestProcessorEnum:int
    {
        //Reserved calls
        _System_PingGw =0,
        _System_Authenticate=1,
        _System_MapConnection=2,

        //CommonCalls
        Session =1000,
        UserExists =1001,
        CreateUser =1002,
        LoginUser =1003,
        SendMessage=1004,
        //Tanks commands
        TankPosition = 1005,
        FireShell = 1006,
        TakeDamage = 1007,
        JoinOrCreateGame = 1008,
        StartRound = 1009,
        BeginRound = 1010,
        GetCanStartRound = 1011
    };
}
