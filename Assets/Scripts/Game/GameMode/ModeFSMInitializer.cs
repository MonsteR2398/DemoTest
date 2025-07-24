using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeFSMInitializer : MonoBehaviour
{
    public ModeFSM CreateFsm(ModeContext context)
    {
        var fsm = new ModeFSM();
        // Инициализация состояний с конфигами
        fsm.AddState(new MeteorModeState(fsm, context));
        fsm.AddState(new FloodModeState(fsm, context));
        fsm.AddState(new TornadoModeState(fsm, context));
        
        return fsm;
    }
}
public class ModeContext
{
    public ModeController ModeController { get; }
    public ModeContext(ModeController modeController)
    {
        ModeController = modeController;
    }
}
