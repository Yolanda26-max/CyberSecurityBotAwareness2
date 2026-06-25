#  Cybersecurity Guardian Bot

**Student:** Nomhle Yolanda Makhanya  
**Student ID:** ST10470578  
**Module:** PROG6221 - Programming 2B  
**Institution:** Rosebank International  



##  Project Overview

The Cybersecurity Guardian Bot is a WPF-based C# desktop application designed to educate users about cybersecurity topics in a conversational and engaging way. The bot recognises cybersecurity keywords, remembers user details, detects user sentiment, and provides varied, helpful responses to keep users safe online.



##  Features

###  GUI Design
- Professional dark-themed WPF interface with pink accents
- Clean layout with chat display, input box, Send and Clear buttons
- Timestamped messages for a natural chat experience
- Status bar showing current user and topics covered

###  Keyword Recognition
The bot recognises and responds to the following cybersecurity topics:
- Passwords
- Phishing
- Scams
- Privacy
- Malware & Viruses
- Ransomware
- Hacking
- Identity Theft
- Firewalls
- VPNs
- Encryption
- Two-Factor Authentication (2FA)

###  Random Responses
- Multiple predefined responses per topic stored in a `Dictionary<string, List<string>>`
- Uses `Random` to select a different response each time, keeping conversations varied and engaging

###  Conversation Flow
- Handles follow-up phrases like "tell me more", "another tip", "give me another"
- Continues the current topic without restarting the conversation
- Type `help` to see all available topics at any time

###  Memory and Recall
The bot remembers:
- User's **name** throughout the conversation
- User's **favourite topic** (personalises responses accordingly)
- User's **concern** when expressed
- All **topics discussed** — type "what have we discussed" to recall them

###  Sentiment Detection
The bot detects and responds empathetically to:
- **Worried / Scared / Anxious / Overwhelmed** — offers reassurance and tips
- **Frustrated / Confused** — offers to simplify explanations
- **Happy / Excited / Curious** — matches positive energy

###  Error Handling
- Default response for unrecognised inputs
- Suggests the `help` command when the bot doesn't understand
- App never crashes on unexpected input

###  Code Optimisation
- Uses `Dictionary` and `List` for efficient data management
- Organised with `#region` blocks for readability
- Full XML documentation comments on all methods
- Clean OOP structure with separation of concerns



##  How to Run

1. Clone the repository:

git clone https://github.com/YOUR_USERNAME/CyberSecurityBotAwareness.git

2. Open the solution in **Visual Studio 2026**
3. Build the solution (**Ctrl+Shift+B**)
4. Run the application (**F5** or green play button)


##  Technologies Used

- **Language:** C#
- **Framework:** .NET 8.0
- **UI:** WPF (Windows Presentation Foundation)
- **IDE:** Visual Studio 2026
- **Version Control:** Git & GitHub



##  Project Structure


CyberSecurityBotAwareness/
├── MainWindow.xaml          # GUI layout and design
├── MainWindow.xaml.cs       # Bot logic, responses, and event handlers
├── App.xaml                 # Application entry point
├── App.xaml.cs              # Application configuration
└── README.md                # Project documentation


##  Example Interactions

| User Input | Bot Response |
|---|---|
| `Yolanda` | Greets user and asks for favourite topic |
| `tell me about phishing` | Shares a random phishing tip |
| `tell me more` | Shares another phishing tip |
| `I'm worried about scams` | Responds empathetically and shares a scam tip |
| `what have we discussed` | Lists all topics covered in the session |
| `help` | Shows all available topics |

##  Version History

| Version | Description |
|---|---|
| Part 1 | Console-based chatbot with basic keyword responses and audio |
| Part 2 | WPF GUI chatbot with sentiment detection, memory, and random responses |



