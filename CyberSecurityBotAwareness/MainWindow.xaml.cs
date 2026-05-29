using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;


namespace CyberSecurityBotAwareness
{
    /// <summary>
    /// Cybersecurity Guardian Bot - Part 2
    /// GUI-based chatbot with keyword recognition, memory, sentiment detection and random responses
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields

        // Memory storage - remembers multiple user details
        private string userName = "";
        private string favouriteTopic = "";
        private string lastTopic = "";
        private string userConcern = "";
        private List<string> topicsDiscussed = new List<string>();
        private Random random = new Random();

        // Keyword responses using Dictionary - multiple responses per topic for variety
        private Dictionary<string, List<string>> keywordResponses = new Dictionary<string, List<string>>()
        {
            { "password", new List<string> {
                "Make sure to use strong, unique passwords for each account. Use a mix of uppercase, lowercase, numbers and symbols!",
                "Consider using a password manager to keep track of your passwords safely. Never reuse the same password across multiple sites!",
                "A strong password should be at least 12 characters long. Use a passphrase to make it memorable but secure.",
                "Never share your passwords with anyone — legitimate services will never ask for your password!"
            }},
            { "scam", new List<string> {
                "Be cautious of unsolicited messages asking for personal information. Scammers often disguise themselves as trusted organisations!",
                "If something sounds too good to be true, it probably is. Online scams often promise prizes or money to get your attention.",
                "Always verify the identity of anyone asking for money or personal details using official contact details.",
                "Romance scams are on the rise — be cautious of people online who quickly ask for money or personal information."
            }},
            { "privacy", new List<string> {
                "Protect your privacy by reviewing app permissions regularly. Limit what personal information you share online.",
                "Use a VPN when connecting to public Wi-Fi to protect your personal data from being intercepted.",
                "Regularly check your social media privacy settings. Oversharing personal details can make you a target for identity theft.",
                "Be careful about what you post online — even deleted content can sometimes be recovered or screenshotted."
            }},
            { "phishing", new List<string> {
                "Be cautious of emails asking for personal information. Scammers often disguise themselves as trusted organisations.",
                "Never click on links in unsolicited emails. Always go directly to the website by typing the address in your browser.",
                "Check the sender's email address carefully. Phishing emails often use addresses that look similar to legitimate ones.",
                "Look out for urgent language in emails — phrases like 'Act now!' are common phishing tactics.",
                "When in doubt, call the company directly using a number from their official website."
            }},
            { "malware", new List<string> {
                "Malware is malicious software designed to harm your device. Always keep your antivirus updated!",
                "Ransomware locks your files and demands payment. Always back up your important files to an external drive or cloud.",
                "Never download software from pop-up ads or unofficial websites. Only use trusted sources.",
                "Keep your operating system updated — many updates contain important security patches against malware."
            }},
            { "virus", new List<string> {
                "A virus can spread and damage your files. Run regular antivirus scans and avoid opening attachments from unknown senders.",
                "Computer viruses can spread through USB drives. Always scan external devices before opening files.",
                "Signs your device may be infected: slow performance, unexpected pop-ups, programs opening on their own."
            }},
            { "firewall", new List<string> {
                "A firewall monitors and controls incoming and outgoing network traffic. Always keep your firewall enabled!",
                "Both hardware and software firewalls are important. Make sure your router firewall is enabled.",
                "A firewall acts as a barrier between your trusted network and untrusted external networks."
            }},
            { "vpn", new List<string> {
                "A VPN encrypts your internet connection, keeping your online activity private — especially on public Wi-Fi.",
                "Not all VPNs are equal — choose a reputable paid VPN service. Free VPNs may log and sell your data.",
                "A VPN hides your IP address, making it harder for websites and hackers to track your location."
            }},
            { "encryption", new List<string> {
                "Encryption converts your data into a secure format that only authorised parties can read. Always use HTTPS when browsing.",
                "End-to-end encryption means only you and the recipient can read messages — look for apps that offer this.",
                "Full disk encryption protects all data on your device if it's lost or stolen. Enable it in your security settings."
            }},
            { "2fa", new List<string> {
                "Two-Factor Authentication adds an extra layer of security. Enable it wherever possible!",
                "Use an authenticator app instead of SMS for 2FA — SMS can be intercepted by attackers.",
                "Even if someone steals your password, 2FA prevents them from accessing your account without the second step."
            }}
        };

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
            ShowWelcomeMessage();
        }

        
        private void ShowWelcomeMessage()
        {
            AppendMessage("Guardian Bot", "Hello! I'm your Cybersecurity Guardian Bot 🛡️\nWhat's your name?", "#00FF41");
        }

        #endregion

        #region Event Handlers

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessInput();
        }

        private void UserInputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ProcessInput();
        }

        /// <summary>
        /// Clears chat and resets all memory
        /// </summary>
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ChatDisplay.Document.Blocks.Clear();
            userName = "";
            favouriteTopic = "";
            lastTopic = "";
            userConcern = "";
            topicsDiscussed.Clear();
            StatusBar.Text = "🟢 Chat cleared. Type a message to begin.";
            ShowWelcomeMessage();
        }

        #endregion

        #region Input Processing

        /// <summary>
        /// Reads user input and generates a response
        /// </summary>
        private void ProcessInput()
        {
            string input = UserInputBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(input)) return;

            AppendMessage("You", input, "#58A6FF");
            UserInputBox.Clear();

            string response = GenerateResponse(input);
            AppendMessage("Guardian Bot", response, "#00FF41");

            StatusBar.Text = "🟢 Bot responded successfully.";
        }

        #endregion

        #region Response Generation

        /// <summary>
        /// Generates a response based on user input
        /// Checks name, memory, conversation flow, sentiment, and keywords
        /// </summary>
        private string GenerateResponse(string input)
        {
            string lower = input.ToLower();

            // Get user name first
            if (string.IsNullOrEmpty(userName))
            {
                userName = input;
                return $"Nice to meet you, {userName}! 😊 I'm here to help you stay safe online.\n\n" +
                       $"You can ask me about: passwords, scams, privacy, phishing, malware, VPNs, firewalls, encryption, or 2FA.\n\n" +
                       $"What cybersecurity topic interests you most?";
            }

            // Memory — store favourite topic
            if (lower.Contains("interested in") || lower.Contains("i like") || lower.Contains("my favourite"))
            {
                foreach (var keyword in keywordResponses.Keys)
                {
                    if (lower.Contains(keyword))
                    {
                        favouriteTopic = keyword;
                        lastTopic = keyword;
                        TrackTopic(keyword);
                        return $"Great! I'll remember that you're interested in {keyword}, {userName}. " +
                               $"It's a crucial part of staying safe online.\n\n{GetRandomResponse(keyword)}";
                    }
                }
            }

            // Memory — store user concern
            if (lower.Contains("i am concerned") || lower.Contains("i'm concerned") || lower.Contains("my concern"))
            {
                userConcern = input;
                return $"I've noted your concern, {userName}. Let me share some tips!\n\n{GenerateCybersecurityTip(lower) ?? "Ask me about a specific topic like phishing or passwords!"}";
            }

            // Memory recall — topics discussed
            if (lower.Contains("what have we discussed") || lower.Contains("what did we talk about"))
            {
                if (topicsDiscussed.Count > 0)
                    return $"We've discussed: {string.Join(", ", topicsDiscussed)}. Would you like to explore any further, {userName}?";
                return $"We haven't discussed any specific topics yet, {userName}. Ask me about passwords, phishing, or malware!";
            }

            // Conversation flow — follow up requests
            if (lower.Contains("tell me more") || lower.Contains("explain more") ||
                lower.Contains("another tip") || lower.Contains("more info"))
            {
                if (!string.IsNullOrEmpty(lastTopic))
                    return $"Here's another tip about {lastTopic}, {userName}:\n\n{GetRandomResponse(lastTopic)}";
                return $"What topic would you like to know more about, {userName}?";
            }

            // Sentiment detection — worried/scared
            if (lower.Contains("worried") || lower.Contains("scared") ||
                lower.Contains("anxious") || lower.Contains("nervous"))
            {
                string tip = GenerateCybersecurityTip(lower);
                return $"It's completely understandable to feel that way, {userName}. I'm here to help! 💪\n\n" +
                       (tip ?? "Let me know what specific topic is worrying you and I'll share some tips!");
            }

            // Sentiment detection — frustrated/confused
            if (lower.Contains("frustrated") || lower.Contains("confused") || lower.Contains("don't understand"))
            {
                return $"No worries, {userName}! Let's take it step by step 😊. " +
                       $"Which topic are you finding confusing? I can explain anything in simple terms!";
            }

            // Sentiment detection — curious/excited
            if (lower.Contains("curious") || lower.Contains("interesting") || lower.Contains("want to know"))
            {
                return $"Love the curiosity, {userName}! 🔍 Ask me about any cybersecurity topic and I'll share what I know!";
            }

            // Keyword recognition
            string keywordResponse = GenerateCybersecurityTip(lower);
            if (keywordResponse != null)
                return keywordResponse;

            // Greetings
            if (lower.Contains("hello") || lower.Contains("hi") || lower.Contains("hey"))
                return $"Hey there, {userName}! 👋 How can I help you stay safe online today?";

            if (lower.Contains("thank"))
                return $"You're welcome, {userName}! Stay safe out there 🛡️";

            if (lower.Contains("bye") || lower.Contains("goodbye"))
                return $"Goodbye, {userName}! Stay cyber-safe! 🛡️";

            if (lower.Contains("how are you"))
                return $"I'm running smoothly and ready to help, {userName}! 😄";

            // Default error handling
            return $"I'm not sure I understand that, {userName}. Can you try rephrasing? 🤔\n\n" +
                   $"Try asking about: passwords, scams, privacy, phishing, malware, VPNs, firewalls, encryption, or 2FA.";
        }

        /// <summary>
        /// Searches user input for cybersecurity keywords and returns a random matching response
        /// </summary>
        private string GenerateCybersecurityTip(string lower)
        {
            foreach (var keyword in keywordResponses.Keys)
            {
                if (lower.Contains(keyword))
                {
                    lastTopic = keyword;
                    TrackTopic(keyword);

                    if (!string.IsNullOrEmpty(favouriteTopic) && keyword == favouriteTopic)
                        return $"Since {keyword} is your favourite topic, {userName}, here's a tip:\n\n{GetRandomResponse(keyword)}";

                    return GetRandomResponse(keyword);
                }
            }
            return null;
        }

        /// <summary>
        /// Returns a random response from the list for a given keyword
        /// Uses List and Random for varied, engaging responses
        /// </summary>
        private string GetRandomResponse(string keyword)
        {
            if (keywordResponses.ContainsKey(keyword))
            {
                var responses = keywordResponses[keyword];
                return responses[random.Next(responses.Count)];
            }
            return "I don't have information on that topic yet.";
        }

        /// <summary>
        /// Tracks topics discussed for memory recall feature
        /// </summary>
        private void TrackTopic(string topic)
        {
            if (!topicsDiscussed.Contains(topic))
                topicsDiscussed.Add(topic);
        }

        #endregion

        #region UI Helpers

        /// <summary>
        /// Appends a colour coded message to the chat display
        /// </summary>
        private void AppendMessage(string sender, string message, string colorHex)
        {
            var paragraph = new Paragraph();
            paragraph.Margin = new Thickness(0, 5, 0, 5);

            var senderRun = new Run($"{sender}: ")
            {
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorHex)),
                FontWeight = FontWeights.Bold
            };

            var messageRun = new Run(message)
            {
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C9D1D9"))
            };

            paragraph.Inlines.Add(senderRun);
            paragraph.Inlines.Add(messageRun);
            ChatDisplay.Document.Blocks.Add(paragraph);

            ChatScrollViewer.ScrollToBottom();
        }

        #endregion
    }
}