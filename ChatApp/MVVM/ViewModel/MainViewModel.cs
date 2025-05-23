﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ChatClient.MVVM.Core;
using ChatClient.MVVM.Model;
using ChatClient.Net;

namespace ChatClient.MVVM.ViewModel
{
    class MainViewModel
    {

        //For some reason send message stopped working and doesnt work

        public  ObservableCollection<UserModel>  Users { get; set; }

        public ObservableCollection<String> Messages { get; set; }
        public RelayCommand ConnectToServerCommand { get; set; }
        public RelayCommand SendMessageCommand { get; set; }
        public string Username { get; set; }
        public string Message { get; set; }


        private Server _server;
        public MainViewModel()

        {
            Users = new ObservableCollection<UserModel>();
            Messages = new ObservableCollection<String>();
            _server = new Server();
            _server.connectedEvent += UserConnected;
            _server.msgrecievedEvent += MessageRecieved;
            _server.userdisconnectevent += RemoveUser;
            ConnectToServerCommand = new RelayCommand(o => _server.ConnectToServer(Username), o => !string.IsNullOrEmpty(Username));

            SendMessageCommand = new RelayCommand(o =>
            {
                _server.SendMessageToServer(Message);
                Message = string.Empty;
            }, o => !string.IsNullOrEmpty(Message));


        }

        private void RemoveUser()
        {
            var uid = _server.PacketReader.ReadMessage();
            var user = Users.Where(x => x.UID == uid).FirstOrDefault();
            Application.Current.Dispatcher.Invoke(() => Users.Remove(user));
        }

        private void MessageRecieved()
        {
            var msg = _server.PacketReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() =>
            {
                Messages.Add(msg);
            });
        }

        private  void UserConnected()
        {
            var user = new UserModel
            {
                Username = _server.PacketReader.ReadMessage(),
                UID = _server.PacketReader.ReadMessage()
            };

            if (!Users.Any(x => x.UID == user.UID))
            {

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Users.Add(user);
                });
            }
        }
    }
}
