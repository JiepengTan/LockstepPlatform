using Lockstep.ECS.ECDefine;

namespace  Lockstep.ECS.ECDefine.UnsafeECS {
    public partial class SignalDefine {
        //a public partial class SignalDefine{ [Signal]void  to be activated whenever any damage is dealt (players attacking enemies, vice versa) }
        //The CharacterStatusSystem handles that activation so the DamageStructure
        //can be used to effectively cause the damage
        [Signal]
        void OnDamage(DamageStructure dmg){ }

        //who has cast the projectile (so it doesn't deal damage to itself), forward direction,
        //angle of the projectile and the SpellType, so the correct projectile can be created (the simple one, or the special one)
        [Signal]
        void OnCastProjectile(EntityRef projectileSource, Vector2 forward, Vector2 right, float
            projectileAngle, EProjectileType projectileType, ProjectileSpec projectileSpec){ }

        [Signal]
        void collision(ref Projectile projectile, ref Goblin goblin){ }

        [Signal]
        void collision(ref Projectile projectile, ref Enemy enemy){ }

        [Signal]
        void collision(ref Projectile projectile){ }
    }
}