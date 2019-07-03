//who took damage (for its HP to be decreased), who did the damage (so if it is a player that killed the target, increase his score)
//and the ammount of damage dealt

using Lockstep.ECS.ECDefine;

public class DamageStructure : IComponent {
    public EntityRef Target;
    public EntityRef CharacterAttacker;
    public float Value;
}

//activate damage VFX/SFX on the position of the Character that has been damaged
public class CharacterDamage : IEvent {
    EntityRef Character;
}

//activate death VFX/SFX on the position of the Character that has died
public class CharacterDead : IEvent {
    EntityRef Character;
}

//a public partial class SignalDefine{ [Signal]void  to be activated whenever any damage is dealt (players attacking enemies, vice versa) }
//The CharacterStatusSystem handles that activation so the DamageStructure
//can be used to effectively cause the damage
public partial class SignalDefine {
    [Signal]
    void OnDamage(DamageStructure dmg){ }
}