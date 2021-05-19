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

    public class StartDialog : Dialog
    {
        [Expression("Hello Bot")]
        public void Hello(Context context, Result result)
        {
            result.SendResponse("Hello User!");
        }
    }

    public class GameManager : MonoBehaviour
    {
        OscovaBot MainBot;

        public GameObject chatPanel, textObject;
        public InputField chatBox;

        public Color UserColor, BotColor;

        List<Message> Messages = new List<Message>();

        // Start is called before the first frame update
        void Start()
        {
            try
            {
                //Create new instance of bot.
                MainBot = new OscovaBot();
                OscovaBot.Logger.LogReceived += (s, o) =>
                {
                    Debug.Log($"OscovaBot: {o.Log}");
                };

                //Train on simple dialog.
                MainBot.Dialogs.Add(new StartDialog());

                //Import bot's knowledge-base from a Workspace project file.
                //To read the content of this file ensure you've got Oryzer installed. Visit Oryzer.com

                //UNCHECK THE LINE BELOW TO IMPORT A PIZZA BOT DEMO.
                //MainBot.ImportWorkspace("Assets/pizza-bot.west");
                MainBot.Trainer.StartTraining();

                //When the bot generates a response simply display it.
                MainBot.MainUser.ResponseReceived += (sender, evt) =>
                {
                    AddMessage($"Bot: {evt.Response.Text}", MessageType.Bot);
                };
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
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
                Debug.Log($"OscovaBot:[USER] {userMessage}");
                AddMessage($"User: {userMessage}", MessageType.User);

                //Create a request for bot to process.
                var request = MainBot.MainUser.CreateRequest(userMessage);

                //Evaluate the request (Compute NLU - Natural Language Understanding)
                var evaluationResult = MainBot.Evaluate(request);

                //Invoke the best suggested intent found. This is compel a response generation.
                evaluationResult.Invoke();

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
    }
}