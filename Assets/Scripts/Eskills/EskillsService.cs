﻿using System;
using Eskills.Scripts;
using Eskills.Service;
using Eskills.Service.Repository;
using Eskills.Ui;
using UnityEngine;

namespace Eskills
{
    public class EskillsService : MonoBehaviour, ICoroutineExecutor
    {
        private readonly string _privateKey = Config.GetPrivateKey();
        private EskillsManager _eskillsManager;
        [SerializeField] private UserNameProvider _userNameProvider;


        void Awake()
        {
            var roomRepository = new RoomRepository(new RoomResponseMapper());
            var ticketRepository = new TicketRepository(_privateKey, this);
            var getRoomInfoUseCase = new GetRoomInfoUseCase(roomRepository, this);
            var setScoreUseCase = new SetScoreUseCase(roomRepository, this);
            var getPeriodicUpdateUseCase = new GetPeriodicUpdateUseCase(roomRepository, this);
            var createRoomUseCase = new CreateRoomUseCase(this, ticketRepository, _privateKey, roomRepository);
            _eskillsManager = new EskillsManager(new PurchaseActivity(), getRoomInfoUseCase, setScoreUseCase,
                getPeriodicUpdateUseCase, createRoomUseCase);
        }


        public void StartPurchase(MatchParameters matchParameters)
        {
            string userName;
            if (_userNameProvider == null)
            {
                userName = "";
                Debug.LogWarning("You should provide an implementation of " + nameof(UserNameProvider) +
                                 " before calling StartPurchase method. An empty user name was used");
            }
            else
            {
                userName = _userNameProvider.GetUserName();
            }

            if (Application.isEditor)
            {
                _eskillsManager.CreateRoom(userName, matchParameters.value, matchParameters.currency,
                    matchParameters.product, matchParameters.timeout, matchParameters.matchEnvironment,
                    matchParameters.numberOfPlayers,
                    session => GetComponent<OnMatchCreatedReceiver>().OnMatchCreated(session));
            }

            if (Application.platform != RuntimePlatform.Android) return;
            _eskillsManager.StartPurchase(userName, matchParameters.value, matchParameters.currency,
                matchParameters.product, matchParameters.timeout, matchParameters.matchEnvironment,
                matchParameters.numberOfPlayers);
        }


        public void GetRoomInfo(string session, Action<RoomData> success, Action<EskillsError> error)
        {
            _eskillsManager.GetRoomInfo(session, success, error);
        }

        public void SetScore(string session, SetScoreBody.Status status, int score, Action<RoomData> success = null,
            Action<EskillsError> error = null)
        {
            _eskillsManager.SetScore(session, status, score, success, error);
        }

        public void GetPeriodicUpdate(string session, Action<RoomData> success, Action<EskillsError> error)
        {
            _eskillsManager.GetPeriodicUpdate(session, success, error);
        }

        public void StopPeriodicUpdate()
        {
            _eskillsManager.StopPeriodicUpdate();
        }
    }
}