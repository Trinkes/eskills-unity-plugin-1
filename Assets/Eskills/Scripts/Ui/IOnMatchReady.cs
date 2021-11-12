using System;
using UnityEngine;

namespace Eskills.Scripts.Ui
{
    [Serializable]
    public abstract class OnMatchReadyResult : MonoBehaviour
    {
        public abstract void OnMatchReady(string session);
    }
}