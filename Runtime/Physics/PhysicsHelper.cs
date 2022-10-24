using UnityEngine;

namespace BB.BardicFramework.Physics
{
    public static class PhysicsHelper
    {
    }

    public class SpeedOverTime
    {
        private float jerk, maxAccel, maxSpeed, speed, acceleration, deccel;

        private float lastChangeTime;
        private bool isAccelerating;

        public SpeedOverTime(float jerk, float maxSpeed, float maxAccel, float deccel)
        {
            this.acceleration = 0;
            this.speed = 0;
            this.lastChangeTime = 0;


            this.jerk = jerk;
            this.maxAccel = maxAccel;
            this.maxSpeed = maxSpeed;
            this.deccel = deccel;
        }

        public void Go(float initialJerk)
        {
            this.acceleration += initialJerk;
            isAccelerating = true;
            lastChangeTime = Time.time;
        }

        public void Stop()
        {
            GetSpeed();
            isAccelerating = false;
        }

        public float GetSpeed() => GetSpeed((Time.time - lastChangeTime));

        public float GetSpeed(float dt)
        {
            if(isAccelerating)
            {
                acceleration += jerk * dt;
                if (acceleration > maxAccel) acceleration = maxAccel;

                speed += acceleration * dt;
                if (speed > maxSpeed) speed = maxSpeed;
            }
            else
            {
                speed -= deccel * dt;
            }
            lastChangeTime = Time.time;
            return speed;
        }
    }
}