using System;

namespace Events
{
    /// <summary>
    /// Central event system for discrete inputs
    /// </summary>
    public class InputEvent
    {
        public InputEventContext Context { get; private set; } = InputEventContext.Default;
        
        #region Events
        
        // Movement
        public event Action JumpStarted;
        public event Action JumpCanceled;
        public event Action CrouchStarted;
        public event Action CrouchCanceled;
        public event Action SprintStarted;
        public event Action SprintCanceled;
        
        // Combat
        public event Action ToggleWeaponPerformed;
        public event Action<AttackType> AttackPerformed;
        public event Action BlockStarted;
        public event Action BlockCanceled;
        
        // Interaction
        
        
        // UI
        public event Action<int> HotbarSelected;
        public event Action DisplayInventoryUIPerformed;
        public event Action DisplayQuestLogUIPerformed;
        public event Action DisplayPauseMenuPerformed;
        public event Action CloseActiveUI;
        public event Action<InputEventContext> ConfirmPerformed;
        public event Action DropItemPerformed;
        
        #endregion

        public void ChangeContext(InputEventContext context)
        {
            Context = context;
        }
        
        #region Event invocation methods
        
        // Movement
        public void InvokeJumpStarted() => JumpStarted?.Invoke();
        public void InvokeJumpCanceled() => JumpCanceled?.Invoke();
        public void InvokeCrouchStarted() => CrouchStarted?.Invoke();
        public void InvokeCrouchCanceled() => CrouchCanceled?.Invoke();
        public void InvokeSprintStarted() => SprintStarted?.Invoke();
        public void InvokeSprintCanceled() => SprintCanceled?.Invoke();
        
        // Combat
        public void InvokeToggleWeaponPerformed() => ToggleWeaponPerformed?.Invoke();
        public void InvokeAttackPerformed(AttackType type) => AttackPerformed?.Invoke(type);
        public void InvokeBlockStarted() => BlockStarted?.Invoke();
        public void InvokeBlockCanceled() => BlockCanceled?.Invoke();
        
        // Interaction
        
        
        // UI
        public void InvokeHotbarSelected(int index) => HotbarSelected?.Invoke(index);
        public void InvokeDisplayPauseMenuPerformed() => DisplayPauseMenuPerformed?.Invoke();
        public void InvokeCloseActiveUI() => CloseActiveUI?.Invoke();
        public void InvokeConfirmPerformed() => ConfirmPerformed?.Invoke(Context);
        public void InvokeDisplayQuestLogUIPerformed() => DisplayQuestLogUIPerformed?.Invoke();
        public void InvokeDropItemPerformed() => DropItemPerformed?.Invoke();
        
        // General
        public void InvokeDisplayInventoryUIPerformed() => DisplayInventoryUIPerformed?.Invoke();
        
        #endregion
    }
}