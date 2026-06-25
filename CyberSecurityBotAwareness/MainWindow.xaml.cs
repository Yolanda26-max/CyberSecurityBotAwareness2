using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace CyberSecurityBotAwareness
{
    public partial class MainWindow : Window
    {
        #region Fields

        private string userName = "";
        private string favouriteTopic = "";
        private string lastTopic = "";
        private string userConcern = "";
        private List<string> topicsDiscussed = new List<string>();
        private Random random = new Random();

        public static List<string> ActivityLog = new List<string>();

        private Dictionary<string, List<string>> keywordResponses = new Dictionary<string, List<string>>()
        {
            { "password", new List<string> {
                "Make sure to use strong, unique passwords for each account. Use a mix of uppercase, lowercase, numbers and symbols!",
                "Consider using a password manager to keep track of your passwords safely. Never reuse the same password!",
                "A strong password should be at least 12 characters long. Use a passphrase to make it memorable but secure.",
                "Never share your passwords with anyone. legitimate services will never ask for your password!"
            }},
            { "scam", new List<string> {
                "Be cautious of unsolicited messages asking for personal information. Scammers often disguise themselves as trusted organisations!",
                "If something sounds too good to be true, it probably is. Online scams often promise prizes or money.",
                "Always verify the identity of anyone asking for money or personal details using official contact details.",
                "Romance scams are on the rise  be cautious of people online who quickly ask for money or personal information."
            }},
            { "privacy", new List<string> {
                "Protect your privacy by reviewing app permissions regularly. Limit what personal information you share online.",
                "Use a VPN when connecting to public Wi-Fi to protect your personal data from being intercepted.",
                "Regularly check your social media privacy settings. Oversharing details can make you a target for identity theft.",
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
                "Ransomware locks your files and demands payment. Always back up your important files to cloud storage.",
                "Never download software from pop-up ads or unofficial websites. Only use trusted sources.",
                "Keep your operating system updated — many updates contain important security patches against malware."
            }},
            { "virus", new List<string> {
                "A virus can spread and damage your files. Run regular antivirus scans and avoid opening attachments from unknown senders.",
                "Computer viruses can spread through USB drives. Always scan external devices before opening files.",
                "Signs of infection: slow performance, unexpected pop-ups, programs opening on their own. Run a full scan immediately."
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
                "Encryption converts your data into a secure format that only authorised parties can read. Always use HTTPS!",
                "End-to-end encryption means only you and the recipient can read messages — look for apps that offer this.",
                "Full disk encryption protects all data on your device if it's lost or stolen. Enable it in your security settings."
            }},
            { "2fa", new List<string> {
                "Two-Factor Authentication adds an extra layer of security. Enable it wherever possible!",
                "Use an authenticator app instead of SMS for 2FA — SMS can be intercepted by attackers.",
                "Even if someone steals your password, 2FA prevents them from accessing your account without the second step."
            }}
        };

        private Dictionary<string, string> nlpIntentMap = new Dictionary<string, string>()
        {
            { "add task", "add_task" },
            { "create task", "add_task" },
            { "new task", "add_task" },
            { "add a task", "add_task" },
            { "i need to", "add_task" },
            { "remind me", "set_reminder" },
            { "set a reminder", "set_reminder" },
            { "set reminder", "set_reminder" },
            { "don't let me forget", "set_reminder" },
            { "quiz", "start_quiz" },
            { "test me", "start_quiz" },
            { "start quiz", "start_quiz" },
            { "play game", "start_quiz" },
            { "challenge me", "start_quiz" },
            { "test my knowledge", "start_quiz" },
            { "show activity log", "show_log" },
            { "activity log", "show_log" },
            { "what have you done", "show_log" },
            { "show log", "show_log" },
            { "recent actions", "show_log" },
            { "show tasks", "open_tasks" },
            { "my tasks", "open_tasks" },
            { "view tasks", "open_tasks" },
            { "task list", "open_tasks" },
            { "manage tasks", "open_tasks" },
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
            AppendMessage("Guardian Bot",
                "Hello! I'm your Cybersecurity Guardian Bot 🛡️\n" +
                "What's your name?", "#E91E8C");
        }

        #endregion

        #region Event Handlers

        private void SendButton_Click(object sender, RoutedEventArgs e) => ProcessInput();

        private void UserInputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) ProcessInput();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ChatDisplay.Document.Blocks.Clear();
            userName = "";
            favouriteTopic = "";
            lastTopic = "";
            userConcern = "";
            topicsDiscussed.Clear();
            StatusBar.Text = "● Chat cleared. Type a message to begin.";
            ShowWelcomeMessage();
        }

        private void TaskManagerButton_Click(object sender, RoutedEventArgs e)
        {
            AddToActivityLog("User opened the Task Manager");
            var taskWindow = new TaskManagerWindow(userName);
            taskWindow.Owner = this;
            taskWindow.ShowDialog();
        }

        private void QuizButton_Click(object sender, RoutedEventArgs e)
        {
            AddToActivityLog("Quiz started via navigation button.");
            var quizWindow = new QuizWindow(userName);
            quizWindow.Owner = this;
            quizWindow.ShowDialog();
        }

        private void ActivityLogButton_Click(object sender, RoutedEventArgs e)
        {
            AppendMessage("Guardian Bot", ShowActivityLog(), "#E91E8C");
        }

        #endregion

        #region Input Processing

        private void ProcessInput()
        {
            string input = UserInputBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(input)) return;

            AppendMessage("You", input, "#58A6FF");
            UserInputBox.Clear();

            string response = GenerateResponse(input);
            AppendMessage("Guardian Bot", response, "#E91E8C");

            StatusBar.Text = "● Bot responded successfully.";
        }

        #endregion

        #region Response Generation

        private string GenerateResponse(string input)
        {
            string lower = input.ToLower();

            if (string.IsNullOrEmpty(userName))
            {
                userName = input;
                AddToActivityLog($"User introduced themselves as '{userName}'.");
                return $"Nice to meet you, {userName}! 😊\n\n" +
                       $"You can ask me about: passwords, scams, privacy, phishing, malware, VPNs, firewalls, encryption, or 2FA.\n\n" +
                       $"Use the buttons above to open the 📋 Task Manager or 🎮 Quiz!\n" +
                       $"Type 'show activity log' to see recent actions.";
            }

            string intent = DetectIntent(lower);
            if (intent != null)
            {
                AddToActivityLog($"NLP detected intent '{intent}' from: \"{input}\"");
                return HandleIntent(intent, input);
            }

            if (lower.Contains("interested in") || lower.Contains("i like") || lower.Contains("my favourite"))
            {
                foreach (var keyword in keywordResponses.Keys)
                {
                    if (lower.Contains(keyword))
                    {
                        favouriteTopic = keyword;
                        lastTopic = keyword;
                        TrackTopic(keyword);
                        AddToActivityLog($"User set favourite topic to '{keyword}'.");
                        return $"Great! I'll remember that you're interested in {keyword}, {userName}.\n\n{GetRandomResponse(keyword)}";
                    }
                }
            }

            if (lower.Contains("i am concerned") || lower.Contains("i'm concerned") || lower.Contains("my concern"))
            {
                userConcern = input;
                string tip = GenerateCybersecurityTip(lower) ?? "Ask me about a specific topic like phishing or passwords!";
                return $"I've noted your concern, {userName}. Let me share some tips!\n\n{tip}";
            }

            if (lower.Contains("what have we discussed") || lower.Contains("what did we talk about"))
            {
                if (topicsDiscussed.Count > 0)
                    return $"We've discussed: {string.Join(", ", topicsDiscussed)}. Would you like to explore any further, {userName}?";
                return $"We haven't discussed any specific topics yet, {userName}. Ask me about passwords, phishing, or malware!";
            }

            if (lower.Contains("tell me more") || lower.Contains("explain more") ||
                lower.Contains("another tip") || lower.Contains("more info"))
            {
                if (!string.IsNullOrEmpty(lastTopic))
                    return $"Here's another tip about {lastTopic}, {userName}:\n\n{GetRandomResponse(lastTopic)}";
                return $"What topic would you like to know more about, {userName}?";
            }

            if (lower.Contains("worried") || lower.Contains("scared") ||
                lower.Contains("anxious") || lower.Contains("nervous"))
            {
                string tip = GenerateCybersecurityTip(lower);
                return $"It's completely understandable to feel that way, {userName}. I'm here to help! 💪\n\n" +
                       (tip ?? "Let me know what specific topic is worrying you!");
            }

            if (lower.Contains("frustrated") || lower.Contains("confused") || lower.Contains("don't understand"))
                return $"No worries, {userName}! Let's take it step by step 😊. Which topic are you finding confusing?";

            if (lower.Contains("curious") || lower.Contains("interesting") || lower.Contains("want to know"))
                return $"Love the curiosity, {userName}! 🔍 Ask me about any cybersecurity topic!";

            string keywordResponse = GenerateCybersecurityTip(lower);
            if (keywordResponse != null) return keywordResponse;

            if (lower.Contains("hello") || lower.Contains("hi") || lower.Contains("hey"))
                return $"Hey there, {userName}! 👋 How can I help you stay safe online today?";
            if (lower.Contains("thank"))
                return $"You're welcome, {userName}! Stay safe out there 🛡️";
            if (lower.Contains("bye") || lower.Contains("goodbye"))
                return $"Goodbye, {userName}! Stay cyber-safe! 🛡️";
            if (lower.Contains("how are you"))
                return $"I'm running smoothly and ready to help, {userName}! 😄";

            return $"I didn't quite understand that, {userName}. Could you rephrase? 🤔\n\n" +
                   $"Try asking about: passwords, scams, privacy, phishing, malware, VPNs, firewalls, encryption, or 2FA.\n" +
                   $"Or type 'show activity log', 'start quiz', or 'add task'.";
        }

        #endregion

        #region NLP Intent Detection

        private string DetectIntent(string lower)
        {
            foreach (var entry in nlpIntentMap)
                if (lower.Contains(entry.Key)) return entry.Value;
            return null;
        }

        private string HandleIntent(string intent, string originalInput)
        {
            switch (intent)
            {
                case "add_task":
                    OpenTaskManager();
                    return $"It sounds like you want to add a task, {userName}! I've opened the Task Manager for you.";
                case "set_reminder":
                    OpenTaskManager();
                    return $"Sure, {userName}! I've opened the Task Manager where you can set a reminder.";
                case "start_quiz":
                    OpenQuiz();
                    return $"Great idea, {userName}! Opening the quiz now — good luck! 🎮";
                case "show_log":
                    return ShowActivityLog();
                case "open_tasks":
                    OpenTaskManager();
                    return $"Opening the Task Manager for you, {userName}! 📋";
                default:
                    return null;
            }
        }

        private void OpenTaskManager()
        {
            AddToActivityLog("Task Manager opened via NLP command.");
            var taskWindow = new TaskManagerWindow(userName);
            taskWindow.Owner = this;
            taskWindow.ShowDialog();
        }

        private void OpenQuiz()
        {
            AddToActivityLog("Quiz started via NLP command.");
            var quizWindow = new QuizWindow(userName);
            quizWindow.Owner = this;
            quizWindow.ShowDialog();
        }

        #endregion

        #region Activity Log

        public static void AddToActivityLog(string action)
        {
            ActivityLog.Add($"[{DateTime.Now:HH:mm:ss}] {action}");
        }

        private string ShowActivityLog()
        {
            if (ActivityLog.Count == 0)
                return $"No actions recorded yet, {userName}. Start chatting, add tasks, or take the quiz!";

            int start = Math.Max(0, ActivityLog.Count - 10);
            var recent = ActivityLog.GetRange(start, ActivityLog.Count - start);

            string log = $"Here's a summary of recent actions, {userName}:\n\n";
            for (int i = 0; i < recent.Count; i++)
                log += $"{i + 1}. {recent[i]}\n";

            if (ActivityLog.Count > 10)
                log += $"\n(Showing last 10 of {ActivityLog.Count} total actions)";

            return log;
        }

        #endregion

        #region Cybersecurity Tip Helpers

        private string GenerateCybersecurityTip(string lower)
        {
            foreach (var keyword in keywordResponses.Keys)
            {
                if (lower.Contains(keyword))
                {
                    lastTopic = keyword;
                    TrackTopic(keyword);
                    AddToActivityLog($"User asked about '{keyword}'.");

                    if (!string.IsNullOrEmpty(favouriteTopic) && keyword == favouriteTopic)
                        return $"Since {keyword} is your favourite topic, {userName}, here's a tip:\n\n{GetRandomResponse(keyword)}";

                    return GetRandomResponse(keyword);
                }
            }
            return null;
        }

        private string GetRandomResponse(string keyword)
        {
            if (keywordResponses.ContainsKey(keyword))
            {
                var responses = keywordResponses[keyword];
                return responses[random.Next(responses.Count)];
            }
            return "I don't have information on that topic yet.";
        }

        private void TrackTopic(string topic)
        {
            if (!topicsDiscussed.Contains(topic))
                topicsDiscussed.Add(topic);
        }

        #endregion

        #region UI Helpers

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