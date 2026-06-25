using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace CyberSecurityBotAwareness
{
    public partial class QuizWindow : Window
    {
        private string userName;
        private int currentQuestionIndex = 0;
        private int score = 0;
        private bool answered = false;

        private List<QuizQuestion> questions = new List<QuizQuestion>()
        {
            new QuizQuestion
            {
                Question = "What is phishing?",
                Options = new[] { "A) A type of malware", "B) A fraudulent attempt to obtain sensitive information by disguising as a trustworthy entity", "C) A firewall technique", "D) A type of encryption" },
                CorrectIndex = 1,
                Explanation = "Phishing is when attackers disguise themselves as trusted sources to trick you into revealing sensitive information like passwords.",
                Type = "MULTIPLE CHOICE"
            },
            new QuizQuestion
            {
                Question = "True or False: You should use the same password for all your accounts to make it easier to remember.",
                Options = new[] { "A) True", "B) False", "", "" },
                CorrectIndex = 1,
                Explanation = "False! Using the same password everywhere is dangerous. If one account is hacked, all your accounts become vulnerable.",
                Type = "TRUE / FALSE"
            },
            new QuizQuestion
            {
                Question = "What does 2FA stand for?",
                Options = new[] { "A) Two-Factor Authentication", "B) Two-File Access", "C) Twice-Failed Attempt", "D) Two-Firewall Application" },
                CorrectIndex = 0,
                Explanation = "2FA stands for Two-Factor Authentication. It adds an extra layer of security by requiring a second verification step beyond just a password.",
                Type = "MULTIPLE CHOICE"
            },
            new QuizQuestion
            {
                Question = "True or False: A VPN encrypts your internet connection and helps protect your privacy online.",
                Options = new[] { "A) True", "B) False", "", "" },
                CorrectIndex = 0,
                Explanation = "True! A VPN (Virtual Private Network) encrypts your connection, making it much harder for hackers to intercept your data.",
                Type = "TRUE / FALSE"
            },
            new QuizQuestion
            {
                Question = "Which of the following is the safest password?",
                Options = new[] { "A) password123", "B) John1990", "C) Tr0ub4dor&3Horse!", "D) 12345678" },
                CorrectIndex = 2,
                Explanation = "Tr0ub4dor&3Horse! is the strongest because it uses uppercase, lowercase, numbers, symbols and is long — making it very hard to crack.",
                Type = "MULTIPLE CHOICE"
            },
            new QuizQuestion
            {
                Question = "True or False: It is safe to click on links in emails from unknown senders if the email looks professional.",
                Options = new[] { "A) True", "B) False", "", "" },
                CorrectIndex = 1,
                Explanation = "False! Phishing emails are designed to look professional and legitimate. Never click links from unknown senders.",
                Type = "TRUE / FALSE"
            },
            new QuizQuestion
            {
                Question = "What is ransomware?",
                Options = new[] { "A) Software that speeds up your computer", "B) A type of antivirus", "C) Malware that locks your files and demands payment", "D) A secure browser extension" },
                CorrectIndex = 2,
                Explanation = "Ransomware is malicious software that encrypts your files and demands a ransom payment to restore access. Always back up your data!",
                Type = "MULTIPLE CHOICE"
            },
            new QuizQuestion
            {
                Question = "True or False: Public Wi-Fi networks are always safe to use for online banking.",
                Options = new[] { "A) True", "B) False", "", "" },
                CorrectIndex = 1,
                Explanation = "False! Public Wi-Fi is often unsecured. Hackers can intercept your data. Always use a VPN on public networks.",
                Type = "TRUE / FALSE"
            },
            new QuizQuestion
            {
                Question = "What does HTTPS mean in a website address?",
                Options = new[] { "A) The website is fast", "B) The website uses a secure encrypted connection", "C) The website is free", "D) The website is government approved" },
                CorrectIndex = 1,
                Explanation = "HTTPS means the website uses SSL/TLS encryption to protect data sent between your browser and the website. Always look for HTTPS!",
                Type = "MULTIPLE CHOICE"
            },
            new QuizQuestion
            {
                Question = "True or False: Antivirus software alone is enough to keep your device completely secure.",
                Options = new[] { "A) True", "B) False", "", "" },
                CorrectIndex = 1,
                Explanation = "False! Antivirus is important but not enough on its own. You also need strong passwords, 2FA, software updates, and safe browsing habits.",
                Type = "TRUE / FALSE"
            },
            new QuizQuestion
            {
                Question = "What is social engineering in cybersecurity?",
                Options = new[] { "A) Building social media platforms", "B) Manipulating people into revealing confidential information", "C) Engineering software for social networks", "D) A type of firewall" },
                CorrectIndex = 1,
                Explanation = "Social engineering is when attackers manipulate people psychologically to reveal confidential information rather than hacking systems directly.",
                Type = "MULTIPLE CHOICE"
            },
            new QuizQuestion
            {
                Question = "True or False: You should regularly update your software and operating system to stay protected.",
                Options = new[] { "A) True", "B) False", "", "" },
                CorrectIndex = 0,
                Explanation = "True! Software updates often contain critical security patches that fix vulnerabilities attackers could exploit. Always keep your software updated!",
                Type = "TRUE / FALSE"
            },
        };

        public QuizWindow(string userName)
        {
            InitializeComponent();
            this.userName = userName;
            QuizProgress.Maximum = questions.Count;
            MainWindow.AddToActivityLog("Quiz started.");
            LoadQuestion();
        }

        private void LoadQuestion()
        {
            if (currentQuestionIndex >= questions.Count)
            {
                ShowResults();
                return;
            }

            answered = false;
            FeedbackBorder.Visibility = Visibility.Collapsed;
            SubmitNextButton.Content = "Submit Answer";

            var q = questions[currentQuestionIndex];

            QuestionText.Text = q.Question;
            QuestionTypeLabel.Text = q.Type;
            QuestionTypeBadge.Background = q.Type == "TRUE / FALSE"
                ? new SolidColorBrush(Color.FromRgb(13, 71, 161))
                : new SolidColorBrush(Color.FromRgb(123, 45, 139));

            // Set options
            OptionA.Content = q.Options[0];
            OptionB.Content = q.Options[1];
            OptionC.Content = q.Options[2];
            OptionD.Content = q.Options[3];

            // Hide unused options for True/False
            OptionC.Visibility = string.IsNullOrEmpty(q.Options[2]) ? Visibility.Collapsed : Visibility.Visible;
            OptionD.Visibility = string.IsNullOrEmpty(q.Options[3]) ? Visibility.Collapsed : Visibility.Visible;

            // Reset selections
            OptionA.IsChecked = false;
            OptionB.IsChecked = false;
            OptionC.IsChecked = false;
            OptionD.IsChecked = false;

            // Reset colours
            OptionA.Foreground = Brushes.White;
            OptionB.Foreground = Brushes.White;
            OptionC.Foreground = Brushes.White;
            OptionD.Foreground = Brushes.White;

            QuestionCountLabel.Text = $"Question {currentQuestionIndex + 1} of {questions.Count}";
            ScoreLabel.Text = $"Score: {score} / {currentQuestionIndex}";
            QuizProgress.Value = currentQuestionIndex;
            QuizStatus.Text = "";
        }

        private void SubmitNextButton_Click(object sender, RoutedEventArgs e)
        {
            if (!answered)
            {
                // Get selected answer
                int selectedIndex = -1;
                if (OptionA.IsChecked == true) selectedIndex = 0;
                else if (OptionB.IsChecked == true) selectedIndex = 1;
                else if (OptionC.IsChecked == true) selectedIndex = 2;
                else if (OptionD.IsChecked == true) selectedIndex = 3;

                if (selectedIndex == -1)
                {
                    QuizStatus.Text = "⚠️ Please select an answer first!";
                    return;
                }

                answered = true;
                var q = questions[currentQuestionIndex];
                bool isCorrect = selectedIndex == q.CorrectIndex;

                if (isCorrect)
                {
                    score++;
                    FeedbackText.Text = $"✅ Correct! {q.Explanation}";
                    FeedbackBorder.Background = new SolidColorBrush(Color.FromRgb(3, 60, 30));
                }
                else
                {
                    FeedbackText.Text = $"❌ Incorrect. The correct answer was: {q.Options[q.CorrectIndex]}\n\n{q.Explanation}";
                    FeedbackBorder.Background = new SolidColorBrush(Color.FromRgb(60, 10, 10));
                }

                // Colour the correct answer green
                var options = new[] { OptionA, OptionB, OptionC, OptionD };
                options[q.CorrectIndex].Foreground = Brushes.LimeGreen;
                if (!isCorrect) options[selectedIndex].Foreground = Brushes.Red;

                FeedbackBorder.Visibility = Visibility.Visible;
                SubmitNextButton.Content = "Next Question ➡";
                QuizStatus.Text = isCorrect ? "🎉 Well done!" : "Better luck next time!";
            }
            else
            {
                currentQuestionIndex++;
                LoadQuestion();
            }
        }

        private void ShowResults()
        {
            SubmitNextButton.Visibility = Visibility.Collapsed;
            ResultPanel.Visibility = Visibility.Visible;

            string resultMessage;
            if (score == questions.Count)
                resultMessage = $"🏆 Perfect Score! {score}/{questions.Count} — You're a Cybersecurity Expert!";
            else if (score >= questions.Count * 0.8)
                resultMessage = $"🌟 Excellent! {score}/{questions.Count} — Great cybersecurity knowledge!";
            else if (score >= questions.Count * 0.6)
                resultMessage = $"👍 Good job! {score}/{questions.Count} — Keep learning to stay safe!";
            else
                resultMessage = $"📚 {score}/{questions.Count} — Keep practising to improve your cybersecurity awareness!";

            ResultText.Text = resultMessage;
            MainWindow.AddToActivityLog($"Quiz completed — Score: {score}/{questions.Count}.");
            ScoreLabel.Text = $"Score: {score} / {questions.Count}";
            QuizProgress.Value = questions.Count;
        }

        private void PlayAgainButton_Click(object sender, RoutedEventArgs e)
        {
            currentQuestionIndex = 0;
            score = 0;
            answered = false;
            SubmitNextButton.Visibility = Visibility.Visible;
            ResultPanel.Visibility = Visibility.Collapsed;
            MainWindow.AddToActivityLog("Quiz restarted.");
            LoadQuestion();
        }
    }

    public class QuizQuestion
    {
        public string Question { get; set; } = "";
        public string[] Options { get; set; } = new string[4];
        public int CorrectIndex { get; set; }
        public string Explanation { get; set; } = "";
        public string Type { get; set; } = "MULTIPLE CHOICE";
    }
}