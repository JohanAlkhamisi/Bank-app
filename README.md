[README.md](https://github.com/user-attachments/files/33506672/README.md)
# CHAS Bank

A small C# console bank app for our OOP course. The menus are in Swedish. The code comments and this README are in English.

## Start the app

1. Open `Bank app.slnx` in Visual Studio with .NET 10 installed.
2. Start the `Bank app` project.
3. Choose `1` to log in.

You can also run `dotnet run --project "Bank app/Bank app.csproj"` from this folder.

### Demo users

| Username | Password | Role |
| --- | --- | --- |
| admin | Admin123 | Admin |
| johan | johan123 | Customer |
| sara | Sara123 | Customer |

Usernames and passwords use an exact match. For example, `Johan` and `johan` are different names.

Johan starts with account 1001, which has 10000 SEK, and account 1003, which is empty. Sara starts with account 1002, which has 5000 SEK.

## Files and classes

| File | What it does |
| --- | --- |
| `Program.cs` | Starts the menu. Main has no bank or menu logic. |
| `Menu.cs` | Shows the menus, reads choices and prints results. It has different choices for admins and customers. |
| `Input.cs` | Reads text, whole numbers, decimal numbers and currencies. It also checks for due payments while the user types or waits. |
| `Bank.cs` | Keeps the lists and queue. It handles login, accounts, transfers, loans, rates, profiles, statistics and notifications. |
| `User.cs` | Keeps a user's name, password, role, failed login attempts, contact details and credit level. It checks passwords. |
| `BankAccount.cs` | Keeps an account's number, owner, name, currency and balance. Deposit and Withdraw control changes to the private balance. |
| `SavingsAccount.cs` | Inherits from BankAccount. It calculates savings interest at 2 percent per year. |
| `Loan.cs` | Keeps a loan's owner, amount, agreed interest rate and payout status. It calculates one year's interest. |
| `Transaction.cs` | Keeps one transfer, deposit or loan payout, including its amount, accounts, times and status. |
| `Notification.cs` | Keeps one simulated email or SMS, with the owner, account, transaction, address, message and time. |
| `Enums.cs` | Keeps the fixed choices used by other classes. |

### Enums

| Enum | Choices |
| --- | --- |
| Currency | SEK, EUR, USD |
| TransactionType | Transfer, Deposit, Loan |
| TransactionStatus | Pending, Completed, Failed |
| CreditRating | Low, Medium, High |

## How the objects work together

Main creates one Menu object. Menu creates one Bank object and one Input object. Menu gives Input the same Bank object, so both use the same accounts and payment queue.

Bank keeps these collections:

| Collection | What it keeps |
| --- | --- |
| `List<User>` | Users, including the admin. |
| `List<BankAccount>` | Normal accounts and savings accounts. |
| `Queue<Transaction>` | Payments waiting for the next quarter-hour. The oldest request is checked first. |
| `List<Transaction>` | Payment history, including waiting, completed and failed payments. |
| `List<Loan>` | Loans waiting for payout and loans already paid out. |
| `List<Notification>` | Simulated emails and SMS messages. |
| `Dictionary<Currency, decimal>` | How many SEK one unit of each currency is worth. |
| `Dictionary<CreditRating, decimal>` | The loan interest rate for each credit level. |

BankAccount.OwnerId matches User.Id. Loan.UserId and Notification.UserId also match User.Id. Transactions use account numbers. A loan payout also keeps a LoanId. These numbers connect the objects; User does not have its own account list.

The queue and history list keep the same Transaction object. When Bank changes its status, the history shows the new status too.

SavingsAccount uses the shared account code from BankAccount. It overrides CalculateYearlyInterest, so the same method call gives savings interest for a savings account and zero interest for a normal account.

## Payments, rates and loans

- Payments wait until the next time ending in :00, :15, :30 or :45. A request entered at 14:07 is scheduled for 14:15.
- Input checks the clock while the program waits for keyboard input. The program must stay open for payments to run.
- Waiting outgoing transfers reduce the available balance, so the same money cannot be promised twice.
- Admin menu 3 saves today's EUR and USD rates. The rates are entered by the admin, not downloaded. The starting values are examples and must be confirmed before currency exchange is allowed.
- A transfer saves the quoted receiving amount when it is entered. Later rate changes do not change that transfer's quote.
- The loan limit is five times the customer's own money, minus existing loan amounts. Waiting loans also count, and money from a paid loan does not count as the customer's own money.
- Admin can change the loan rate for each credit level. A new loan keeps the rate agreed when it was requested.
- Amounts must be above zero, at most 100000 per request and have at most two decimal places. Both a comma and a dot can be used for decimals.
- Savings and loan interest are one-year estimates. The app does not add interest automatically or handle loan repayments.

## Notifications: a simple simulation

No real emails or SMS messages are sent. The app creates message objects and shows them under customer menu 10, `Mina notifieringar`.

A message is created after a completed payment above 10000 in the affected account's currency. Exactly 10000 does not create a message. The limit is checked separately for each account, using its outgoing or incoming amount. It is not a common limit converted to SEK.

- An email address gives one simulated email.
- A phone number gives one simulated SMS.
- Both contact details give both messages.
- No contact details means no message. Add them under menu 9 before the payment completes.
- A transfer can notify both the sender and the receiver. An own-account transfer can create a message for each account.
- A large deposit or loan payout can notify the receiving account's owner.
- Waiting, rejected and failed payments do not create messages.
- Each customer can only view their own messages. Later changes to contact details do not change old message addresses.
