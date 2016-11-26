namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicArrive : DynamicVelocityMatch
    {
        public override string Name
        {
            get { return "Arrive"; }
        }

        public float MaxSpeed { get; set; }

        public float StopRadius { get; set; }

        public float SlowRadius { get; set; }

        public DynamicArrive()
        {
            this.MovingTarget = new KinematicData();
            MaxSpeed = 20.0f;
            StopRadius = 0.1f;
            SlowRadius = 5.0f;
        }

        public override MovementOutput GetMovement()
        {
            //var output = new MovementOutput();
            var direction = this.Target.position - this.Character.position;
            var distance = direction.magnitude;
            var targetSpeed = 0.0f;

            if (distance < StopRadius)
            {
                targetSpeed = 0.0f;
            }
            else if (distance > SlowRadius)
            {
                targetSpeed = MaxSpeed;
            }
            else
            {
                targetSpeed = MaxSpeed * (distance / SlowRadius);
            }

            this.MovingTarget.velocity = direction.normalized * targetSpeed;


            return base.GetMovement();
        }

    }
}


