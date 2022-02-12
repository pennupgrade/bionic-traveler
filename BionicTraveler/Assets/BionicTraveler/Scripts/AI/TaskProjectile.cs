namespace BionicTraveler.Scripts.AI
{
    using BionicTraveler.Scripts.World;
    using Framework;

    /// <summary>
    /// Please document me.
    /// </summary>
    public class TaskProjectile : EntityTask
    {
        private readonly Entity target;
        public override EntityTaskType Type => EntityTaskType.Projectile;

        public TaskProjectile(DynamicEntity owner, Entity target):
            base(owner)
        {
            this.target = target;
        }

        public override void OnProcess()
        {
            
        }

        
    }
}
