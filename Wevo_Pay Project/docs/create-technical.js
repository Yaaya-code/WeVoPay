const { Document, Packer, Paragraph, TextRun, Table, TableRow, TableCell,
        Header, Footer, AlignmentType, HeadingLevel, BorderStyle, WidthType,
        ShadingType, PageNumber, PageBreak, LevelFormat } = require("docx");
const fs = require("fs");
const path = require("path");

const BLUE = "1D4ED8";
const NAVY = "0F172A";
const GRAY = "475569";
const LIGHT = "EFF6FF";
const WHITE = "FFFFFF";
const BORDER = "CBD5E1";

const border = { style: BorderStyle.SINGLE, size: 8, color: BORDER };
const borders = { top: border, bottom: border, left: border, right: border };
const noBorder = { style: BorderStyle.NONE, size: 0, color: "FFFFFF" };
const noBorders = { top: noBorder, bottom: noBorder, left: noBorder, right: noBorder };

function p(text, opts = {}) {
  return new Paragraph({
    spacing: { after: opts.after ?? 120, before: opts.before ?? 0 },
    alignment: opts.align,
    children: [new TextRun({
      text,
      bold: opts.bold,
      italics: opts.italics,
      size: opts.size || 22,
      font: "Arial",
      color: opts.color || NAVY
    })]
  });
}

function h1(text) {
  return new Paragraph({
    heading: HeadingLevel.HEADING_1,
    spacing: { before: 280, after: 160 },
    children: [new TextRun({ text, bold: true, size: 32, font: "Arial", color: BLUE })]
  });
}

function h2(text) {
  return new Paragraph({
    heading: HeadingLevel.HEADING_2,
    spacing: { before: 220, after: 120 },
    children: [new TextRun({ text, bold: true, size: 26, font: "Arial", color: NAVY })]
  });
}

function bullet(text, ref = "bullets") {
  return new Paragraph({
    numbering: { reference: ref, level: 0 },
    spacing: { after: 80 },
    children: [new TextRun({ text, size: 21, font: "Arial", color: GRAY })]
  });
}

function cell(text, w, opts = {}) {
  return new TableCell({
    borders,
    width: { size: w, type: WidthType.DXA },
    shading: opts.fill ? { fill: opts.fill, type: ShadingType.CLEAR } : undefined,
    margins: { top: 60, bottom: 60, left: 100, right: 100 },
    children: [new Paragraph({
      children: [new TextRun({
        text,
        bold: opts.bold,
        size: opts.size || 18,
        font: "Arial",
        color: opts.color || NAVY
      })]
    })]
  });
}

function row(cells, widths, header = false) {
  return new TableRow({
    children: cells.map((c, i) => cell(c, widths[i], {
      bold: header,
      fill: header ? LIGHT : undefined,
      color: header ? BLUE : NAVY
    }))
  });
}

function table(headers, data, widths) {
  const total = widths.reduce((a, b) => a + b, 0);
  return new Table({
    width: { size: total, type: WidthType.DXA },
    columnWidths: widths,
    rows: [
      row(headers, widths, true),
      ...data.map(r => row(r, widths, false))
    ]
  });
}

function codeBlock(lines) {
  return lines.map(line => new Paragraph({
    spacing: { after: 40 },
    shading: { fill: "F1F5F9", type: ShadingType.CLEAR },
    children: [new TextRun({ text: line || " ", size: 17, font: "Consolas", color: "1E293B" })]
  }));
}

const W = 9360;
const w2 = [2800, 6560];
const w3 = [2400, 3480, 3480];
const w4 = [2200, 2400, 2400, 2360];

const doc = new Document({
  styles: {
    default: { document: { run: { font: "Arial", size: 22 } } },
    paragraphStyles: [
      { id: "Heading1", name: "Heading 1", basedOn: "Normal", next: "Normal", quickFormat: true,
        run: { size: 32, bold: true, font: "Arial", color: BLUE },
        paragraph: { spacing: { before: 280, after: 160 }, outlineLevel: 0 } },
      { id: "Heading2", name: "Heading 2", basedOn: "Normal", next: "Normal", quickFormat: true,
        run: { size: 26, bold: true, font: "Arial", color: NAVY },
        paragraph: { spacing: { before: 220, after: 120 }, outlineLevel: 1 } },
    ]
  },
  numbering: {
    config: [
      { reference: "bullets", levels: [{ level: 0, format: LevelFormat.BULLET, text: "•",
        alignment: AlignmentType.LEFT,
        style: { paragraph: { indent: { left: 720, hanging: 360 } } } }] },
      { reference: "numbers", levels: [{ level: 0, format: LevelFormat.DECIMAL, text: "%1.",
        alignment: AlignmentType.LEFT,
        style: { paragraph: { indent: { left: 720, hanging: 360 } } } }] },
      { reference: "qna", levels: [{ level: 0, format: LevelFormat.DECIMAL, text: "Q%1.",
        alignment: AlignmentType.LEFT,
        style: { paragraph: { indent: { left: 720, hanging: 360 } } } }] },
    ]
  },
  sections: [{
    properties: {
      page: {
        size: { width: 12240, height: 15840 },
        margin: { top: 1008, right: 1008, bottom: 1008, left: 1008 }
      }
    },
    headers: {
      default: new Header({ children: [
        new Paragraph({
          border: { bottom: { style: BorderStyle.SINGLE, size: 12, color: BLUE, space: 4 } },
          spacing: { after: 120 },
          children: [
            new TextRun({ text: "WeVo Pay  |  Technical Instructor Guide", bold: true, size: 18, font: "Arial", color: BLUE }),
            new TextRun({ text: "\tConfidential — for defense/Q&A", size: 16, font: "Arial", color: GRAY })
          ],
          tabStops: [{ type: "right", position: 10224 }]
        })
      ]})
    },
    footers: {
      default: new Footer({ children: [
        new Paragraph({
          border: { top: { style: BorderStyle.SINGLE, size: 6, color: BORDER, space: 6 } },
          alignment: AlignmentType.CENTER,
          children: [
            new TextRun({ text: "Page ", size: 16, font: "Arial", color: GRAY }),
            new TextRun({ children: [PageNumber.CURRENT], size: 16, font: "Arial", color: GRAY }),
            new TextRun({ text: "  |  ASP.NET Core MVC  |  Answer sheet for instructor questions", size: 16, font: "Arial", color: GRAY })
          ]
        })
      ]})
    },
    children: [
      // COVER
      new Paragraph({ spacing: { before: 1200 }, children: [] }),
      p("TECHNICAL REFERENCE", { size: 20, color: BLUE, bold: true, after: 80 }),
      p("WeVo Pay", { size: 56, bold: true, color: NAVY, after: 80 }),
      p("How everything works — for instructor questions", { size: 24, color: GRAY, after: 200 }),
      p("ASP.NET Core MVC  ·  Entity Framework Core  ·  SQL Server  ·  Cookie Auth", { size: 18, color: GRAY, after: 400 }),
      table(
        ["Item", "Detail"],
        [
          ["Project type", "ASP.NET Core MVC web application"],
          ["Purpose of this doc", "Defend technical decisions and explain implementation"],
          ["Related presentation", "WeVo_Pay_Project_Presentation.pdf"],
          ["Default admin", "admin / Admin@123 (seeded on startup)"]
        ],
        [3200, 6160]
      ),

      new Paragraph({ children: [new PageBreak()] }),

      // 1
      h1("1. How to use this document"),
      p("If the instructor asks HOW you built something, answer from the matching section:"),
      bullet("Architecture / layers / DI → Section 2–3"),
      bullet("Database / entities / migrations → Section 4"),
      bullet("Authentication & roles → Section 5"),
      bullet("Transfer business flow → Section 6"),
      bullet("Auto-cancel timer → Section 7"),
      bullet("Messages / referral / rates / settings → Section 8"),
      bullet("UI structure → Section 9"),
      bullet("Quick Q&A answers → Section 10"),

      // 2
      h1("2. High-level architecture"),
      p("WeVo Pay uses classic layered MVC:"),
      bullet("Views (Razor): HTML UI, forms, timers, layouts"),
      bullet("Controllers: receive HTTP, authorize, validate, call services, return views"),
      bullet("Services: business rules (fee, verify, complete, expire, messages)"),
      bullet("Data: AppDbContext + EF Core maps models to SQL Server tables"),
      p("Dependency Injection registers all services in Program.cs as Scoped. Background job is HostedService. Exchange rates use IHttpClientFactory + IMemoryCache.", { after: 160 }),
      p("Request path example (Create Transfer):", { bold: true }),
      ...codeBlock([
        "Browser POST /Transfer/Create",
        "  → TransferController.Create(dto)  [Authorize]",
        "  → TransferService.CreateTransferAsync(userId, dto)",
        "  → AppDbContext.TransferRequests.Add(...) + SaveChanges",
        "  → Redirect Success view"
      ]),

      // 3
      h1("3. Project structure (what lives where)"),
      table(
        ["Folder", "Responsibility"],
        [
          ["Controllers/", "HTTP endpoints, auth attributes, ViewBag"],
          ["Services/ + Interfaces/", "Business logic contracts + implementations"],
          ["Models/", "EF entities = database tables"],
          ["DTOs/", "Form/input/view models without EF navigation clutter"],
          ["Data/", "AppDbContext, DbSeeder"],
          ["Enums/", "TransferStatus, UserRole, TransactionStatus"],
          ["Views/", "Razor pages per feature area"],
          ["wwwroot/", "CSS, JS, images (static files)"],
          ["Migrations/", "EF schema history"]
        ],
        [2800, 6560]
      ),
      h2("Main controllers"),
      table(
        ["Controller", "Role", "Audience"],
        [
          ["AccountController", "Register / Login / Logout", "Public + auth"],
          ["UserController", "Dashboard, Profile, Settings", "User"],
          ["TransferController", "Create, MyTransfers, Details, Success", "User"],
          ["AdminController", "Queues: verify/complete/reject, stats", "Admin"],
          ["CompanyWalletController", "Manage company wallets", "Admin"],
          ["SystemSettingController", "Fee %, min/max amounts", "Admin"],
          ["MessageController", "User chat + admin inbox", "Both"],
          ["RatesController", "Currency + gold in EGP", "User"],
          ["TransactionController", "Transaction listing/details", "Admin"]
        ],
        [2800, 3800, 2760]
      ),

      // 4
      h1("4. Database & domain model"),
      p("ORM: Entity Framework Core. Provider: SQL Server. Connection: appsettings.json key ConString (case-insensitive)."),
      h2("Core tables"),
      table(
        ["Entity", "Key fields", "Notes"],
        [
          ["User", "UserName, Email, Phone, PasswordHash, Role", "Role enum stored as string"],
          ["CompanyWallet", "WalletName, WalletNumber, IsActive", "Platform mobile wallets"],
          ["TransferRequest", "Amount, Fee, Status, InstaPayAddress", "Core business object"],
          ["Transaction", "Amount, ReferenceNumber, Status", "Created on Complete"],
          ["SystemSetting", "FeePercentage, Min/Max amount", "Global config"],
          ["SupportMessage", "UserId, SenderId, Body, IsFromAdmin", "Support chat"]
        ],
        [2200, 3600, 3560]
      ),
      h2("Important relationships"),
      bullet("TransferRequest.UserId → User (many transfers per user)"),
      bullet("TransferRequest.CompanyWalletId → CompanyWallet"),
      bullet("Transaction 1:1 TransferRequest (created when completed)"),
      bullet("User.ReferredByUserId → User (optional self-reference)"),
      bullet("SupportMessage.UserId = conversation owner (customer)"),
      h2("Migrations that changed schema in this work"),
      bullet("AddSupportMessages → SupportMessages table"),
      bullet("AddUserReferral → Users.ReferredByUserId"),
      p("The 1-hour auto-cancel does NOT change schema; it only updates Status to Cancelled.", { italics: true, after: 160 }),

      // 5
      h1("5. Authentication & authorization"),
      h2("How login works"),
      bullet("UserService.LoginAsync finds user by email OR username (case-insensitive)"),
      bullet("PasswordHasher.VerifyHashedPassword checks password"),
      bullet("AccountController builds ClaimsIdentity with NameIdentifier, Name, Email, Role"),
      bullet("HttpContext.SignInAsync writes authentication cookie"),
      h2("Roles"),
      bullet("UserRole.User → user app (dashboard, transfers, rates, messages)"),
      bullet("UserRole.Admin → admin panel ([Authorize(Roles = \"Admin\")])"),
      h2("How to answer: Why cookies not JWT?"),
      p("This is a server-rendered MVC app. Cookie auth is the standard, simple choice for browser sessions. JWT is more common for pure APIs/SPAs."),
      h2("Admin seed"),
      p("DbSeeder.SeedAdminAsync runs on startup: migrates DB and ensures admin exists with password Admin@123 (username admin)."),

      // 6
      h1("6. Transfer engine (business + code)"),
      h2("Statuses"),
      table(
        ["Status", "Meaning", "Who changes it"],
        [
          ["Pending", "Created; waiting verification", "User create / system start"],
          ["Verified", "Admin confirmed payment received", "Admin Verify"],
          ["Completed", "Paid to InstaPay; Transaction row", "Admin Complete"],
          ["Rejected", "Admin refused", "Admin Reject"],
          ["Cancelled", "Timeout without verify (1 hour)", "Background + on-read cancel"]
        ],
        [1800, 4200, 3360]
      ),
      h2("CreateTransferAsync steps"),
      bullet("Load user; fail if missing"),
      bullet("Load active CompanyWallet; fail if missing"),
      bullet("Load active SystemSetting; fail if missing"),
      bullet("Validate InstaPay address and amount vs Min/Max"),
      bullet("Fee = Round(amount * FeePercentage / 100, 2)"),
      bullet("TotalAmount = amount + fee"),
      bullet("Insert TransferRequest Status=Pending, CreatedAt=UtcNow"),
      h2("Verify / Complete / Reject"),
      bullet("Verify: only if Pending (after running expiry cancel); sets VerifiedAt + VerifiedByAdminId"),
      bullet("Complete: only if Verified and no Transaction yet; creates Transaction with reference WP-yyyyMMdd-XXXXXX"),
      bullet("Reject: from Pending or Verified; sets RejectedAt + RejectedByAdminId"),
      h2("Fee: is it constant?"),
      p("No. Create page and CreateTransferAsync both read SystemSettings.FeePercentage. Changing fee in admin updates NEW transfers. Old transfers keep stored FeePercentage."),

      // 7
      h1("7. One-hour auto-cancel (timer)"),
      p("Constant: TransferService.PendingTimeout = TimeSpan.FromHours(1)"),
      p("Method: CancelExpiredPendingTransfersAsync()"),
      ...codeBlock([
        "cutoff = UtcNow - 1 hour",
        "find TransferRequests where Status==Pending AND CreatedAt <= cutoff",
        "set Status = Cancelled; SaveChanges"
      ]),
      p("Triggered by:", { bold: true, before: 120 }),
      bullet("TransferExpiryBackgroundService every ~1 minute (IHostedService)"),
      bullet("Also when listing/details/verify runs (so UI is not stale)"),
      p("UI: countdown on Details and My Transfers; banners on Create/Success/Dashboard."),
      h2("Why not only JavaScript timer?"),
      p("JS countdown is display only. Real cancel must be server-side so it works even if the user closes the browser."),

      // 8
      h1("8. Other features (how they work)"),
      h2("Message Center"),
      bullet("SupportMessage rows; thread key = customer UserId"),
      bullet("User send: IsFromAdmin=false; Admin send: IsFromAdmin=true, UserId=target"),
      bullet("Opening chat marks the other side unread messages as IsRead=true"),
      bullet("MessageController: Roles User for Index/Send; Roles Admin for AdminIndex/AdminConversation/AdminReply"),
      h2("Referral"),
      bullet("RegisterDto.WasReferred + ReferralCode"),
      bullet("Parse username from plain name, ?ref=, or /ref/username"),
      bullet("Store Users.ReferredByUserId"),
      bullet("Profile builds link: /Account/Register?refCode=UserName"),
      h2("User Settings"),
      bullet("UpdateProfile: unique email/phone excluding self; re-SignIn cookie"),
      bullet("ChangePassword: verify current hash, require different new password (min 6)"),
      h2("Exchange Rates"),
      bullet("ExchangeRateService calls open.er-api.com (USD base) → convert to EGP"),
      bullet("Gold: gold-api.com XAU price USD/oz → convert using USD/EGP"),
      bullet("Cached 30 minutes in IMemoryCache"),
      bullet("No DB tables for rates"),

      // 9
      h1("9. UI / front-end technical notes"),
      bullet("Layouts: _Layout.cshtml (users/guests), _AdminLayout.cshtml (admin)"),
      bullet("Navbar branches: FullWidth (auth), Authenticated Admin, Authenticated User, Guest"),
      bullet("Styles mainly in wwwroot/css/wevopay.css and dashboard.css"),
      bullet("Client JS only for UX: fee calculator, password toggle, countdown timers, copy referral"),
      bullet("Real validation and security stay on the server"),

      // 10
      h1("10. Instructor Q&A — ready answers"),

      h2("Q: Why MVC not Web API + React?"),
      p("Course-style full-stack teaching target. Razor MVC keeps one project, clear Controllers/Views/Models separation, faster for academic delivery."),

      h2("Q: Where is business logic?"),
      p("In Services (e.g. TransferService), not in Views. Controllers orchestrate HTTP only."),

      h2("Q: How do you prevent users seeing others' transfers?"),
      p("GetUserTransferByIdAsync filters by transferId AND userId from claims. Admin endpoints require Admin role."),

      h2("Q: How are passwords stored?"),
      p("Never plain text. ASP.NET Core Identity PasswordHasher produces a salted hash in User.PasswordHash."),

      h2("Q: How does the fee update when admin changes settings?"),
      p("Create form loads FeePercentage from SystemSetting via ISystemSettingService. CreateTransferAsync recomputes fee from active settings at save time."),

      h2("Q: How does auto-cancel work if the server restarts?"),
      p("On next run, hosted service resumes and queries Pending older than 1 hour. Also lazy cancel on transfer reads."),

      h2("Q: What if admin verifies after expiry?"),
      p("VerifyTransferAsync first runs CancelExpiredPendingTransfersAsync. Expired pending becomes Cancelled, so verify returns false."),

      h2("Q: How are messages secured?"),
      p("User endpoints only for Role User and thread = current user id. Admin endpoints require Role Admin."),

      h2("Q: Did every feature need a migration?"),
      p("No. Messages and referral needed schema changes. UI polish, settings password change, timer cancel, and rates did not need new tables."),

      h2("Q: What external APIs do you use?"),
      p("open.er-api.com for FX; api.gold-api.com for gold spot. Results cached; failures degrade gracefully."),

      h2("Q: How would you improve this for production?"),
      bullet("Move secrets to User Secrets / Azure Key Vault"),
      bullet("Add unit tests for TransferService status transitions"),
      bullet("Real-time messaging (SignalR)"),
      bullet("Payment confirmation proofs / upload receipts"),
      bullet("Audit log table for admin actions"),
      bullet("Stronger password policy and lockout"),

      // 11
      h1("11. Key files cheat-sheet"),
      table(
        ["Topic", "Primary files"],
        [
          ["Startup / DI / seed", "Program.cs, Data/DbSeeder.cs"],
          ["DB mapping", "Data/AppDbContext.cs, Models/*"],
          ["Create/verify transfer", "Services/TransferService.cs, Controllers/TransferController.cs, AdminController.cs"],
          ["Timer", "TransferService.CancelExpired..., TransferExpiryBackgroundService.cs"],
          ["Auth", "AccountController.cs, UserService.LoginAsync/RegisterAsync"],
          ["Messages", "MessageService.cs, MessageController.cs, Models/SupportMessage.cs"],
          ["Rates", "ExchangeRateService.cs, RatesController.cs"],
          ["UI shell", "Views/Shared/_Layout.cshtml, wwwroot/css/wevopay.css"]
        ],
        [2800, 6560]
      ),

      h1("12. Demo script (if asked to show)"),
      bullet("1) Login as admin → Dashboard stats, pending list"),
      bullet("2) Logout → Register/Login user → Create transfer → see 1h timer"),
      bullet("3) Admin verify → complete → user sees Completed"),
      bullet("4) Messages user ↔ admin"),
      bullet("5) Rates page currencies + gold"),
      bullet("6) Settings change password / profile"),

      new Paragraph({ spacing: { before: 400 }, children: [] }),
      p("End of technical reference — use Section 10 for spoken answers.", { italics: true, color: GRAY, size: 18 })
    ]
  }]
});

Packer.toBuffer(doc).then(buf => {
  const out = path.join(__dirname, "WeVo_Pay_Technical_Guide.docx");
  fs.writeFileSync(out, buf);
  console.log("Wrote", out);
});
