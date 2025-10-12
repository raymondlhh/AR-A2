using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenAI;
using OpenAI.Models;
using OpenAI.Chat;
using TMPro;
using System.Threading.Tasks;

public class ChatGPTManager : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private InputField inputField;
    [SerializeField] private Text displayText;
    private Text inputText;
    public Text OutputText;
    //[SerializeField] private SpeechController speechController;

    private string userInput;
    private string chatHistory;
    [SerializeField] private string aiIdentity = "";

    [SerializeField] private string apiKey = "";
    private OpenAIClient api;

    private void Start()
    {
        chatHistory += aiIdentity + "\n";
        api = new OpenAIClient(new OpenAIAuthentication(apiKey));
        button.onClick.AddListener(AskAI);
    }

    private async void AskAI()
    {
        button.enabled = false;
        inputField.enabled = false;

        userInput = inputField.text;
        chatHistory += $"{userInput}\n";

        displayText.text = "......";
        inputField.text = "";

        try
        {
            var messages = new List<Message>
            {
                new Message(Role.System, aiIdentity),
                new Message(Role.User, userInput)
            };

            var chatRequest = new ChatRequest(messages, model: Model.GPT3_5_Turbo);
            var result = await api.ChatEndpoint.GetCompletionAsync(chatRequest);

            if (result.Choices.Count > 0)
            {
                var response = result.Choices[0].Message.Content.ToString();
                displayText.text = response;
                chatHistory += $"{response}\n";
            }
            else
            {
                displayText.text = "No response from ChatGPT.";
            }
        }
        catch (System.Exception e)
        {
            displayText.text = $"Error: {e.Message}";
            Debug.LogError($"OpenAI API Error: {e}");
        }

        // // Start text-to-speech with the AI response
        // if (speechController != null)
        // {
        //     Debug.Log($"Starting TTS with text: {displayText.text}");
        //     speechController.StartTextRecognition(displayText.text);
        // }
        // else
        // {
        //     Debug.LogError("SpeechController is not assigned!");
        // }

        button.enabled = true;
        inputField.enabled = true;
    }

    /// <summary>
    /// Public method to send text to ChatGPT and get response using inputText and OutputText
    /// </summary>
    /// <param name="textToSend">The text to send to ChatGPT</param>
    public async void SendTextToChatGPT(string textToSend)
    {
        if (string.IsNullOrEmpty(textToSend))
        {
            Debug.LogWarning("ChatGPTManager: No text provided to send to ChatGPT");
            return;
        }

        // Set the inputText to the text we want to send
        if (inputText != null)
        {
            inputText.text = textToSend;
        }

        // Set OutputText to show loading
        if (OutputText != null)
        {
            OutputText.text = "......";
        }

        try
        {
            var messages = new List<Message>
            {
                new Message(Role.System, aiIdentity),
                new Message(Role.User, textToSend)
            };

            var chatRequest = new ChatRequest(messages, model: Model.GPT3_5_Turbo);
            var result = await api.ChatEndpoint.GetCompletionAsync(chatRequest);

            if (result.Choices.Count > 0)
            {
                var response = result.Choices[0].Message.Content.ToString();
                if (OutputText != null)
                {
                    OutputText.text = response;
                }
                chatHistory += $"{textToSend}\n{response}\n";
            }
            else
            {
                if (OutputText != null)
                {
                    OutputText.text = "No response from ChatGPT.";
                }
            }
        }
        catch (System.Exception e)
        {
            if (OutputText != null)
            {
                OutputText.text = $"Error: {e.Message}";
            }
            Debug.LogError($"OpenAI API Error: {e}");
        }
    }
}