# Chas Bank

A C# console bank application for the initial backlog. The banking code uses classes, constructors, inheritance, polymorphism, lists, dictionaries, enums, loops and input validation. Menus and comments are in Swedish. Planning and technical documentation are in English.

This project was generated with AI assistance. The group should understand, review and adapt the code before submitting it. The commit simulation is a planning example. The included Git bundle records the actual assistant development commits separately.

## Open and run

1. Extract the ZIP to a normal folder.
2. Open `BankApp.sln` in Visual Studio with the .NET 8 SDK installed.
3. Set the **BankApp** project as the startup project.
4. Press **Ctrl+F5** to run without the debugger.

From a terminal in the solution folder:

```sh
dotnet build BankApp.sln
dotnet run --project BankApp/BankApp.csproj
dotnet run --project BankApp.Tests/BankApp.Tests.csproj
```

The tests are a separate console project with no third party packages. A successful run ends with `34 passed, 0 failed` and exit code 0.

## First run and demo data

| Role | Username | Password | Starting accounts |
| --- | --- | --- | --- |
| Administrator | admin | Admin123 | No personal accounts |
| Customer | johan | Johan123 | 1001: 10,000 SEK; 1003: savings, 0 SEK; 1004: 0 EUR |
| Customer | sara | Sara123 | 1002: 5,000 SEK |

These are fictional credentials and balances for the school demonstration. New users start without accounts or money. They open an account themselves. An administrator can register an external deposit.

On first run, exchange rates have **not** been confirmed for today. Log in as admin, choose **90**, then **4**, and enter the day's rates. For a fictional demonstration, use 11 SEK per EUR and 10 SEK per USD. These values are examples, not live market rates.

For a quick demonstration, log in as johan, choose **2**, transfer 110 SEK from 1001 to 1004, then view accounts and history. The receiving account gets 10 EUR when the batch runs. A transfer from 1001 to savings account 1003 also shows an annual interest estimate.

## Source code structure

| File / class | Responsibility |
| --- | --- |
| `Program.cs` / `Program` | Main only calls `new Menu().Start()` to start the application. |
| `Menu.cs` / `Menu` | Displays customer and administrator menus, reads choices and calls Bank. It also displays confirmations, balances and interest estimates. |
| `Bank.cs` / `Bank` | Checks permissions and ownership, creates users and accounts, handles login, queues and processes transactions, applies rates and calculates loan limits and statistics. |
| `Customer.cs` / `Customer` | Stores a customer's identity, profile, role, login lock, credit rating and notification preferences. Checks and changes passwords. Admins are Customer objects with IsAdmin set to true. |
| `BankAccount.cs` / `BankAccount` | Stores account number, owner, name, currency and balance. Deposit and Withdraw control changes to Balance, which has a private setter. A normal account returns zero savings interest. |
| `SavingsAccount.cs` / `SavingsAccount` | Inherits BankAccount and overrides CalculateAnnualInterest. Adds InterestRate. |
| `PendingTransaction.cs` / `PendingTransaction` | Stores the requested amounts, account numbers, fixed exchange quote, scheduled time, actual processing time, type, status and any failure reason. Completed records remain in the same list as history. |
| `Loan.cs` / `Loan` | Stores the owner, original loan, remaining principal in SEK, agreed rate and loan status. Calculates one year's interest on remaining principal. |
| `Notification.cs` / `Notification` | Stores a large transaction alert, recipient details, chosen channels and the outcome of each delivery attempt. |
| `BankData.cs` / `BankData` | Holds the lists and dictionaries that belong to this bank. This is the object saved to JSON. |
| `DataStore.cs` / `DataStore` | Loads and validates stored data. Saves a temporary file, keeps a backup, then replaces the main data file. |
| `ConsoleInput.cs` / `ConsoleInput` | Reads and validates input. Accepts comma or dot for money, hides typed passwords and checks for due batches while waiting for keyboard input. |
| `ConsoleView.cs` / `ConsoleView` | Displays the ASCII logo, headings and colored success/error messages. |
| `INotificationSender.cs` / `INotificationSender` | Defines the Send method. Bank can use the real sender or the preview sender used by tests. |
| `NotificationSender.cs` / `NotificationSender` | Optionally sends email using SMTP and SMS using Twilio. Preview mode is the default. External settings come from environment variables. |
| `Enums.cs` | Defines Currency, CreditRating, TransactionType, TransactionStatus, LoanStatus and NotificationChannel. |
| `BankApp.Tests/Program.cs` | Starts the test suite. |
| `BankApp.Tests/TestSuite.cs` / `TestSuite` | Runs independent checks for invalid input, login, ownership, transfers, loans, notifications, statistics and persistence. |

Objects contain the data for one thing. For example, each Customer represents one user, each BankAccount represents one account, and each PendingTransaction represents one requested money operation. The same Bank instance is used throughout the session.

Accounts, loans and notifications store their owner's CustomerNumber. Transactions store account numbers and an optional LoanNumber. The code looks up the matching object using these identifiers.

The UML overview selects the main properties and methods. It is not a complete list of every member:

![UML overview](docs/UML.png)

Editable diagram sources: [SVG](docs/UML.svg) and [Mermaid](docs/UML.mmd).

## OOP and course concepts

| Concept | Example in this project | Supplied course source |
| --- | --- | --- |
| Classes, objects and constructors | Customer and BankAccount, created with `new` | `New Text Document (2).txt` |
| Access modifiers and encapsulation | Private Balance setter and account methods | `NET26_260924.pdf`, agenda on page 2; `New Text Document (3).txt` |
| Inheritance | SavingsAccount inherits BankAccount | `NET26_260924.pdf`, agenda on page 2 |
| Polymorphism, virtual and override | CalculateAnnualInterest runs the savings override even through a BankAccount variable | `NET26_260928.pdf`, agenda on page 2 and virtual/abstract on page 17 |
| Interface | INotificationSender separates the sending method from Bank | `NET26_260928.pdf`, agenda on page 2 |
| Generic collections | `List<Customer>`, `List<BankAccount>`, `List<PendingTransaction>` | `NET26_260930.pdf`, agenda on page 2 |
| Dictionary and enum | Currency exchange rates and credit rating rules | `New Text Document.txt` |
| Conditions, arrays and loops | Menu switch, account foreach, input while, statistics array | `net26_-_loopar_arrays_-_260910.pdf` and `NET26_villkorsatser_260907.pdf` |
| Error handling and Git | Try/catch, commits, branches and reviews | `NET26_260917.pdf`, agenda on page 2 |

JSON serialization, password hashing, console polling and external notification APIs are small extensions beyond the main course concepts. They are kept in named classes or methods. LINQ calls such as Where, Any and Sum help filter and total collections. Read [the code guide](docs/CodeGuide.md) for short explanations.

## Transaction rules

- New transfers, admin deposits, withdrawals, loan payouts and loan repayments are queued. No balance changes when a request is created.
- Batches run at the next local clock quarter: :00, :15, :30 or :45. A request made exactly at :15 waits for :30.
- A queued outgoing amount is reserved. Available balance equals Balance minus pending outgoing amounts. Pending incoming money cannot be spent yet.
- Interactive input checks the clock about once per second, including while the user is typing. Processing runs in the same loop as the menus, without a second thread changing balances.
- After a restart, overdue requests are processed once. While the app is closed, no code runs. The next startup catches up. This is a console application, not a server service.
- When input is redirected, the program checks due requests before each line rather than polling the keyboard. The scheduled processor is also tested directly with controlled timestamps.
- Currency conversion uses `amount * source SEK rate / target SEK rate`. Converted amounts are rounded to two decimals away from zero.
- Rates for new foreign currency conversions must be updated today by admin. The quote is fixed when the request is created. Later rate changes do not alter that request.
- Both balances are checked before either changes. A failed request remains in history and releases its reservation.

## Savings and loans

Savings interest is a **one year estimate**: `amount * annual rate`. The example 2% is a fictional setting. The app does not automatically credit interest. Admin can change the rate for new and existing savings accounts.

Loans are recorded in SEK, even if paid into an account in another currency. Loan interest is a one year estimate on remaining principal, without compounding or a payment schedule. Repayment in the app reduces principal only. Interest is not automatically charged.

For new borrowing:

```text
own money = max(0, balances in SEK - active loan principal - pending external spending)
new loan capacity = max(0, 5 * own money - active principal - pending loan payouts)
```

Subtracting borrowed money prevents repeated borrowing from increasing the limit indefinitely. For example, 10,000 SEK of own money allows 50,000 SEK in loans. Receiving that loan brings the account balance to 60,000 SEK, but creates a 50,000 SEK debt. Own money remains 10,000 SEK and the remaining capacity is zero. This is the project's conservative interpretation of the five-times rule.

Admin sets rates by Low, Medium or High credit rating and assigns a rating to each customer. New loans use the current rule. Existing loans keep their agreed rate. Borrowing is checked when the request is created, not continuously reapproved after market changes.

## Storage and login

Data is saved after completed changes to `bank-data.json` in the application's working directory. The previous saved version is kept in `bank-data.json.bak`. Set `BANK_DATA_PATH` to use another path. Failed login attempts and locks are saved too.

If a file exists but cannot be loaded, the application stops instead of replacing it with demo data. If saving fails, the application stops and the last successfully saved data remains available. The file checks are basic consistency checks, not full protection against manual tampering.

Three consecutive wrong passwords lock a known user. Successful login resets the failure count. Admin can unlock a user. Unknown usernames do not increase another user's failure count. Create a second admin before experimenting with locking the only admin. Restore a known backup if all admins are locked.

Passwords use a random salt and .NET's PBKDF2-SHA256 function. Plaintext passwords are not saved. The whole JSON file is not encrypted. This project assumes one running instance and trusted access to local files. It is a school demonstration, not banking infrastructure.

## Extra notifications

The profile contains email, an international phone number, a preferred channel and a threshold in SEK. Alerts are created only for successful money operations whose SEK value is **greater than** the threshold. An own-account transfer creates one alert for that customer. A transfer to another customer can alert both customers.

Preview is the default. Preview records appear under **10. Notifieringar** and do not contact external services. Code for real delivery is included, but real delivery was not tested with provider credentials. To enable it, set `BANK_SEND_NOTIFICATIONS=true` in the process environment before starting the app.

| Channel | Required environment variables |
| --- | --- |
| Email, SMTP with STARTTLS | BANK_SMTP_HOST, BANK_SMTP_PORT (usually 587), BANK_SMTP_USER, BANK_SMTP_PASSWORD, BANK_EMAIL_FROM |
| SMS, Twilio | BANK_TWILIO_ACCOUNT_SID, BANK_TWILIO_AUTH_TOKEN, BANK_TWILIO_FROM |

Use your own approved provider configuration. Enter secrets in the local process environment, never in source files or Git. Example PowerShell setup uses placeholders:

```powershell
$env:BANK_SEND_NOTIFICATIONS = "true"
$env:BANK_SMTP_HOST = "your-smtp-host"
$env:BANK_SMTP_PORT = "587"
$env:BANK_SMTP_USER = "your-smtp-user"
$env:BANK_SMTP_PASSWORD = "your-local-secret"
$env:BANK_EMAIL_FROM = "your-sender-address"
# Set the three Twilio variables too if SMS is selected.
dotnet run --project BankApp/BankApp.csproj
```

The sender tries each selected channel separately. “Accepted by mail server” or “Accepted by SMS provider” means the provider accepted the request. It does not confirm delivery to a person's device. Failure is shown in the notification record. There is no automatic retry or delivery callback. Sending may pause input briefly. Transaction state is saved before contacting a provider, so a sending failure does not undo a money operation.

## Planning, Git and review

- [Backlog and acceptance criteria](docs/Backlog.md)
- [Illustrative development and commit simulation](docs/CommitSimulation.md)
- [Git and Visual Studio instructions](docs/GitGuide.md)
- [Code guide](docs/CodeGuide.md)
- [Verification report](docs/TestReport.md)
- [Swedish reflection draft](docs/Reflection-SV.md)

Use **one shared GitHub repository** for the group. Build and test a coherent part before committing it. Members can work on branches and review pull requests. Do not commit data files, build output or credentials. The bundle is an optional copy of the real assistant-generated history and is not evidence of student contributions. Instructions for importing it are in GitGuide.

## Technical references for the extra parts

- [Microsoft: JSON polymorphism](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/polymorphism?pivots=dotnet-8-0)
- [Microsoft: PBKDF2](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.rfc2898derivebytes.pbkdf2?view=net-8.0)
- [Microsoft: SmtpClient](https://learn.microsoft.com/en-us/dotnet/api/system.net.mail.smtpclient?view=net-8.0)
- [Twilio: send SMS messages](https://www.twilio.com/docs/messaging/tutorials/how-to-send-sms-messages)
