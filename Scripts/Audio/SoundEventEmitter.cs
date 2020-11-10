using FMOD.Studio;
using System.Linq;
using UnityEngine;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public enum SoundEventEmitterMessage
    {
        None,
        Awake,
        Start,
        OnDestroy,
        OnCollisionEnter2D,
        OnCollisionExit2D,
        OnTriggerEnter2D,
        OnTriggerExit2D,
        OnReceivingBullet,
        OnPickup,
        OnSubmit,
        OnCancel,
        OnGotFocus,
        OnLostFocus,
        OnNavigate,
    }

    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(SoundEmitter))]
    public class SoundEventEmitter : MonoBehaviour
    {
        #region Messages

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            DoCheck(SoundEventEmitterMessage.Awake);
        }

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            DoCheck(SoundEventEmitterMessage.Start);
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnDestroy()
        {
            DoCheck(SoundEventEmitterMessage.OnDestroy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter2D(Collision2D collision)
        {
            DoCheck(SoundEventEmitterMessage.OnCollisionEnter2D);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionExit2D(Collision2D collision)
        {
            DoCheck(SoundEventEmitterMessage.OnCollisionExit2D);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            DoCheck(SoundEventEmitterMessage.OnTriggerEnter2D);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerExit2D(Collider2D collision)
        {
            DoCheck(SoundEventEmitterMessage.OnTriggerExit2D);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        private void OnReceivingBullet(BulletResponse response)
        {
            DoCheck(SoundEventEmitterMessage.OnReceivingBullet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        private void OnPickup(PickupResponse response)
        {
            DoCheck(SoundEventEmitterMessage.OnPickup);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void OnSubmit(UINavigationData data)
        {
            DoCheck(SoundEventEmitterMessage.OnSubmit);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void OnCancel(UINavigationData data)
        {
            DoCheck(SoundEventEmitterMessage.OnCancel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="navigable"></param>
        private void OnGotFocus(UINavigable navigable)
        {
            DoCheck(SoundEventEmitterMessage.OnGotFocus);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="navigable"></param>
        private void OnLostFocus(UINavigable navigable)
        {
            DoCheck(SoundEventEmitterMessage.OnLostFocus);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void OnNavigate(UINavigationData data)
        {
            DoCheck(SoundEventEmitterMessage.OnNavigate);
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public void ForceStop()
        {
            foreach (var eventInstance in _eventInstances)
            {
                eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        private void DoCheck(SoundEventEmitterMessage message)
        {
            CheckPlay(message);
            CheckStop(message);
        }

        /// <summary>
        /// 
        /// </summary>
        private void CheckPlay(SoundEventEmitterMessage message)
        {
            if (message == _playAt)
            {
                if (_stopAtReplay)
                {
                    ForceStop();
                }
                _eventInstances = this.EmitSound(_soundKey, _mandatory).ToArray();
            }
        }

        private void CheckStop(SoundEventEmitterMessage message)
        {
            if (message == _stopAt)
            {
                foreach (var eventInstance in _eventInstances)
                {
                    eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                }
            }
        }

        #endregion

        #region Fields

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SoundEventEmitterMessage _playAt = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private SoundEventEmitterMessage _stopAt = default;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private string _soundKey = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private bool _mandatory = true;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private bool _stopAtReplay = false;

        private EventInstance[] _eventInstances = new EventInstance[0];

        #endregion
    }
}