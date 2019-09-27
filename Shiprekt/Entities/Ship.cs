using System;
using System.Collections.Generic;
using System.Text;
using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using FlatRedBall.AI.Pathfinding;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Graphics.Particle;
using FlatRedBall.Math.Geometry;
using Microsoft.Xna.Framework;
using static Shiprekt.Entities.ShipSail;
using FlatRedBall.Debugging;
using Shiprekt.Utilities;
using Shiprekt.Factories;
using Shiprekt.Managers;
using Microsoft.Xna.Framework.Audio;

namespace Shiprekt.Entities
{
    public partial class Ship
    {
        #region Classes

        private class ShipInvulnPeriod
        {
            public Ship Ship;
            public float InvulnerableTimeLeft;
        }

        List<SoundEffect> shotSoundEffects = new List<SoundEffect>();

        #endregion

        #region Fields/Properties

        public bool AllowedToDrive
        {
            get
            {
                return IsAllowedToDrive;
            }
            set
            {
                IsAllowedToDrive = value;
            }
        }
        public int TeamIndex { get; private set; }

        IPressableInput shootLeftInput;
        IPressableInput shootRightInput;
        I1DInput sailTurningInput;

        DataTypes.RacingEntityValues EffectiveRacingEntityValues;
        DataTypes.RacingEntityValues BaseRacingEntityValues;

        List<ShipInvulnPeriod> shipInvulnList = new List<ShipInvulnPeriod>();
        float timeUntilNextShotAvailable;
        #endregion

        #region Events/Delegates

        public event Action<Bullet> BulletHit;
        public event Action BulletShot;

        #endregion

        #region Initialize

        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
        private void CustomInitialize()
        {
            this.ForwardDirection = RacingDirection.Right;
            timeUntilNextShotAvailable = 0;

            this.FireSmokeEmitterInstance.IsEmitting = false;

            InitializeMovementValues();

            Health = ShipEntityValuesInstance.MaxHealth;


#if DEBUG
            DoDebugInitialize();
#endif
        }

        private void DoDebugInitialize()
        {
            if (DebuggingVariables.ShowShipCollisions)
            {
                this.Collision.Visible = true;
            }
        }

        internal void SetSail(SailColor sailColor)
        {
            ShipSailInstance.CurrentSailColorState = sailColor;
        }

        private void InitializeMovementValues()
        {
            // Store off the base values, which are the values unmodified from the CSV. If the CSV reloads, this will get updated
            BaseRacingEntityValues = CarData;

            // Copy the values and store them in an "Effective" variable. Since this is a clone, these values
            // can be modified without changing the base CarData (or anything from the CSV). These values will
            // get modified at runtime according to wind and any other gameplay mechanics we want to add
            EffectiveRacingEntityValues = FlatRedBall.IO.FileManager.CloneObject(CarData);

            // Tell the game to use the effective values as its current values.
            CarData = EffectiveRacingEntityValues;
        }

        partial void CustomInitializeTopDownInput()
        {
            if (InputDevice is Xbox360GamePad gamePad)
            {
                shootLeftInput = gamePad.LeftTrigger
                    .Or(gamePad.GetButton(Xbox360GamePad.Button.LeftShoulder));
                shootRightInput = gamePad.RightTrigger
                    .Or(gamePad.GetButton(Xbox360GamePad.Button.RightShoulder));
                sailTurningInput = gamePad.RightStick.Horizontal;
                IPressableInput gas = gamePad.GetButton(Xbox360GamePad.Button.B);
                Gas = new DelegateBasedPressableInput(() => !gas.IsDown, () => gas.WasJustReleased, () => gas.WasJustPressed).To1DInput();
            }
            else if (InputDevice is Keyboard keyboard)
            {
                shootLeftInput = keyboard.GetKey(Microsoft.Xna.Framework.Input.Keys.Q)
                    .Or(InputManager.Mouse.GetButton(Mouse.MouseButtons.LeftButton));

                shootRightInput = keyboard.GetKey(Microsoft.Xna.Framework.Input.Keys.E)
                    .Or(InputManager.Mouse.GetButton(Mouse.MouseButtons.RightButton));
                sailTurningInput = InputManager.Keyboard.Get1DInput(Microsoft.Xna.Framework.Input.Keys.Left, Microsoft.Xna.Framework.Input.Keys.Right);
                IPressableInput gas = InputManager.Keyboard.GetKey(Microsoft.Xna.Framework.Input.Keys.Space);
                Gas = new DelegateBasedPressableInput(() => !gas.IsDown, () => gas.WasJustReleased, () => gas.WasJustPressed).To1DInput();
            }
        }

        public void SetTeam(int teamIndex)
        {
            TeamIndex = teamIndex;
        }

        #endregion

        #region Activity

        private void CustomActivity()
        {
            DoShootingActivity();
            DoSailTurningActivity();
            DoRamActivity();
            DoDebugActivity();
        }

        private void DoDebugActivity()
        {
            if (Entities.DebuggingVariables.IsShipMovementInfoDisplayed)
            {
                string debugString = $@"Velocity: {Velocity}
                    EffectiveForwardAcceleration: {EffectiveRacingEntityValues.ForwardAcceleration},
                    MaxSpeed: {EffectiveRacingEntityValues.EffectiveMaxSpeed}";

                FlatRedBall.Debugging.Debugger.Write(debugString);
            }
        }

        private void DoShootingActivity()
        {
            timeUntilNextShotAvailable -= TimeManager.SecondDifference;

            if (timeUntilNextShotAvailable <= 0)
            {
                if (shootLeftInput.WasJustPressed)
                {
                    Shoot(this.Left.ToVector2());
                }
                else if (shootRightInput.WasJustPressed)
                {
                    Shoot(this.Right.ToVector2());
                }
            }
        }

        internal void Shoot(Vector2 bulletDirection)
        {
            var bullet = Factories.BulletFactory.CreateNew(this.X, this.Y);

            bullet.Velocity = (bulletDirection * Bullet.BulletSpeed).ToVector3();
            bullet.TeamIndex = this.TeamIndex;
            bullet.Owner = this;

            SetFutureBulletDestroyLogic(bullet);

            PlayShotSound();

            timeUntilNextShotAvailable = SecondsBetweenShotsMin;

            BulletShot();
        }

        private void SetFutureBulletDestroyLogic(Bullet bullet)
        {
            var bulletDuration = Bullet.BulletDistance / Bullet.BulletSpeed;

            bullet.Call(() =>
            {
                BulletHit(bullet);
            })
            .After(bulletDuration);


        }

        private void PlayShotSound()
        {
            switch(FlatRedBallServices.Random.Next(3))
            {
                case 0: cannon01.Play(); break;
                case 1: cannon02.Play(); break;
                case 2: cannon03.Play(); break;
            }
        }

        private void DoSailTurningActivity()
        {
            var turnSpeed = sailTurningInput.Value * -ShipEntityValuesInstance.SailRotationSpeed;
            var left = ShipEntityValuesInstance.MaxSailRotation * ((float)Math.PI / 180);
            var right = 360 * ((float)Math.PI / 180) - ShipEntityValuesInstance.MaxSailRotation * ((float)Math.PI / 180);
            var middle = (float)Math.PI;

            var maxLeftTurningRight = ShipSailInstance.RelativeRotationZ == left && turnSpeed < 0;
            var maxRightTurningLeft = ShipSailInstance.RelativeRotationZ == right && turnSpeed > 0;
            var safeZone = ShipSailInstance.RelativeRotationZ < left || ShipSailInstance.RelativeRotationZ > right;
            if (maxLeftTurningRight || maxRightTurningLeft || safeZone)
            {
                ShipSailInstance.RelativeRotationZVelocity = turnSpeed;
            }
            else
            {
                if (ShipSailInstance.RelativeRotationZ > left && ShipSailInstance.RelativeRotationZ < middle)
                {
                    ShipSailInstance.RelativeRotationZ = left;
                }
                else if (ShipSailInstance.RelativeRotationZ < right && ShipSailInstance.RelativeRotationZ > middle)
                {
                    ShipSailInstance.RelativeRotationZ = right;
                }
                ShipSailInstance.RelativeRotationZVelocity = 0;
            }
        }

        internal void TakeDamage(int damageAmount, Ship whoDealtDamage)
        {
            if(Health > 0)
            {
                Health -= damageAmount;
                if (Health <= 0)
                {
                    FireSmokeEmitterInstance.IsEmitting = false;
                    JoinedPlayerManager.AwardKill(whoDealtDamage.InputDevice);
                    JoinedPlayerManager.RecordDeath(this.InputDevice);
                    Die();
                }
                else
                {
                    FireSmokeEmitterInstance.IsEmitting = Health <= 21 ;

                    // didn't die, so just play a sound:
                    var endingInt = FlatRedBallServices.Random.Next(3) + 1;

                    var hitSound = (SoundEffect)GetFile("shipimpact0" + endingInt);

                    hitSound.Play();
                }
            }
        }

        internal void ResetHealth()
        {
            Health = ShipEntityValuesInstance.MaxHealth;
        }

        public void ApplyWind(Vector2 windDirectionNormalized)
        {
            // Update the max speed according to the wind          
            var sailToWindDot = Vector2.Dot(ShipSailInstance.RotationMatrix.Right.ToVector2(), windDirectionNormalized);
            var coefficient = sailToWindDot == 0 ? 0 : (sailToWindDot /= 2) + .5f;

#if DEBUG
            if (DebuggingVariables.FastMovement)
            {
                EffectiveRacingEntityValues.EffectiveMaxSpeed = BaseRacingEntityValues.EffectiveMaxSpeed;
            }
            else
#endif
            {
                EffectiveRacingEntityValues.EffectiveMaxSpeed = Math.Max(MinSpeed, BaseRacingEntityValues.EffectiveMaxSpeed * coefficient);
            }

            //Change the sail visual. 
            ShipSailInstance.UpdateSailVisual(windDirectionNormalized);
        }

        private void DoRamActivity()
        {
            for (int i = shipInvulnList.Count - 1; i >= 0; i--)
            {
                shipInvulnList[i].InvulnerableTimeLeft -= TimeManager.SecondDifference;
                if (shipInvulnList[i].InvulnerableTimeLeft <= 0) shipInvulnList.RemoveAt(i);
            }
        }
        #endregion

        #region Destroy

        private void CustomDestroy()
        {


        }

        internal void Die()
        {
            AfterDying(this);
        }

        #endregion

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }

        public bool CanRamShip(Ship ship)
        {
            if (ship == this) return false;
            if (Velocity.Length() < this.MinimumRamSpeed) return false;
            foreach (var pair in shipInvulnList)
            {
                if (pair.Ship == ship) return false;
            }

            var shipToShipDot = Vector2.Dot(RotationMatrix.Right.ToVector2(), ship.RotationMatrix.Right.ToVector2());
            var deg = Math.Abs(shipToShipDot * 180);

            // This should be controlled by a debugging variable so it can be easily
            // toggled without having to search in code.
            //Debugger.CommandLineWrite($"Deg: {deg}");
            //We will detect how big a ram is based on the shipToShipDot's difference from 0. 
            if (deg < RamInstance.GlancingRamAngle / 2) return true;
            else return false;
        }

        public int GetRamShipDmg(Ship ship)
        {
            var shipToShipDot = Vector2.Dot(RotationMatrix.Right.ToVector2(), ship.RotationMatrix.Right.ToVector2());
            var deg = Math.Abs(shipToShipDot * 180);

            if (deg < RamInstance.HeavyRamAngle)
            {
                Debugger.CommandLineWrite("Heavy Ram");
                return RamInstance.HeavyRamDamage;
            }
            else if (deg < RamInstance.MediumRamAngle)
            {
                Debugger.CommandLineWrite("Heavy Ram");
                return RamInstance.MediumRamDamage;
            }
            else if (deg < RamInstance.GlancingRamAngle)
            {
                Debugger.CommandLineWrite("Heavy Ram");
                return RamInstance.GlancingRamDamage;
            }
            else
            {
                return 0;
            }
        }

        public void MarkShipRammed(Ship ship)
        {
            shipInvulnList.Add(new ShipInvulnPeriod() { Ship = ship, InvulnerableTimeLeft = RamCooldown });
        }

    }

    #region Extension Methods

    public static class Vector3Extensions
    {
        public static Vector3 Normalized(this Vector3 sender)
        {
            Vector3 normalized = sender;
            normalized.Normalize();
            return normalized;
        }
    }

    public static class Vector2Extensions
    {
        public static Vector2 Normalized(this Vector2 sender)
        {
            Vector2 normalized = sender;
            normalized.Normalize();
            return normalized;
        }
    }

    #endregion
}
