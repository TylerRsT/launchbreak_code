using Rewired;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjectShovel
{
    /// <summary>
    /// 
    /// </summary>
    public static class InputHelper
    {
        #region Const

        private const string GameplayHorizontalMoveInputName = "Gameplay_HorizontalMove";
        private const string GameplayVerticalMoveInputName = "Gameplay_VerticalMove";

        private const string GameplayLeftMoveInputName = "Gameplay_LeftMove";
        private const string GameplayRightMoveInputName = "Gameplay_RightMove";
        private const string GameplayUpMoveInputName = "Gameplay_UpMove";
        private const string GameplayDownMoveInputName = "Gameplay_DownMove";

        private const string UIHorizontalInputName = "UI_Horizontal";
        private const string UIVerticalInputName = "UI_Vertical";

        private const string UILeftInputName = "UI_Left";
        private const string UIRightInputName = "UI_Right";
        private const string UIUpInputName = "UI_Up";
        private const string UIDownInputName = "UI_Down";

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputName"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool AnyButtonDown(string inputName, out int index)
        {
            index = -1;
            foreach(var player in ReInput.players.Players)
            {
                if (player.GetButtonDown(inputName))
                {
                    index = player.id;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputName"></param>
        /// <returns></returns>
        public static bool AnyButtonDown(string inputName)
        {
            int index;
            return AnyButtonDown(inputName, out index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputName"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool AnyButtonUp(string inputName, out int index)
        {
            index = -1;
            foreach(var player in ReInput.players.Players)
            {
                if (player.GetButtonUp(inputName))
                {
                    index = player.id;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputName"></param>
        /// <returns></returns>
        public static bool AnyButtonUp(string inputName)
        {
            int index;
            return AnyButtonUp(inputName, out index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputName"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool AnyButton(string inputName, out int index)
        {
            index = -1;
            foreach (var player in ReInput.players.Players)
            {
                if (player.GetButton(inputName))
                {
                    index = player.id;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputName"></param>
        /// <returns></returns>
        public static bool AnyButton(string inputName)
        {
            int index;
            return AnyButton(inputName, out index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputName"></param>
        /// <returns></returns>
        public static float AnyAxis(string inputName)
        {
            var axis = 0.0f;
            foreach (var player in ReInput.players.Players)
            {
                axis += player.GetAxis(inputName);
            }
            return axis;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static float GetGameplayHorizontalAxis(int index)
        {
            var axis = 0.0f;
            var playerController = ReInput.players.GetPlayer(index);
            if (playerController.GetButton(GameplayLeftMoveInputName))
            {
                axis -= 1.0f;
            }
            if (playerController.GetButton(GameplayRightMoveInputName))
            {
                axis += 1.0f;
            }

            if(axis == 0.0f)
            {
                axis = playerController.GetAxis(GameplayHorizontalMoveInputName);
            }

            return axis;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public static float GetUIHorizontalAxis(int index)
        {
            var axis = 0.0f;
            var playerController = ReInput.players.GetPlayer(index);
            if (playerController.GetButton(UILeftInputName))
            {
                axis -= 1.0f;
            }
            if (playerController.GetButton(UIRightInputName))
            {
                axis += 1.0f;
            }

            if(axis == 0.0f)
            {
                axis = playerController.GetAxis(UIHorizontalInputName);
            }

            return axis;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static float GetAnyUIHorizontalAxis()
        {
            var axis = 0.0f;
            for(var i = 0; i <= GameConstants.MaxPlayerCount; ++i)
            {
                axis += GetUIHorizontalAxis(i);
            }
            return axis;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static float GetGameplayVerticalAxis(int index)
        {
            var axis = 0.0f;
            var playerController = ReInput.players.GetPlayer(index);
            if (playerController.GetButton(GameplayDownMoveInputName))
            {
                axis -= 1.0f;
            }
            if (playerController.GetButton(GameplayUpMoveInputName))
            {
                axis += 1.0f;
            }

            if(axis == 0.0f)
            {
                axis = playerController.GetAxis(GameplayVerticalMoveInputName);
            }

            return axis;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public static float GetUIVerticalAxis(int index)
        {
            var axis = 0.0f;
            var playerController = ReInput.players.GetPlayer(index);
            if (playerController.GetButton(UIDownInputName))
            {
                axis -= 1.0f;
            }
            if (playerController.GetButton(UIUpInputName))
            {
                axis += 1.0f;
            }

            if(axis == 0.0f)
            {
                axis = playerController.GetAxis(UIVerticalInputName);
            }

            return axis;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static float GetAnyUIVerticalAxis()
        {
            var axis = 0.0f;
            for (var i = 0; i <= GameConstants.MaxPlayerCount; ++i)
            {
                axis += GetUIVerticalAxis(i);
            }
            return axis;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="percentage"></param>
        public static void SetAllVibrations(float percentage)
        {
            for (int i = 0; i < GameConstants.MaxPlayerCount; i++)
            {
                GameModeParams.instance.playerParams[i].vibrationMultiplier = percentage;

                Rewired.ReInput.players.Players[i+1]?.SetVibration(0, percentage);
                Rewired.ReInput.players.Players[i+1]?.SetVibration(1, percentage);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userIndex"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static void SetVibration(int userIndex, float left, float right)
        {
            if(!ReInput.isReady)
            {
                return;
            }

            Rewired.ReInput.players?.Players[userIndex]?.SetVibration(0, left);
            Rewired.ReInput.players?.Players[userIndex]?.SetVibration(1, right);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void StopAllVibrations()
        {
            if (!ReInput.isReady)
            {
                return;
            }

            for (int i = 0; i < GameConstants.MaxPlayerCount; i++)
            {
                Rewired.ReInput.players?.Players[i+1]?.StopVibration();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userIndex"></param>
        public static void StopVibration(int userIndex)
        {
            if (!ReInput.isReady)
            {
                return;
            }

            Rewired.ReInput.players?.Players[userIndex]?.StopVibration();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="behaviour"></param>
        /// <param name="userIndex"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static Coroutine StartVibrationCoroutine(this MonoBehaviour behaviour, int userIndex, float left, float right, float duration)
        {
            SetVibration(userIndex, left, right);
            IEnumerator stopper()
            {
                yield return new WaitForSecondsRealtime(duration);
                StopVibration(userIndex);
            };
            return behaviour.StartCoroutine(stopper());
        }

        #endregion
    }
}
