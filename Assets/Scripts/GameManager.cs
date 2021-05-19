using System;
using System.Collections.Generic;
using Syn.Bot.Oscova;
using Syn.Bot.Oscova.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace Assets
{
    public class Message
    {
        public string Text;
        public Text TextObject;
        public MessageType MessageType;
    }

    public enum MessageType
    {
        User, Bot
    }

    public class GameManager : MonoBehaviour
    {
        OscovaBot MainBot;

        public GameObject chatPanel, textObject;
        public InputField chatBox;

        public Color UserColor, BotColor;

        List<Message> Messages = new List<Message>();

        private ChatbotPC bot;

        // Start is called before the first frame update
        void Start()
        {
            bot = new ChatbotPC();
            bot.LoadBrain();
        }

        public void AddMessage(string messageText, MessageType messageType)
        {
            if (Messages.Count >= 25)
            {
                //Remove when too much.
                Destroy(Messages[0].TextObject.gameObject);
                Messages.Remove(Messages[0]);
            }

            var newMessage = new Message { Text = messageText };

            var newText = Instantiate(textObject, chatPanel.transform);

            newMessage.TextObject = newText.GetComponent<Text>();
            newMessage.TextObject.text = messageText;
            newMessage.TextObject.color = messageType == MessageType.User ? UserColor : BotColor;

            Messages.Add(newMessage);
        }

        public void SendMessageToBot()
        {
            var userMessage = chatBox.text;

            if (!string.IsNullOrEmpty(userMessage))
            {
                Debug.Log($"aimlBot:[USER] {userMessage}");
                AddMessage($"User: {userMessage}", MessageType.User);

                var botMessage = bot.getOutput(userMessage);

                Debug.Log($"aimlBot:[BOT] {botMessage}");
                AddMessage($"Bot: {botMessage}", MessageType.Bot);

                chatBox.Select();
                chatBox.text = "";
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                //Process user message on enter press.
                SendMessageToBot();
            }
        }

        void OnDisable()
        {
            bot.SaveBrain();
        }
    }
}